using System;

namespace saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;

public class RemoveLessonInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
}