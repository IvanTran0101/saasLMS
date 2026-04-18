using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;

namespace saasLMS.Blazor.Client.Components.Shared;

public partial class StudentCourseCard : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Parameter, EditorRequired]
    public CourseListItemDto Course { get; set; } = default!;

    /// <summary>Non-null when the student is enrolled in this course.</summary>
    [Parameter]
    public CourseProgressDto? Progress { get; set; }

    /// <summary>True when the student is enrolled; false for "Other Courses" cards.</summary>
    [Parameter]
    public bool IsEnrolled { get; set; }

    /// <summary>Last-accessed lesson to resume from. Used for navigation when enrolled.</summary>
    [Parameter]
    public Guid? ResumeLessonId { get; set; }

    /// <summary>Instructor display name, used on non-enrolled cards.</summary>
    [Parameter]
    public string InstructorName { get; set; } = string.Empty;

    /// <summary>Invoked when the student clicks Enroll on a non-enrolled card.</summary>
    [Parameter]
    public EventCallback<Guid> OnEnroll { get; set; }

    private bool _enrolling;

    private int _progressPct => Progress is null ? 0 : (int)Math.Round(Progress.ProgressPercent);

    private string _progressCssClass => Progress?.Status switch
    {
        CourseProgressStatus.Completed  => "completed",
        CourseProgressStatus.InProgress => "inprogress",
        _                               => "notstarted"
    };

    private string _progressLabel => Progress?.Status switch
    {
        CourseProgressStatus.Completed  => "Completed",
        CourseProgressStatus.InProgress => "In Progress",
        _                               => "Not Started"
    };

    private void HandleCardClick()
    {
        if (!IsEnrolled) return; // non-enrolled cards use the Enroll button

        if (ResumeLessonId.HasValue)
            NavigationManager.NavigateTo($"/student/learn/{Course.CourseId}/{ResumeLessonId.Value}");
        else
            NavigationManager.NavigateTo($"/student/learn/{Course.CourseId}");
    }

    private async Task HandleEnrollClickAsync()
    {
        if (_enrolling) return;
        _enrolling = true;
        try
        {
            await OnEnroll.InvokeAsync(Course.CourseId);
        }
        finally
        {
            _enrolling = false;
        }
    }
}
