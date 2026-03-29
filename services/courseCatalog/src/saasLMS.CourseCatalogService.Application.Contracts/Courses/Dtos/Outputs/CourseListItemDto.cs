using System;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Outputs;

public class CourseListItemDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public CourseStatus Status { get; set; }
}