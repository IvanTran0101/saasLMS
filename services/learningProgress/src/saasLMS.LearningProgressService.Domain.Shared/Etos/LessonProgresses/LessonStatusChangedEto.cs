using System;
using saasLMS.LearningProgressService.LessonProgresses;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

public class LessonStatusChangedEto : LessonProgressEtoBase
{
    public LessonProgressStatus From      { get; init; }
    public LessonProgressStatus To        { get; init; }
    public DateTime             ChangedAt { get; init; }
}