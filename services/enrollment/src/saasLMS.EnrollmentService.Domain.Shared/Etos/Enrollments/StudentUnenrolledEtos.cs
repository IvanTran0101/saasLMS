using System;
using Volo.Abp.EventBus;

namespace saasLMS.EnrollmentService.Etos.Enrollments;

[EventName("saaslms.enrollment.student_unenrolled.v1")]
public sealed class StudentUnenrolledEto : EnrollmentEtoBase
{
    public Guid EnrollmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime CancelledAt { get; set; }
}