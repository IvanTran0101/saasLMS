using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Assignments.Etos;

[EventName("lms.assessment.assignmentpublished.v1")]
public class AssignmentPublishedEto : IntegrationEventEtoBase
{
    public Guid AssignmentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime PublishedAt { get; set; }
}
