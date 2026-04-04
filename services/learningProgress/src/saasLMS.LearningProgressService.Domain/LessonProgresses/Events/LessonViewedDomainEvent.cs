using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Events;

public class LessonViewedDomainEvent
{
    public LessonProgress LessonProgress { get; }
    public DateTime       ViewedAt       { get; }
 
    public LessonViewedDomainEvent(LessonProgress lessonProgress, DateTime viewedAt)
    {
        LessonProgress = lessonProgress;
        ViewedAt       = viewedAt;
    }
}