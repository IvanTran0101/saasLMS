using System;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class CreateFileMaterialInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSize { get; set; }
}