using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;

namespace saasLMS.Blazor.Client.Components.Shared;

public partial class InstructorCourseCard : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter, EditorRequired]
    public CourseListItemDto Course { get; set; } = default!;  // ✅ Đổi từ CourseDto → CourseListItemDto

    private string _statusCssClass => Course.Status switch
    {
        CourseStatus.Published => "published",
        CourseStatus.Hidden    => "hidden",
        _                      => "draft"
    };

    private string _statusLabel => Course.Status switch
    {
        CourseStatus.Published => "Published",
        CourseStatus.Hidden    => "Hidden",
        _                      => "Drafting"
    };

    private void NavigateToCourse()
    {
        NavigationManager.NavigateTo($"/courses/{Course.CourseId}/edit"); // ✅ CourseListItemDto dùng CourseId, không phải Id
    }
}