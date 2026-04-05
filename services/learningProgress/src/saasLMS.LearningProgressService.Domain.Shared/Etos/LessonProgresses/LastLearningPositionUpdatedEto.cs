using System;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.EventBus;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

[EventName("lms.learning_progress.last_learning_position_updated.v1")]
public class LastLearningPositionUpdatedEto
{
    public Guid                 EventId    { get; init; }
    public DateTime             OccurredAt { get; init; }
    public Guid                 TenantId   { get; init; }
    public Guid                 StudentId  { get; init; }
    public Guid                 CourseId   { get; init; }
    public Guid                 LessonId   { get; init; }
    public LessonProgressStatus LessonStatus     { get; init; }
    public DateTime             UpdatedAt  { get; init; }
}