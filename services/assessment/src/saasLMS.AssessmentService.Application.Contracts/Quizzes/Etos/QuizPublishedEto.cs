using System;

namespace saasLMS.AssessmentService.Quizzes.Etos;

public class QuizPublishedEto : IntegrationEventEtoBase
{
    public Guid QuizId { get; set; }
    public Guid CourseId { get; set; }
    public Guid LessonId { get; set; }
    public DateTime PublishedAt { get; set; }
}