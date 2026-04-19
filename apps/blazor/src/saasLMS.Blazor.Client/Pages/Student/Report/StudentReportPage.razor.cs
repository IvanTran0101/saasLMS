using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.LessonProgresses;
using saasLMS.ReportingService.Reports;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Report;

[Authorize(Roles = LmsRoles.Student)]
public partial class StudentReportPage : AbpComponentBase
{
    [Inject] 
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;

    [Inject]
    private ILearningProgressAppService LearningProgressAppService { get; set; } = default!;

    [Inject]
    private IReportingAppService ReportingAppService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    // ── Course list state ────────────────────────────────────────────────────
    private bool _isLoadingCourses = true;
    private List<CourseListItemDto> _enrolledCourses = new();
    private Dictionary<Guid, CourseProgressDto> _progressMap = new();

    // ── Selected course & report state ──────────────────────────────────────
    private CourseListItemDto? _selectedCourse;
    private bool _isLoadingReport;
    private StudentReportTab _activeTab = StudentReportTab.Overview;

    // ── Report data ──────────────────────────────────────────────────────────
    private StudentCourseProgressViewDto? _studentProgress;

    // ── Overview tab: progress bar shorthand ─────────────────────────────────
    private int OverallPct       => (int)Math.Round(_studentProgress?.OverallProgress            ?? 0);
    private int LessonPct        => (int)Math.Round(_studentProgress?.LessonCompletionPercent    ?? 0);
    private int AssignmentPct    => (int)Math.Round(_studentProgress?.AssignmentCompletionPercent ?? 0);
    private int QuizPct          => (int)Math.Round(_studentProgress?.QuizCompletionPercent      ?? 0);

    // ── SVG donut (r=40, viewBox="0 0 100 100") ──────────────────────────────
    private static double DonutCirc => 2.0 * Math.PI * 40.0;
    private static readonly System.Globalization.CultureInfo _inv = System.Globalization.CultureInfo.InvariantCulture;

    private string DonutDash(double pct)
    {
        var arc = pct / 100.0 * DonutCirc;
        return arc.ToString("F2", _inv) + " " + DonutCirc.ToString("F2", _inv);
    }
    private string DonutOffset(double pct)
    {
        var arc = pct / 100.0 * DonutCirc;
        return (DonutCirc - arc).ToString("F2", _inv);
    }

    // ── Course list progress shorthand ───────────────────────────────────────
    private int CourseProgressPct(Guid courseId)
        => _progressMap.TryGetValue(courseId, out var p) ? (int)Math.Round(p.ProgressPercent) : 0;

    private string CourseProgressColor(Guid courseId)
    {
        if (!_progressMap.TryGetValue(courseId, out var p)) return "#1a73e8";
        return p.Status == CourseProgressStatus.Completed ? "#1e8e3e" : "#1a73e8";
    }

    // ────────────────────────────────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        if (!CurrentUser.IsInRole(LmsRoles.Student))
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadEnrolledCoursesAsync();
    }

    private async Task LoadEnrolledCoursesAsync()
    {
        try
        {
            _isLoadingCourses = true;

            var enrollments = await EnrollmentAppService.GetMyEnrollmentsAsync(
                new GetMyEnrollmentsInput { Status = EnrollmentStatus.Active });

            if (enrollments.Count == 0)
            {
                _isLoadingCourses = false;
                return;
            }

            var enrolledIds = enrollments.Select(e => e.CourseId).ToHashSet();
            var allCourses  = await CourseCatalogAppService.GetPublishedCoursesByTenantAsync();

            _enrolledCourses = allCourses
                .Where(c => enrolledIds.Contains(c.CourseId))
                .ToList();

            // Load progress for each enrolled course
            var progressTasks = _enrolledCourses
                .Select(c => LearningProgressAppService.GetMyCourseProgressAsync(c.CourseId));

            var progressResults = await Task.WhenAll(progressTasks);

            _progressMap = _enrolledCourses
                .Zip(progressResults, (course, progress) => (course.CourseId, progress))
                .ToDictionary(x => x.CourseId, x => x.progress);
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
        _activeTab      = StudentReportTab.Overview;
        _studentProgress = null;

        await LoadStudentProgressAsync();
    }

    private void ClearSelection()
    {
        _selectedCourse  = null;
        _studentProgress = null;
    }

    private async Task SetTab(StudentReportTab tab)
    {
        _activeTab = tab;

        if (_studentProgress == null)
            await LoadStudentProgressAsync();
    }

    private async Task LoadStudentProgressAsync()
    {
        if (_selectedCourse == null) return;

        try
        {
            _isLoadingReport = true;
            StateHasChanged();

            _studentProgress = await ReportingAppService.GetStudentCourseProgressAsync(_selectedCourse.CourseId);
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

public enum StudentReportTab
{
    Overview,
    Assignments,
    Quizzes
}
