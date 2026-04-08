using System;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Outputs;

public class CourseEligibilityDto
{
    public Guid CourseId { get; set; }
    public Guid TenantId { get; set; }
    public CourseStatus Status { get; set; }
    public bool IsHidden { get; set; }
}
