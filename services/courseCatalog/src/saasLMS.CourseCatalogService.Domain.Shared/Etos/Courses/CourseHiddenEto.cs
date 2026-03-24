using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Etos.Courses;

public class CourseHiddenEto : CourseCatalogEtoBase
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public CourseStatus Status { get; set; }
}