using System;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Inputs;

public class CreateCourseInput
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
