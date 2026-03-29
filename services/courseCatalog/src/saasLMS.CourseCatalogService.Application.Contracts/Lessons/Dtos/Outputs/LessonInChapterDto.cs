using System;
using System.Collections.Generic;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;

namespace saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;

public class LessonInChapterDto
{
    public Guid Id { get; set; }
    public Guid ChapterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public LessonContentState ContentState { get; set; }
    public List<MaterialInLessonDto> Materials { get; set; } = new();
}
