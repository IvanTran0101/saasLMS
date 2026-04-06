using System;

using Volo.Abp.EventBus;
namespace saasLMS.CourseCatalogService.Etos.Chapters;

[EventName("lms.coursecatalog.chapterdeleted.v1")]
public class ChapterDeletedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderNo { get; set; }
}