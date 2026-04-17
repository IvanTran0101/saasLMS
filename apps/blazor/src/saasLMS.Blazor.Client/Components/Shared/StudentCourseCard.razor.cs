using System;
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
        if (IsEnrolled)
        {
            if (ResumeLessonId.HasValue)
                NavigationManager.NavigateTo($"/student/learn/{Course.CourseId}/{ResumeLessonId.Value}");
            // If no resume position yet, fall through to preview page
            else
                NavigationManager.NavigateTo($"/student/courses/{Course.CourseId}");
        }
        else
        {
            NavigationManager.NavigateTo($"/student/courses/{Course.CourseId}");
        }
    }
}
