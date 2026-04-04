using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Outputs;

public class MaterialInLessonDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public MaterialType Type { get; set; }
    public MaterialStatus Status { get; set; }
    public int SortOrder { get; set; }
    public string? StorageKey { get; set; } = string.Empty;
    public string? FileName { get; set; } = string.Empty;
    public string? MimeType { get; set; } = string.Empty;
    public long? FileSize { get; set; }
    public string? ExternalUrl { get; set; } = string.Empty;
    public string? Content { get; set; } = string.Empty;
    public TextFormat? Format { get; set; }
}
