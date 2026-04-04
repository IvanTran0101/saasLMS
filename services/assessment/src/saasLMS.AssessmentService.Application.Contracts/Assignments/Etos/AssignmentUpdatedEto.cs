using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Assignments.Etos;

[EventName("lms.assessment.assignmentupdated.v1")]
public class AssignmentUpdatedEto : IntegrationEventEtoBase
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public decimal MaxScore { get; set; }
    public DateTime UpdatedAt { get; set; }
}
