namespace saasLMS.EnrollmentService.Enrollments.Events;

public sealed class StudentEnrolledDomainEvent
{
    public Enrollment Enrollment { get; }

    public StudentEnrolledDomainEvent(Enrollment enrollment)
    {
        Enrollment = enrollment;
    }
}