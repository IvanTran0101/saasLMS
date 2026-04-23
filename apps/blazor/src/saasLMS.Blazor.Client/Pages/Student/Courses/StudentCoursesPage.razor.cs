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
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.Identity;

namespace saasLMS.Blazor.Client.Pages.Student.Courses;

[Authorize]
public partial class StudentCoursesPage : AbpComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;

    [Inject]
    private IIdentityUserAppService IdentityUserAppService { get; set; } = default!;

    private bool _isLoading = true;

    private List<CourseListItemDto> _allTenantCourses = new();
    private List<EnrollmentListItemDto> _myEnrollments = new();
    private List<CourseListItemDto> _availableCourses = new();
    private List<CourseListItemDto> _filteredCourses = new();
    private Dictionary<Guid, string> _instructorNameMap = new();

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

        await Task.WhenAll(LoadCoursesAsync(), LoadEnrollmentsAsync());
        BuildAvailableList();
        await LoadInstructorNamesAsync();
        ApplySearch();
    }

    private async Task LoadCoursesAsync()
    {
        try
        {
            _allTenantCourses = await CourseCatalogAppService.GetPublishedCoursesByTenantAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task LoadEnrollmentsAsync()
    {
        try
        {
            _myEnrollments = await EnrollmentAppService.GetMyEnrollmentsAsync(
                new GetMyEnrollmentsInput { Status = EnrollmentStatus.Active });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void BuildAvailableList()
    {
        var enrolledIds = _myEnrollments.Select(e => e.CourseId).ToHashSet();
        _availableCourses = _allTenantCourses
            .Where(c => !enrolledIds.Contains(c.CourseId))
            .ToList();
        _isLoading = false;
    }

    private async Task LoadInstructorNamesAsync()
    {
        var instructorIds = _availableCourses
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

    private async Task EnrollAsync(Guid courseId)
    {
        try
        {
            await EnrollmentAppService.EnrollAsync(new EnrollCourseInput { CourseId = courseId });

            var course = _availableCourses.FirstOrDefault(c => c.CourseId == courseId);
            if (course != null)
            {
                _availableCourses.Remove(course);
                ApplySearch();
                StateHasChanged();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void ApplySearch()
    {
        var term = _searchText.Trim();
        if (string.IsNullOrEmpty(term))
        {
            _filteredCourses = _availableCourses;
            return;
        }

        _filteredCourses = _availableCourses
            .Where(c => c.Title.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
