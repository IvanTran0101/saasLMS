using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.QuizAttempts.Etos;

[EventName("lms.assessment.quizattemptcompleted.v1")]
public class QuizAttemptCompletedEto : IntegrationEventEtoBase
{
    public Guid QuizAttemptId { get; set; }
    public Guid QuizId { get; set; }
    public Guid StudentId { get; set; }
    public decimal Score { get; set; }
    public DateTime CompletedAt { get; set; }
    public string CompletionMode { get; set; } = string.Empty; // "Manual" | "Timeout"
}
