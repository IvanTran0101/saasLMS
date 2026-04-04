using System;

namespace saasLMS.EnrollmentService.Etos.Enrollments;

public sealed class StudentUnenrolledEto : EnrollmentEtoBase
{
    public Guid EnrollmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid StudentId { get; set; }

    public DateTime CancelledAt { get; set; }
}