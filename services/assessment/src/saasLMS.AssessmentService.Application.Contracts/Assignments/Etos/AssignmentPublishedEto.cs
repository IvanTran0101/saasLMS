using System;

namespace saasLMS.AssessmentService.Assignments.Etos;

public class AssignmentPublishedEto : IntegrationEventEtoBase
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime PublishedAt { get; set; }
}