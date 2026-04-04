using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;

public class UpdateLessonInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public LessonContentState ContentState { get; set; }
}