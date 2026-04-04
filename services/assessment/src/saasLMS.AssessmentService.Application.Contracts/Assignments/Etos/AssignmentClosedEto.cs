using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Assignments.Etos;

[EventName("lms.assessment.assignmentclosed.v1")]
public class AssignmentClosedEto : IntegrationEventEtoBase
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime ClosedAt { get; set; }
}
