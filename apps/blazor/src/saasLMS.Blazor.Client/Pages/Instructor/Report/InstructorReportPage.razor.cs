using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.ReportingService.Reports;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Instructor.Report;

[Authorize(Roles = LmsRoles.Instructor)]
public partial class InstructorReportPage : AbpComponentBase
{
    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IReportingAppService ReportingAppService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    // ── Course list state ────────────────────────────────────────────────────
    private bool _isLoadingCourses = true;
    private List<CourseListItemDto> _courses = new();

    // ── Selected course & report state ──────────────────────────────────────
    private CourseListItemDto? _selectedCourse;
    private bool _isLoadingReport;
    private ReportTab _activeTab = ReportTab.ClassProgress;

    // ── Report data ──────────────────────────────────────────────────────────
    private ClassProgressViewDto? _classProgress;
    private CourseOutcomeReportViewDto? _courseOutcome;

    // ── ClassProgress: bucket shorthand ──────────────────────────────────────
    private int CpB0   => _classProgress?.Bucket_0_25  ?? 0;
    private int CpB26  => _classProgress?.Bucket_26_50 ?? 0;
    private int CpB51  => _classProgress?.Bucket_51_75 ?? 0;
    private int CpB76  => _classProgress?.Bucket_76_99 ?? 0;
    private int CpB100 => _classProgress?.Bucket_100   ?? 0;

    private int CpBMax => Math.Max(1, Math.Max(CpB0, Math.Max(CpB26, Math.Max(CpB51, Math.Max(CpB76, CpB100)))));

    // Returns pixel height (0–120) proportional to count vs max bucket
    private int CpBarPx(int count) => count * 120 / CpBMax;

    // ── ClassProgress: SVG donut (r=40, viewBox="0 0 100 100") ───────────────
    // C = 2*pi*40 ~= 251.33
    // Segment1 (Completed)  : dasharray=CpDashComp,   dashoffset=0
    // Segment2 (InProgress) : dasharray=CpDashInProg, dashoffset=CpOffsetInProg
    // dashoffset = C - compArc  => gap of compArc units, then orange starts
    private static double DonutCirc => 2.0 * Math.PI * 40.0;
    private double CpDonutDenom => Math.Max(1.0, (double)((_classProgress?.CompletedCount ?? 0) + (_classProgress?.InProgressCount ?? 0)));
    private double CpCompArc    => (_classProgress?.CompletedCount  ?? 0) / CpDonutDenom * DonutCirc;
    private double CpInProgArc  => (_classProgress?.InProgressCount ?? 0) / CpDonutDenom * DonutCirc;

    private static readonly System.Globalization.CultureInfo _inv = System.Globalization.CultureInfo.InvariantCulture;
    private string CpDashComp    => CpCompArc.ToString("F2", _inv)   + " " + DonutCirc.ToString("F2", _inv);
    private string CpDashInProg  => CpInProgArc.ToString("F2", _inv) + " " + DonutCirc.ToString("F2", _inv);
    private string CpOffsetInProg => (DonutCirc - CpCompArc).ToString("F2", _inv);

    // ── ClassProgress: insights ───────────────────────────────────────────────
    private double CpCompletionRate =>
        (_classProgress?.ActiveEnrollmentCount ?? 0) > 0
        ? (_classProgress?.CompletedCount ?? 0) * 100.0 / (_classProgress?.ActiveEnrollmentCount ?? 0)
        : 0.0;

    private double CpDropRate =>
        (_classProgress?.TotalStudents ?? 0) > 0
        ? ((_classProgress?.TotalStudents ?? 0) - (_classProgress?.ActiveEnrollmentCount ?? 0)) * 100.0 / (_classProgress?.TotalStudents ?? 0)
        : 0.0;

    private string CpCenterPct => CpCompletionRate.ToString("F0") + "%";

    // ────────────────────────────────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        if (!CurrentUser.IsInRole(LmsRoles.Instructor))
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadCoursesAsync();
    }

    private async Task LoadCoursesAsync()
    {
        try
        {
            _isLoadingCourses = true;
            var instructorId = CurrentUser.Id!.Value;
            _courses = await CourseCatalogAppService.GetCoursesByInstructorAsync(instructorId);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingCourses = false;
        }
    }

    private async Task SelectCourse(CourseListItemDto course)
    {
        _selectedCourse = course;
        _activeTab = ReportTab.ClassProgress;
        _classProgress = null;
        _courseOutcome = null;

        await LoadClassProgressAsync();
    }

    private void ClearSelection()
    {
        _selectedCourse = null;
        _classProgress = null;
        _courseOutcome = null;
    }

    private async Task SetTab(ReportTab tab)
    {
        _activeTab = tab;

        if (tab == ReportTab.ClassProgress && _classProgress == null)
            await LoadClassProgressAsync();
        else if (tab == ReportTab.CourseOutcome && _courseOutcome == null)
            await LoadCourseOutcomeAsync();
    }

    private async Task LoadClassProgressAsync()
    {
        if (_selectedCourse == null) return;

        try
        {
            _isLoadingReport = true;
            StateHasChanged();

            _classProgress = await ReportingAppService.GetClassProgressAsync(_selectedCourse.CourseId);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingReport = false;
        }
    }

    private async Task LoadCourseOutcomeAsync()
    {
        if (_selectedCourse == null) return;

        try
        {
            _isLoadingReport = true;
            StateHasChanged();

            // TODO: Uncomment when ready to wire up to the real API
            // _courseOutcome = await ReportingAppService.GetCourseOutcomeReportAsync(_selectedCourse.CourseId);
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingReport = false;
        }
    }
}

public enum ReportTab
{
    ClassProgress,
    CourseOutcome
}