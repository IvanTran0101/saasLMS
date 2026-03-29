using System;

namespace saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;

public class RenameLessonInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public string NewTitle { get; set; } = string.Empty;
}