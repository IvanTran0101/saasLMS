using System;

namespace saasLMS.AssessmentService.QuizAttempts;

public class QuizAttemptAnswerDto
{
    public Guid QuestionId { get; set; }
    public Guid? ChoiceId { get; set; }
    public string? Value { get; set; }
}
