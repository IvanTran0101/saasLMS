using System;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class UpdateVideoLinkMaterialInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string ExternalUrl { get; set; } = string.Empty;
}