using System;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.EventBus;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

[EventName("lms.learning_progress.lesson_status_changed.v1")]
public class LessonStatusChangedEto : LessonProgressEtoBase
{
    public LessonProgressStatus From      { get; init; }
    public LessonProgressStatus To        { get; init; }
    public DateTime             ChangedAt { get; init; }
}