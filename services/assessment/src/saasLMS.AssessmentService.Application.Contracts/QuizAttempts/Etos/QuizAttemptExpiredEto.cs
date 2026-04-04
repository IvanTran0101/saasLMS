using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.QuizAttempts.Etos;

[EventName("lms.assessment.quizattemptexpired.v1")]
public class QuizAttemptExpiredEto : IntegrationEventEtoBase
{
    public Guid QuizAttemptId { get; set; }
    public Guid QuizId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime ExpiredAt { get; set; }
}
