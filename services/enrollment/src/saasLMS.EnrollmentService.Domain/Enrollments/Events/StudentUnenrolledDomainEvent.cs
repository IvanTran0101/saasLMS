namespace saasLMS.EnrollmentService.Enrollments.Events;

public sealed class StudentUnenrolledDomainEvent
{
    public Enrollment Enrollment { get; }

    public StudentUnenrolledDomainEvent(Enrollment enrollment)
    {
        Enrollment = enrollment;
    }
}