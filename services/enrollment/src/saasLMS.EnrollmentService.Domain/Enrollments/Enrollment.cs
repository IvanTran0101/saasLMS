using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.EnrollmentService.Enrollments;

public class Enrollment : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid StudentId { get; protected set; }
    public EnrollmentStatus Status { get; protected set; }
    public DateTime EnrolledAt { get; protected set; }
    public DateTime? CompletedAt { get; protected set; }
    public DateTime? CancelledAt { get; protected set; }

    protected Enrollment()
    {
    }

    public Enrollment(Guid id, Guid tenantId, Guid courseId, Guid studentId, DateTime enrolledAt) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("Course id cannot be empty.", nameof(courseId));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id cannot be empty.", nameof(studentId));
        }
        TenantId = tenantId;
        CourseId = courseId;
        StudentId = studentId;
        EnrolledAt = enrolledAt;
        Status = EnrollmentStatus.Active;
    }
    public void Complete(DateTime completedAt)
    {
        if (Status == EnrollmentStatus.Cancelled)
        {
            throw new BusinessException("Cannot complete a cancelled enrollment.");
        }
        if (Status == EnrollmentStatus.Completed)
        {
            throw new BusinessException("Enrollment is already completed.");
        }
        
        Status = EnrollmentStatus.Completed;
        CompletedAt = completedAt;
        CancelledAt = null;
    }

    public void Cancel(DateTime cancelledAt)
    {
        
        if (Status == EnrollmentStatus.Cancelled)
        {
            throw new BusinessException("Enrollment is already cancelled.");
        }
        if (Status == EnrollmentStatus.Completed)
        {
            throw new BusinessException("Cannot cancel a completed enrollment.");
        }
        Status = EnrollmentStatus.Cancelled;
        CancelledAt = cancelledAt;
        CompletedAt = null;
    }
    
    
}