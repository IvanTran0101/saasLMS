using System;
using System.Collections.Generic;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;

namespace saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;

public class ChapterDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int OrderNo { get; set; }
    public List<LessonInChapterDto> Lessons { get; set; } = new();
}
