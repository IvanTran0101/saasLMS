using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Quizzes;

public class QuizListItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public decimal MaxScore { get; set; }
    public AttemptPolicy AttemptPolicy { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}