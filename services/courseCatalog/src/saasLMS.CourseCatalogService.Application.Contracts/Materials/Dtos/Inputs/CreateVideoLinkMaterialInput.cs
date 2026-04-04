using System;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class CreateVideoLinkMaterialInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ExternalUrl { get; set; } = string.Empty;
}