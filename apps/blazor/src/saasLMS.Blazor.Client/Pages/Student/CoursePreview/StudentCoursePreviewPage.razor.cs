using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.EnrollmentService.Enrollments;
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.CoursePreview;

[Authorize]
public partial class StudentCoursePreviewPage : AbpComponentBase
{
    [Parameter]
    public Guid CourseId { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IEnrollmentAppService EnrollmentAppService { get; set; } = default!;

    private bool _isLoading = true;
    private bool _enrolling;
    private bool _isEnrolled;

    private CourseDetailDto? _course;
    private string _instructorName = "Instructor";
    private int _totalLessons;

    protected override async Task OnInitializedAsync()
    {
        if (!CurrentUser.IsAuthenticated)
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadCourseAsync();
    }

    private async Task LoadCourseAsync()
    {
        try
        {
            _isLoading = true;

            // Load course detail + check enrollment in parallel
            var courseTask = CourseCatalogAppService.GetCourseDetailStudentAsync(CourseId);
            var enrollmentTask = EnrollmentAppService.FindByCourseAsync(CourseId);

            await Task.WhenAll(courseTask, enrollmentTask);

            _course = courseTask.Result;
            _isEnrolled = enrollmentTask.Result?.Status == EnrollmentStatus.Active;
            _totalLessons = _course.Chapters.Sum(c => c.Lessons.Count);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task EnrollAsync()
    {
        if (_enrolling || _isEnrolled) return;
        _enrolling = true;
        try
        {
            await EnrollmentAppService.EnrollAsync(new EnrollCourseInput { CourseId = CourseId });
            _isEnrolled = true;
            // Navigate to dashboard so the student sees the course in My Learning
            NavigationManager.NavigateTo("/student/dashboard");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _enrolling = false;
        }
    }

    private void GoBack() => NavigationManager.NavigateTo("/student/dashboard");
}
