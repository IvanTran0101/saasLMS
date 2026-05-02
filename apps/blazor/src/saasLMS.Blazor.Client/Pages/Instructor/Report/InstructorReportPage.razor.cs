using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.ReportingService.Reports;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.Identity;

namespace saasLMS.Blazor.Client.Pages.Instructor.Report;

[Authorize(Roles = LmsRoles.Instructor)]
public partial class InstructorReportPage : AbpComponentBase
{
    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IReportingAppService ReportingAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;

    [Inject]
    private IIdentityUserAppService IdentityUserAppService { get; set; } = default!;

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
    private ScoreDistributionData? _scoreDist;

    // ── Staleness tracking ───────────────────────────────────────────────────
    /// <summary>
    /// Refetch Reporting-backed tabs (ClassProgress / CourseOutcome) if data is
    /// older than this threshold. EnrolledStudents does a live parallel user-info
    /// lookup so it keeps the null guard instead (re-select course to refresh it).
    /// </summary>
    private static readonly TimeSpan _staleThreshold = TimeSpan.FromSeconds(30);
    private DateTime _classProgressLoadedAt  = DateTime.MinValue;
    private DateTime _courseOutcomeLoadedAt  = DateTime.MinValue;

    // ── Enrolled Students tab ────────────────────────────────────────────────
    private bool _isLoadingStudents;
    private List<EnrolledStudentRow> _enrolledStudents = new();

    private sealed record EnrolledStudentRow(
        string DisplayName,
        string Email,
        EnrollmentStatus Status,
        DateTime EnrolledAt);

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

    // ── CourseOutcome: score distribution bucket shorthand ───────────────────
    private int SdB0   => _scoreDist?.Bucket_0_25  ?? 0;
    private int SdB26  => _scoreDist?.Bucket_26_50 ?? 0;
    private int SdB51  => _scoreDist?.Bucket_51_75 ?? 0;
    private int SdB76  => _scoreDist?.Bucket_76_99 ?? 0;
    private int SdB100 => _scoreDist?.Bucket_100   ?? 0;

    private int SdBMax => Math.Max(1, Math.Max(SdB0, Math.Max(SdB26, Math.Max(SdB51, Math.Max(SdB76, SdB100)))));

    // Returns pixel height (0–120) proportional to count vs max bucket
    private int SdBarPx(int count) => count * 120 / SdBMax;

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
        _selectedCourse          = course;
        _activeTab               = ReportTab.ClassProgress;
        _classProgress           = null;
        _courseOutcome           = null;
        _scoreDist               = null;
        _enrolledStudents        = new();
        _classProgressLoadedAt   = DateTime.MinValue;
        _courseOutcomeLoadedAt   = DateTime.MinValue;

        await LoadClassProgressAsync();
    }

    private void ClearSelection()
    {
        _selectedCourse         = null;
        _classProgress          = null;
        _courseOutcome          = null;
        _scoreDist              = null;
        _enrolledStudents       = new();
        _classProgressLoadedAt  = DateTime.MinValue;
        _courseOutcomeLoadedAt  = DateTime.MinValue;
    }

    private async Task SetTab(ReportTab tab)
    {
        // Always update the visual tab immediately so the button appears active.
        _activeTab = tab;

        // Don't start a second load if one is already in flight.
        if (_isLoadingReport || _isLoadingStudents) return;

        if (tab == ReportTab.ClassProgress &&
            (_classProgress == null || DateTime.Now - _classProgressLoadedAt > _staleThreshold))
        {
            await LoadClassProgressAsync();
        }
        else if (tab == ReportTab.CourseOutcome &&
                 (_courseOutcome == null || DateTime.Now - _courseOutcomeLoadedAt > _staleThreshold))
        {
            await LoadCourseOutcomeAsync();
        }
        else if (tab == ReportTab.EnrolledStudents && _enrolledStudents.Count == 0)
        {
            // EnrolledStudents does a live parallel user-info lookup — keep the
            // null guard. Re-select the course to force a refresh.
            await LoadEnrolledStudentsAsync();
        }
    }

    private async Task LoadClassProgressAsync()
    {
        if (_selectedCourse == null) return;

        try
        {
            _isLoadingReport = true;
            StateHasChanged();

            _classProgress        = await ReportingAppService.GetClassProgressAsync(_selectedCourse.CourseId);
            _classProgressLoadedAt = DateTime.Now;
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

            _courseOutcome = await ReportingAppService.GetCourseOutcomeReportAsync(_selectedCourse.CourseId);
            _courseOutcomeLoadedAt = DateTime.Now;

            _scoreDist = string.IsNullOrEmpty(_courseOutcome?.ScoreDistributionJson)
                ? null
                : JsonSerializer.Deserialize<ScoreDistributionData>(_courseOutcome.ScoreDistributionJson);
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

    private async Task LoadEnrolledStudentsAsync()
    {
        if (_selectedCourse == null) return;

        try
        {
            _isLoadingStudents = true;
            StateHasChanged();

            var enrollments = await EnrollmentAppService.GetEnrollmentsByCourseAsync(_selectedCourse.CourseId);

            // Lookup user info in parallel
            var userTasks = enrollments.Select(async e =>
            {
                string displayName;
                string email;
                try
                {
                    var user = await IdentityUserAppService.GetAsync(e.StudentId);
                    var fullName = $"{user.Name} {user.Surname}".Trim();
                    displayName = string.IsNullOrEmpty(fullName) ? user.UserName : fullName;
                    email = user.Email ?? string.Empty;
                }
                catch
                {
                    displayName = e.StudentId.ToString()[..8] + "…";
                    email = string.Empty;
                }

                return new EnrolledStudentRow(displayName, email, e.Status, e.EnrolledAt);
            });

            _enrolledStudents = (await Task.WhenAll(userTasks)).ToList();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingStudents = false;
        }
    }

    // ── ScoreDistributionJson shape (mirrors server-side ScoreDistribution) ──
    private sealed class ScoreDistributionData
    {
        public int Bucket_0_25  { get; set; }
        public int Bucket_26_50 { get; set; }
        public int Bucket_51_75 { get; set; }
        public int Bucket_76_99 { get; set; }
        public int Bucket_100   { get; set; }
    }
}

public enum ReportTab
{
    ClassProgress,
    CourseOutcome,
    EnrolledStudents
}
