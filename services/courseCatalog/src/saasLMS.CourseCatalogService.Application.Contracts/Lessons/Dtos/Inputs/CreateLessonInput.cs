using System;

namespace saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;

public class CreateLessonInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } =  string.Empty;
}