using System;

namespace saasLMS.AssessmentService.Submissions.Etos;

public class SubmissionGradedEto : IntegrationEventEtoBase
{
    public Guid SubmissionId { get; set; }
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public decimal Score { get; set; }
    public DateTime GradedAt { get; set; }
}