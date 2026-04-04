using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.QuizAttempts.Etos;

[EventName("lms.assessment.quizattemptstarted.v1")]
public class QuizAttemptStartedEto : IntegrationEventEtoBase
{
    public Guid QuizAttemptId { get; set; }
    public Guid QuizId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime StartedAt { get; set; }
}
