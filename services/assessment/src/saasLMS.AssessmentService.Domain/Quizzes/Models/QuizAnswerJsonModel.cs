namespace saasLMS.AssessmentService.Quizzes.Models;

public class QuizAnswerJsonModel
{
    public string Text { get; set; }  = string.Empty;
    public bool IsCorrect { get; set; }
}