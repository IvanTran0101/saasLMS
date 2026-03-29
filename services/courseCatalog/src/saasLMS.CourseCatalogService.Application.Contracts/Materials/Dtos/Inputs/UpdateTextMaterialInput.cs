using System;
using System.Management;
using TextFormat = saasLMS.CourseCatalogService.Courses.TextFormat;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class UpdateTextMaterialInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string Content { get; set; } = string.Empty;
    public TextFormat Format { get; set; }
}