using System;
using Volo.Abp.EventBus;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

[EventName("lms.learning_progress.lesson_completed.v1")]
public class LessonCompletedEto : LessonProgressEtoBase
{
    public DateTime CompletedAt { get; init; }
}