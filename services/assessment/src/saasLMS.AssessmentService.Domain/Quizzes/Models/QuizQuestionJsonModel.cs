using System.Collections.Generic;

namespace saasLMS.AssessmentService.Quizzes.Models;

public class QuizQuestionJsonModel
{
    public string Text { get; set; } = string.Empty;
    public List<QuizAnswerJsonModel> Answers { get; set; } = new();
}