using System;
using System.Collections.Generic;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;

namespace saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public LessonContentState ContentState { get; set; }
    public List<MaterialDto> Materials { get; set; } = new();
}
