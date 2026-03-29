using System;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Outputs;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } =  string.Empty;
    public string? Description { get; set; }
    public Guid InstructorId { get; set; }
    public CourseStatus Status { get; set; }
}