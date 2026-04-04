using System;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Submissions.Etos;

[EventName("lms.assessment.submissioncreated.v1")]
public class SubmissionCreatedEto : IntegrationEventEtoBase
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public ContentType ContentType { get; set; }
    public DateTime SubmittedAt { get; set; }
}
