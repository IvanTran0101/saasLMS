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
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Dashboard;

[Authorize]
public partial class StudentDashboardPage : AbpComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;

    [Inject]
    private ILearningProgressAppService LearningProgressAppService { get; set; } = default!;

    // Loading states
    private bool _isLoadingEnrollments = true;
    private bool _isLoadingCourses = true;
    private bool _isLoadingProgress = true;

    // Raw data
    private List<EnrollmentListItemDto> _myEnrollments = new();
    private List<CourseListItemDto> _allTenantCourses = new();
    private Dictionary<Guid, CourseProgressDto> _progressMap = new();
    private Dictionary<Guid, ResumeResultDto> _resumeMap = new();

    // Derived lists (all, before search filter)
    private List<CourseListItemDto> _enrolledCourses = new();
    private List<CourseListItemDto> _otherCourses = new();
    private List<CourseListItemDto> _recentlyAccessed = new();

    // Filtered lists (after search)
    private List<CourseListItemDto> _filteredEnrolledCourses = new();
    private List<CourseListItemDto> _filteredOtherCourses = new();

    // Stats
    private int _totalEnrolled;
    private int _completedCount;

    // Search
    private string _searchText = string.Empty;
    private string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            ApplySearch();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        if (!CurrentUser.IsInRole(LmsRoles.Student))
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        // Step 1: load enrollments + tenant courses in parallel
        await Task.WhenAll(LoadEnrollmentsAsync(), LoadTenantCoursesAsync());

        // Step 2: build course lists so we know which courses are enrolled
        BuildCourseLists();

        // Step 3: load progress + resume positions for enrolled courses in parallel
        await LoadProgressAndResumeAsync();

        // Step 4: apply search + derived stats
        ApplySearch();
        ComputeStats();
    }

    private async Task LoadEnrollmentsAsync()
    {
        try
        {
            _isLoadingEnrollments = true;
            _myEnrollments = await EnrollmentAppService.GetMyEnrollmentsAsync(
                new GetMyEnrollmentsInput { Status = EnrollmentStatus.Active });
            _totalEnrolled = _myEnrollments.Count;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingEnrollments = false;
        }
    }

    private async Task LoadTenantCoursesAsync()
    {
        try
        {
            _isLoadingCourses = true;
            _allTenantCourses = await CourseCatalogAppService.GetPublishedCoursesByTenantAsync();
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

    private void BuildCourseLists()
    {
        var enrolledIds = _myEnrollments.Select(e => e.CourseId).ToHashSet();

        _enrolledCourses = _allTenantCourses
            .Where(c => enrolledIds.Contains(c.CourseId))
            .ToList();

        _otherCourses = _allTenantCourses
            .Where(c => !enrolledIds.Contains(c.CourseId))
            .ToList();
    }

    private async Task LoadProgressAndResumeAsync()
    {
        if (_enrolledCourses.Count == 0)
        {
            _isLoadingProgress = false;
            return;
        }

        try
        {
            _isLoadingProgress = true;

            // Run progress + resume fetches for all enrolled courses in parallel
            var progressTasks = _enrolledCourses
                .Select(c => LearningProgressAppService.GetMyCourseProgressAsync(c.CourseId));

            var resumeTasks = _enrolledCourses
                .Select(c => LearningProgressAppService.GetResumePositionAsync(c.CourseId));

            var progressResults = await Task.WhenAll(progressTasks);
            var resumeResults   = await Task.WhenAll(resumeTasks);

            _progressMap = _enrolledCourses
                .Zip(progressResults, (course, progress) => (course.CourseId, progress))
                .ToDictionary(x => x.CourseId, x => x.progress);

            _resumeMap = _enrolledCourses
                .Zip(resumeResults, (course, resume) => (course.CourseId, resume))
                .ToDictionary(x => x.CourseId, x => x.resume);

            // Recently Accessed: enrolled courses with a LastAccessedAt, sorted desc, top 4
            _recentlyAccessed = _enrolledCourses
                .Where(c => _progressMap.TryGetValue(c.CourseId, out var p) && p.LastAccessedAt.HasValue)
                .OrderByDescending(c => _progressMap[c.CourseId].LastAccessedAt)
                .Take(4)
                .ToList();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingProgress = false;
        }
    }

    private void ApplySearch()
    {
        var term = _searchText.Trim();

        if (string.IsNullOrEmpty(term))
        {
            _filteredEnrolledCourses = _enrolledCourses;
            _filteredOtherCourses    = _otherCourses;
            return;
        }

        var termLower = term.ToLowerInvariant();

        _filteredEnrolledCourses = _enrolledCourses
            .Where(c => c.Title.Contains(termLower, StringComparison.OrdinalIgnoreCase))
            .ToList();

        _filteredOtherCourses = _otherCourses
            .Where(c => c.Title.Contains(termLower, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void ComputeStats()
    {
        _completedCount = _progressMap.Values
            .Count(p => p.Status == CourseProgressStatus.Completed);
    }

}
