using System;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class UploadMaterialFileInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public Guid LessonId { get; set; }
    public Guid MaterialId { get; set; } // hoặc tạo mới
}