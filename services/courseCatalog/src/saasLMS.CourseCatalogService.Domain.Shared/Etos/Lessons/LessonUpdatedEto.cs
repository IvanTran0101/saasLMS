using System;
using saasLMS.CourseCatalogService.Courses;

using Volo.Abp.EventBus;
namespace saasLMS.CourseCatalogService.Etos.Lessons;

[EventName("lms.coursecatalog.lessonupdated.v1")]
public class LessonUpdatedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public LessonContentState ContentState { get; set; }
}