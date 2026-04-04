using System;
using saasLMS.CourseCatalogService.Courses;

using Volo.Abp.EventBus;
namespace saasLMS.CourseCatalogService.Etos.Materials;

[EventName("lms.coursecatalog.materialupdated.v1")]
public class MaterialUpdatedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string Title { get; set; } = string.Empty;
    public MaterialType Type { get; set; }
    public MaterialStatus Status { get; set; }
    public int SortOrder { get; set; }
    public string? FileName { get; set; }
    public string? MimeType { get; set; }
    public long? FileSize { get; set; }
    public string? ExternalUrl { get; set; }
    public string? TextContent { get; set; }
    public TextFormat? TextFormat { get; set; }
}