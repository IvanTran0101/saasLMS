using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;

public class ResumeResultDto
{
    public Guid CourseId { get; set; }
    public Guid? LessonId { get; set; }
    public LessonProgressStatus? LessonStatus { get; set; }
    public DateTime? LastViewedAt { get; set; }
}