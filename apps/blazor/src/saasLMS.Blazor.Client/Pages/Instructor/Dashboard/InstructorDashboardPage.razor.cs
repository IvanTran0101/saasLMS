using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.Blazor.Client.Components.Shared;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Instructor.Dashboard;

[Authorize(Roles = LmsRoles.Instructor)]
public partial class InstructorDashboardPage : AbpComponentBase
{
    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;
    
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    
    private CreateCourseModal _createCourseModal = default!;

    private bool _isLoadingCourses = true;
    private bool _isLoadingStats = true;

    private int _totalStudents;
    private int _totalCourses;

    private List<CourseListItemDto> _allCourses = new();
    private List<CourseListItemDto> _courses = new();

    private bool _showAllCourses;
    private const int CoursesPreviewCount = 5;

    private IReadOnlyList<CourseListItemDto> VisibleCourses =>
        (!string.IsNullOrEmpty(_searchText) || _showAllCourses)
            ? _courses
            : _courses.Take(CoursesPreviewCount).ToList();

    private bool HasMoreCourses =>
        string.IsNullOrEmpty(_searchText) && _courses.Count > CoursesPreviewCount;

    private string _searchText = string.Empty;
    private string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            _showAllCourses = false; // reset khi search thay đổi
            ApplySearch();
        }
    }

    private void ApplySearch()
    {
        var term = _searchText.Trim();
        if (string.IsNullOrEmpty(term))
        {
            _courses = _allCourses;
            return;
        }

        var termLower = term.ToLowerInvariant();
        var isGuid = Guid.TryParse(term, out var guidTerm);

        _courses = _allCourses
            .Where(c =>
                c.Title.Contains(termLower, StringComparison.OrdinalIgnoreCase) ||
                (isGuid && c.CourseId == guidTerm))
            .ToList();
    }

    protected override async Task OnInitializedAsync()
    {
        // Role guard: CurrentUser.IsInRole reads locally from claims — no remote call.
        if (!CurrentUser.IsInRole(LmsRoles.Instructor))
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        // Load courses trước, stats phụ thuộc vào danh sách courseId
        await LoadCoursesAsync();
        await LoadStatsAsync();
    }

    private async Task LoadCoursesAsync()
    {
        try
        {
            _isLoadingCourses = true;

            var instructorId = CurrentUser.Id!.Value;
            _allCourses = await CourseCatalogAppService.GetCoursesByInstructorAsync(instructorId);
            _totalCourses = _allCourses.Count;
            ApplySearch();
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

    private async Task LoadStatsAsync()
    {
        try
        {
            _isLoadingStats = true;

            // Gọi EnrollmentService để đếm số student active cho từng course,
            // chạy song song bằng Task.WhenAll để tối ưu performance
            var countTasks = _allCourses
                .Select(course => EnrollmentAppService.GetEnrollmentsByCourseAsync(course.CourseId));

            var results = await Task.WhenAll(countTasks);

            // Chỉ đếm enrollment có Status = Active
            _totalStudents = results
                .SelectMany(enrollments => enrollments)
                .Count(e => e.Status == EnrollmentStatus.Active);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoadingStats = false;
        }
    }
    
    private void OpenCreateCourseModal() => _createCourseModal.Show();

    private void OnCourseCreated(CourseDto course)
        => NavigationManager.NavigateTo($"/courses/{course.Id}/edit");
}