using System;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.EventBus;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

[EventName("lms.learning_progress.lesson_viewed.v1")]
public class LessonViewedEto : LessonProgressEtoBase
{
    public DateTime ViewedAt { get; init; }
}