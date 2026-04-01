using System;

namespace saasLMS.EnrollmentService.Etos.Enrollments;

public sealed class StudentEnrolledEto : EnrollmentEtoBase
{
    public Guid EnrollmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid StudentId { get; set; }

    public DateTime EnrolledAt { get; set; }
}