using System;

using Volo.Abp.EventBus;
namespace saasLMS.CourseCatalogService.Etos.Chapters;

[EventName("lms.coursecatalog.chapterupdated.v1")]
public class ChapterUpdatedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderNo { get; set; }
}