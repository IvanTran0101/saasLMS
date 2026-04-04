using System;

namespace saasLMS.AssessmentService.Assignments.Etos;

public class AssignmentCreatedEto : IntegrationEventEtoBase
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public decimal MaxScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
