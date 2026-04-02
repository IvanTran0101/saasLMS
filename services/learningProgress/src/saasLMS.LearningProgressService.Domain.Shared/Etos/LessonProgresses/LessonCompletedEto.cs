using System;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

public class LessonCompletedEto : LessonProgressEtoBase
{
    public DateTime CompletedAt { get; init; }
}