using System;

namespace saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;

public class CourseProgressDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public CourseProgressStatus Status { get; set; }
    public int CompletedLessonsCount { get; set; }
    public int TotalLessonsCount { get; set; }
    public decimal ProgressPercent { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? LastAccessedAt { get; set; }
    public Guid? LastAccessedLessonId { get; set; }
}