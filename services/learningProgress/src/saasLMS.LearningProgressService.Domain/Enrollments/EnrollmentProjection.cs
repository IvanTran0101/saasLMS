using System;
using Volo.Abp.Domain.Entities;

namespace saasLMS.LearningProgressService.Enrollments;

public class EnrollmentProjection : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid EnrollmentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid StudentId { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EnrolledAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    protected EnrollmentProjection()
    {
    }

    public EnrollmentProjection(
        Guid id,
        Guid tenantId,
        Guid enrollmentId,
        Guid courseId,
        Guid studentId,
        DateTime enrolledAt,
        bool isActive = true)
        : base(id)
    {
        TenantId = tenantId;
        EnrollmentId = enrollmentId;
        CourseId = courseId;
        StudentId = studentId;
        EnrolledAt = enrolledAt;
        IsActive = isActive;
        CancelledAt = null;
    }

    public void Activate(Guid enrollmentId, DateTime enrolledAt)
    {
        EnrollmentId = enrollmentId;
        EnrolledAt = enrolledAt;
        IsActive = true;
        CancelledAt = null;
    }

    public void Deactivate(DateTime cancelledAt)
    {
        IsActive = false;
        CancelledAt = cancelledAt;
    }
}
