using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Etos.Courses;

public class CoursePublishedEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InstructorId { get; set; }
    public CourseStatus Status { get; set; }
}