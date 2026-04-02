using System;
using saasLMS.LearningProgressService.LessonProgresses;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

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