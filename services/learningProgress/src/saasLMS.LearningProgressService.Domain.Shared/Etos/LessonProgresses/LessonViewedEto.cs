using System;
using saasLMS.LearningProgressService.LessonProgresses;

namespace saasLMS.LearningProgressService.Etos.LessonProgresses;

public class LessonViewedEto : LessonProgressEtoBase
{
    public DateTime ViewedAt { get; init; }
}