using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Etos.Materials;

public class MaterialTextFormatUpdatedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string Title { get; set; }
    public MaterialType Type { get; set; }
    public MaterialStatus Status { get; set; }
    public int SortOrder { get; set; }
    public string? TextContent { get; set; }
    public TextFormat? TextFormat { get; set; }
}