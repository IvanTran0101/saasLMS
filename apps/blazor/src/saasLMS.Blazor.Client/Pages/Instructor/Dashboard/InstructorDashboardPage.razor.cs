using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Instructor.Dashboard;

[Authorize]
public partial class InstructorDashboardPage : AbpComponentBase
{
    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    private bool _isLoadingStats = true;
    private bool _isLoadingCourses = true;

    private int _totalStudents;
    private int _totalCourses;

    private List<CourseDto> _courses = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadCoursesAsync();
        await LoadStatsAsync();
    }

    private async Task LoadCoursesAsync()
    {
        try
        {
            _isLoadingCourses = true;

            var result = await CourseCatalogAppService.GetMyCourseListAsync(new GetCourseListInput
            {
                MaxResultCount = 9
            });

            _courses = result.Items.ToList();
            _totalCourses = (int)result.TotalCount;
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

            // TODO: Replace with a dedicated aggregate stats query when available
            _totalStudents = _courses.Sum(c => c.EnrolledStudentCount);
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