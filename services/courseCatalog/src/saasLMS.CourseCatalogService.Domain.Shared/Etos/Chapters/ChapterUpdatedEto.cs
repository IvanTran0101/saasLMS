using System;

namespace saasLMS.CourseCatalogService.Etos.Chapters;

public class ChapterUpdatedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid CHapterId { get; set; }
    public string Title { get; set; }
    public int OrderNo { get; set; }
}