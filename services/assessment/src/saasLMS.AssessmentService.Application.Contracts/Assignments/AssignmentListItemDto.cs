using System;

namespace saasLMS.AssessmentService.Assignments;

public class AssignmentListItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public decimal MaxScore { get; set; }
    public AssignmentStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}