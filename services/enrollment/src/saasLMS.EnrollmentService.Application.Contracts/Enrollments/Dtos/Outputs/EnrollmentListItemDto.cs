using System;
using saasLMS.EnrollmentService.Enrollments;

namespace saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;

public class EnrollmentListItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public EnrollmentStatus Status { get; set; }
    public DateTime EnrolledAt { get; set; }
}