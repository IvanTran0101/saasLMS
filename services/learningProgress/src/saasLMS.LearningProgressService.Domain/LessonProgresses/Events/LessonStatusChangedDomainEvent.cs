using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Events;

public class LessonStatusChangedDomainEvent
{
    public LessonProgress       LessonProgress { get; }
    public LessonProgressStatus From           { get; }
    public LessonProgressStatus To             { get; }
    public DateTime             ChangedAt      { get; }
 
    public LessonStatusChangedDomainEvent(
        LessonProgress       lessonProgress,
        LessonProgressStatus from,
        LessonProgressStatus to,
        DateTime             changedAt)
    {
        LessonProgress = lessonProgress;
        From           = from;
        To             = to;
        ChangedAt      = changedAt;
    }
}