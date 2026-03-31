using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Dtos.Inputs;

public class ViewLessonInput
{
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public int? TotalLessonsCount { get; set; }
}