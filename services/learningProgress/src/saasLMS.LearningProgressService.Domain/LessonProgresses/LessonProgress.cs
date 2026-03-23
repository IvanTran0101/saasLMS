using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.LearningProgressService.LessonProgresses;

public class LessonProgress : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid LessonId { get; protected set; }
    public Guid StudentId { get; protected set; }
    public LessonProgressStatus Status { get; protected set; }
    public DateTime? FirstViewedAt { get; protected set; }
    public DateTime? LastViewedAt { get; protected set; }
    public DateTime? CompletedAt { get; protected set; }
    
    protected LessonProgress()
    {
    }

    public LessonProgress(Guid id, Guid tenantId, Guid courseId, Guid lessonId, Guid studentId) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.");
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("The course id cannot be empty.");
        }

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("The lesson id cannot be empty.");
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("The student id cannot be empty.");
        }
        TenantId = tenantId;
        CourseId = courseId;
        LessonId = lessonId;
        StudentId = studentId;
        Status = LessonProgressStatus.NotStarted;
    }

    public void MarkAsViewed(DateTime viewedAt)
    {
        FirstViewedAt ??= viewedAt;
        LastViewedAt = viewedAt;
        if (Status == LessonProgressStatus.NotStarted)
        {
            Status = LessonProgressStatus.InProgress;
        }
    }

    public void MarkAsCompleted(DateTime completedAt)
    {
        FirstViewedAt ??= completedAt;
        LastViewedAt = completedAt;
        CompletedAt = completedAt;
        Status = LessonProgressStatus.Completed;
    }

    public void ResetToInProgress(DateTime updatedAt)
    {
        FirstViewedAt ??= updatedAt;
        LastViewedAt = updatedAt;
        CompletedAt = null;
        Status = LessonProgressStatus.InProgress;
    }
}