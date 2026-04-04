using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace saasLMS.LearningProgressService.CourseProgresses;

public class CourseProgress : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid CourseId { get; protected set; }
    public Guid StudentId { get; protected set; }
    public CourseProgressStatus Status { get; protected set; }
    public int CompletedLessonsCount { get; protected set; }
    public int TotalLessonsCount { get; protected set; }
    public decimal ProgressPercent { get; protected set; }
    public DateTime? StartedAt { get; protected set; }
    public DateTime? CompletedAt { get; protected set; }
    public DateTime? LastAccessedAt { get; protected set; }
    public Guid? LastAccessedLessonId { get; protected set; }
    
    protected CourseProgress()
    {
        // For ORM
    }

    public CourseProgress(Guid id, Guid tenantId, Guid courseId, Guid studentId, int totalLessonsCount) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("The tenant id cannot be empty.", nameof(tenantId));
        }

        if (courseId == Guid.Empty)
        {
            throw new ArgumentException("The course id cannot be empty.", nameof(courseId));
        }

        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("The student id cannot be empty.", nameof(studentId));
        }

        if (totalLessonsCount < 0)
        {
            throw new ArgumentException("The total lessons count cannot be negative.", nameof(totalLessonsCount));
        }
        
        TenantId = tenantId;
        CourseId = courseId;
        StudentId = studentId;
        TotalLessonsCount = totalLessonsCount;
        CompletedLessonsCount = 0;
        ProgressPercent = 0;
        Status = CourseProgressStatus.NotStarted;
    }

    public void MarkAsStarted(DateTime startedAt)
    {
        if (Status == CourseProgressStatus.NotStarted)
        {
            Status = CourseProgressStatus.InProgress;
            StartedAt = startedAt;
        }
        LastAccessedAt = startedAt;
    }
    

    public void UpdateCompletedLessons(int completedLessonsCount,  DateTime updatedAt)
    {
        if (completedLessonsCount < 0)
        {
            throw new ArgumentException("The completed lessons count cannot be negative.", nameof(completedLessonsCount));
        }

        if (completedLessonsCount > TotalLessonsCount)
        {
            throw new BusinessException("Completed lessons count cannot exceed total lessons count.");
        }
        CompletedLessonsCount = completedLessonsCount;
        LastAccessedAt = updatedAt;
        RecalculateProgress();
        if (CompletedLessonsCount > 0 && Status == CourseProgressStatus.NotStarted)
        {
            Status = CourseProgressStatus.InProgress;
            StartedAt ??= updatedAt;
        }

        if (CompletedLessonsCount == TotalLessonsCount  && TotalLessonsCount > 0)
        {
            Status = CourseProgressStatus.Completed;
            CompletedAt = updatedAt;
        }
        else if (CompletedLessonsCount < TotalLessonsCount)
        {
            Status = CompletedLessonsCount == 0
                ? CourseProgressStatus.NotStarted
                : CourseProgressStatus.InProgress;

            CompletedAt = null;
        }
        
    }

    public void UpdateTotalLessonsCount(int totalLessonsCount, DateTime updatedAt)
    {
        if (totalLessonsCount < 0)
        {
            throw new ArgumentException("The total lessons count cannot be negative.", nameof(totalLessonsCount));
        }

        if (CompletedLessonsCount > totalLessonsCount )
        {
            throw new BusinessException("Total lessons count cannot be less than completed lessons count.");
        }
        TotalLessonsCount = totalLessonsCount;
        LastAccessedAt = updatedAt;
        RecalculateProgress();
        if (CompletedLessonsCount == TotalLessonsCount && TotalLessonsCount > 0)
        {
            Status = CourseProgressStatus.Completed;
            CompletedAt = updatedAt;
        }
        else if (CompletedLessonsCount < TotalLessonsCount && CompletedLessonsCount > 0)
        {
            Status = CourseProgressStatus.InProgress;
            CompletedAt = null;
        }
        else
        {
            Status = CompletedLessonsCount == 0
                ? CourseProgressStatus.NotStarted
                : CourseProgressStatus.InProgress;
            CompletedAt = null;
        }
    }

    private void RecalculateProgress()
    {
        if (TotalLessonsCount == 0)
        {
            ProgressPercent = 0;
            return;
        }
        ProgressPercent = Math.Round((decimal)CompletedLessonsCount / TotalLessonsCount * 100, 2);
    }

    public void UpdateLastAccess(Guid lessonId, DateTime lastAccessAt)
    {
        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("The lesson id cannot be empty.", nameof(lessonId));
        }
        LastAccessedAt = lastAccessAt;
        LastAccessedLessonId = lessonId;
        if (Status == CourseProgressStatus.NotStarted)
        {
            Status = CourseProgressStatus.InProgress;
            StartedAt = lastAccessAt;
        }
    }
}