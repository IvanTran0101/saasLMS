using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Instructor.Dashboard;

[Authorize]
public partial class InstructorDashboardPage : AbpComponentBase
{
    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;

    private bool _isLoadingCourses = true;
    private bool _isLoadingStats = true;

    private int _totalStudents;
    private int _totalCourses;

    private List<CourseListItemDto> _courses = new();

    protected override async Task OnInitializedAsync()
    {
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
            _courses = await CourseCatalogAppService.GetCoursesByInstructorAsync(instructorId);
            _totalCourses = _courses.Count;
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
            var countTasks = _courses
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
}