using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Events;

public class LessonCompletedDomainEvent
{
    public LessonProgress LessonProgress { get; }
    public DateTime CompletedAt { get; }

    public LessonCompletedDomainEvent(LessonProgress lessonProgress, DateTime completedAt)
    {
        LessonProgress = lessonProgress;
        CompletedAt    = completedAt;
    }
}