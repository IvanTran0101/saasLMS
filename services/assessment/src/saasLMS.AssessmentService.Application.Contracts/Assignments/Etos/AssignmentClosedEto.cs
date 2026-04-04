using System;

namespace saasLMS.AssessmentService.Assignments.Etos;

public class AssignmentClosedEto : IntegrationEventEtoBase
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime ClosedAt { get; set; }
}