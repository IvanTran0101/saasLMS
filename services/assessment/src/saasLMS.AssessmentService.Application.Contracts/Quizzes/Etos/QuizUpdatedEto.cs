using System;
using Volo.Abp.EventBus;
using saasLMS.AssessmentService.Shared;

namespace saasLMS.AssessmentService.Quizzes.Etos;

[EventName("lms.assessment.quizupdated.v1")]
public class QuizUpdatedEto : IntegrationEventEtoBase
{
    public Guid QuizId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public decimal MaxScore { get; set; }
    public AttemptPolicy AttemptPolicy { get; set; }
    public DateTime UpdatedAt { get; set; }
}
