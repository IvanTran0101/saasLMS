using System;
using Volo.Abp.EventBus;

namespace saasLMS.EnrollmentService.Etos.Enrollments;

[EventName("saaslms.enrollment.student_enrolled.v1")]
public sealed class StudentEnrolledEto : EnrollmentEtoBase
{
    public Guid EnrollmentId { get; set; }

    public Guid CourseId { get; set; }

    public Guid StudentId { get; set; }

    public DateTime EnrolledAt { get; set; }
}