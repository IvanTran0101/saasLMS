using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Dtos.Inputs;

public class StartLessonInput
{
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public int? TotalLessonsCount { get; set; }
}