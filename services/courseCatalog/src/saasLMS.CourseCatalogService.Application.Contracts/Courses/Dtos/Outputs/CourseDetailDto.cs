using System;
using System.Collections.Generic;
using saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Outputs;

public class CourseDetailDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public CourseStatus Status { get; set; }
    public string? Description { get; set; }
    public List<ChapterDto> Chapters { get; set; } = new();

}