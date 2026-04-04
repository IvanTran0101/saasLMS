using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Etos.Materials;

public class MaterialRenamedEto : CourseCatalogEtoBase
{
   
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string Title { get; set; }
    public MaterialType Type { get; set; }
    public MaterialStatus Status { get; set; }
    public int SortOrder { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
    public string? ExternalUrl { get; set; }
    public TextFormat? TextFormat { get; set; }
}