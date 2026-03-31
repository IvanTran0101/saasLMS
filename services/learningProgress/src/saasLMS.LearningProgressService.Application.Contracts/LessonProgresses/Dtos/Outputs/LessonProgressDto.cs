using System;

namespace saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;

public class LessonProgressDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public Guid StudentId { get; set; }
    public LessonProgressStatus Status { get; set; }
    public DateTime? FirstViewedAt { get; set; }
    public DateTime? LastViewedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}