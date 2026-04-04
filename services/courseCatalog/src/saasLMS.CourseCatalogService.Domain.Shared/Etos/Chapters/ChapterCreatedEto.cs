using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Etos.Chapters;

public class ChapterCreatedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; }
    public int OrderNo { get; set; } 
}