using System;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Submissions.Etos;

public class SubmissionCreatedEto : IntegrationEventEtoBase
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public ContentType ContentType { get; set; }
    public DateTime SubmittedAt { get; set; }
}