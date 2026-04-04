using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Etos.Lessons;

public class LessonDeletedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public LessonContentState ContentState { get; set; }
}