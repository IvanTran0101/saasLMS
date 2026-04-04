using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Quizzes.Etos;

[EventName("lms.assessment.quizclosed.v1")]
public class QuizClosedEto : IntegrationEventEtoBase
{
    public Guid QuizId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime ClosedAt { get; set; }
}
