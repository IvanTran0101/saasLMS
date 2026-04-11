using System;
using Volo.Abp.EventBus;

namespace saasLMS.NotificationService.Etos.Assessments.Submissions;

[EventName("lms.assessment.submissiongraded.v1")]
public class SubmissionGradedEto : IntegrationEventEtoBase
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public decimal Score { get; set; }
    public DateTime GradedAt { get; set; }
}