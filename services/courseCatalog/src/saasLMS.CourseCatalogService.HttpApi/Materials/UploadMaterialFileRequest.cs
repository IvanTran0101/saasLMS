using System;
using Microsoft.AspNetCore.Http;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class UploadMaterialFileRequest
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; }
    public string? Title { get; set; }
    public IFormFile? File { get; set; }
}
