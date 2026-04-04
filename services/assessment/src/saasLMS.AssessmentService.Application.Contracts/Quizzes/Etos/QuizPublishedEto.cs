using System;
using Volo.Abp.EventBus;

namespace saasLMS.AssessmentService.Quizzes.Etos;

[EventName("lms.assessment.quizpublished.v1")]
public class QuizPublishedEto : IntegrationEventEtoBase
{
    public Guid QuizId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime PublishedAt { get; set; }
}
