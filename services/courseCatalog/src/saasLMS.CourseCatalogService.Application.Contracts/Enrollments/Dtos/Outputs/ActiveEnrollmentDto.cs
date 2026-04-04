using System;

namespace saasLMS.CourseCatalogService.Enrollments.Dtos.Outputs;

public class ActiveEnrollmentDto
{
    public bool IsActive { get; set; }
    public Guid? EnrollmentId { get; set; }
    public DateTime? EnrolledAt { get; set; }
}
