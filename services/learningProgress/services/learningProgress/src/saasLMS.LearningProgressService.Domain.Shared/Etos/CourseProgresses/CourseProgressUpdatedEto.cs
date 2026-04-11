using System;
using saasLMS.LearningProgressService.CourseProgresses;
using Volo.Abp.EventBus;

namespace saasLMS.LearningProgressService.Etos.CourseProgresses;

[EventName("saaslms.learningprogress.courseprogress.updated.v1")]
public sealed class CourseProgressUpdatedEto
{
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
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
