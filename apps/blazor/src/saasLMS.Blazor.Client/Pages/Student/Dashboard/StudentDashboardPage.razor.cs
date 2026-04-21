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
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs; // GetMyEnrollmentsInput
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.LessonProgresses;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;
using saasLMS.NotificationService.Notifications;
using saasLMS.NotificationService.Notifications.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.Identity;

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

    [Inject]
    private IIdentityUserAppService IdentityUserAppService { get; set; } = default!;

    [Inject]
    private INotificationAppService NotificationAppService { get; set; } = default!;

    // Notification state
    private bool _showNotifications;
    private bool _isLoadingNotifications;
    private bool _isMarkingAll;
    private List<NotificationDto> _notifications = new();
    private int _unreadCount;

    // Loading states
    private bool _isLoadingEnrollments = true;
    private bool _isLoadingCourses = true;
    private bool _isLoadingProgress = true;

    // Raw data
    private List<EnrollmentListItemDto> _myEnrollments = new();
    private List<CourseListItemDto> _allTenantCourses = new();
    private Dictionary<Guid, CourseProgressDto> _progressMap = new();
    private Dictionary<Guid, ResumeResultDto> _resumeMap = new();
    private Dictionary<Guid, string> _instructorNameMap = new();

    // Derived lists (all, before search filter)
    private List<CourseListItemDto> _enrolledCourses = new();
    private List<CourseListItemDto> _recentlyAccessed = new();

    // Filtered list (after search)
    private List<CourseListItemDto> _filteredEnrolledCourses = new();

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

        // Step 1: load enrollments + tenant courses + unread count in parallel
        await Task.WhenAll(LoadEnrollmentsAsync(), LoadTenantCoursesAsync(), LoadUnreadCountAsync());

        // Step 2: build course lists so we know which courses are enrolled
        BuildCourseLists();

        // Step 3: load progress, resume positions, and instructor names in parallel
        await Task.WhenAll(LoadProgressAndResumeAsync(), LoadInstructorNamesAsync());

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

    private async Task LoadInstructorNamesAsync()
    {
        var instructorIds = _allTenantCourses
            .Select(c => c.InstructorId)
            .Distinct()
            .ToList();

        if (instructorIds.Count == 0) return;

        var tasks = instructorIds.Select(async id =>
        {
            try
            {
                var user = await IdentityUserAppService.GetAsync(id);
                var fullName = $"{user.Name} {user.Surname}".Trim();
                return (id, name: string.IsNullOrEmpty(fullName) ? user.UserName : fullName);
            }
            catch
            {
                return (id, name: "Instructor");
            }
        });

        var results = await Task.WhenAll(tasks);
        _instructorNameMap = results.ToDictionary(r => r.id, r => r.name);
    }

    private string GetInstructorName(Guid instructorId)
        => _instructorNameMap.TryGetValue(instructorId, out var name) ? name : "Instructor";

    private async Task LoadUnreadCountAsync()
    {
        try
        {
            _unreadCount = await NotificationAppService.GetUnreadCountAsync();
        }
        catch
        {
            // non-critical, swallow silently
        }
    }

    private async Task ToggleNotificationsAsync()
    {
        _showNotifications = !_showNotifications;
        if (_showNotifications && _notifications.Count == 0)
        {
            await LoadNotificationsAsync();
        }
    }

    private void CloseNotifications()
    {
        _showNotifications = false;
    }

    private async Task LoadNotificationsAsync()
    {
        try
        {
            _isLoadingNotifications = true;
            StateHasChanged();
            var summary = await NotificationAppService.GetMyNotificationsAsync();
            _notifications = summary.Items;
            _unreadCount = summary.UnreadCount;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingNotifications = false;
        }
    }

    private async Task MarkNotificationAsReadAsync(NotificationDto notif)
    {
        if (notif.IsRead) return;
        try
        {
            await NotificationAppService.MarkAsReadAsync(notif.Id);
            notif.IsRead = true;
            notif.ReadAt = DateTime.UtcNow;
            _unreadCount = Math.Max(0, _unreadCount - 1);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task MarkAllAsReadAsync()
    {
        if (_unreadCount == 0) return;
        try
        {
            _isMarkingAll = true;
            await NotificationAppService.MarkAllAsReadAsync();
            foreach (var n in _notifications)
            {
                n.IsRead = true;
                n.ReadAt ??= DateTime.UtcNow;
            }
            _unreadCount = 0;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isMarkingAll = false;
        }
    }

    private static string GetTimeAgo(DateTime creationTime)
    {
        var diff = DateTime.UtcNow - creationTime.ToUniversalTime();
        if (diff.TotalMinutes < 1) return "Just now";
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} min{((int)diff.TotalMinutes == 1 ? "" : "s")} ago";
        if (diff.TotalHours < 24) return $"{(int)diff.TotalHours} hour{((int)diff.TotalHours == 1 ? "" : "s")} ago";
        if (diff.TotalDays < 2) return "Yesterday";
        if (diff.TotalDays < 7) return $"{(int)diff.TotalDays} days ago";
        return creationTime.ToString("MMM d");
    }

    private void ApplySearch()
    {
        var term = _searchText.Trim();

        if (string.IsNullOrEmpty(term))
        {
            _filteredEnrolledCourses = _enrolledCourses;
            return;
        }

        _filteredEnrolledCourses = _enrolledCourses
            .Where(c => c.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    private void ComputeStats()
    {
        _completedCount = _progressMap.Values
            .Count(p => p.Status == CourseProgressStatus.Completed);
    }
}
