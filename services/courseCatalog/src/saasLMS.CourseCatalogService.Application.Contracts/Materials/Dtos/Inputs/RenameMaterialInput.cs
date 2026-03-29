using System;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class RenameMaterialInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string NewTitle { get; set; } = string.Empty;
    
}