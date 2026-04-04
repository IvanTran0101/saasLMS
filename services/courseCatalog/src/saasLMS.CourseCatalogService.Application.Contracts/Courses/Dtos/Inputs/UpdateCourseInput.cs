using System;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Inputs;

public class UpdateCourseInput
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    
}