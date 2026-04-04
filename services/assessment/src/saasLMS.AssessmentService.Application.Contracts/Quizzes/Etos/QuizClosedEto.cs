using System;

namespace saasLMS.AssessmentService.Quizzes.Etos;

public class QuizClosedEto : IntegrationEventEtoBase
{
    public Guid QuizId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime ClosedAt { get; set; }
}