namespace saasLMS.EnrollmentService.Enrollments.Events;

public sealed class StudentEnrollmentCompletedDomainEvent
{
    public Enrollment Enrollment { get; }

    public StudentEnrollmentCompletedDomainEvent(Enrollment enrollment)
    {
        Enrollment = enrollment;
    }
}