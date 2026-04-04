using System;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Submissions.Etos;

[EventName("lms.assessment.submissionupdated.v1")]
public class SubmissionUpdatedEto : IntegrationEventEtoBase
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public ContentType ContentType { get; set; }
    public DateTime SubmittedAt { get; set; }
}
