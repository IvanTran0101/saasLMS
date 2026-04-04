using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Events;

public class LastLearningPositionUpdatedDomainEvent
{
    public Guid                 TenantId  { get; }
    public Guid                 CourseId  { get; }
    public Guid                 LessonId  { get; }
    public Guid                 StudentId { get; }
    public LessonProgressStatus LessonStatus    { get; }
    public DateTime             UpdatedAt { get; }
 
    public LastLearningPositionUpdatedDomainEvent(
        Guid                 tenantId,
        Guid                 courseId,
        Guid                 lessonId,
        Guid                 studentId,
        LessonProgressStatus lessonStatus,
        DateTime             updatedAt)
    {
        TenantId  = tenantId;
        CourseId  = courseId;
        LessonId  = lessonId;
        StudentId = studentId;
        LessonStatus    = lessonStatus;
        UpdatedAt = updatedAt;
    }
}