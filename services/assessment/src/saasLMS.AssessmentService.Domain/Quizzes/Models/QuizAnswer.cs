using System;

namespace saasLMS.AssessmentService.Quizzes.Models;

public class QuizAnswer
{
    public string Text { get; private set; }
    public bool IsCorrect { get; private set; }
    protected QuizAnswer()
    {
        Text = string.Empty;
    }
    public QuizAnswer(string text, bool isCorrect)
    {
        SetText(text);
        IsCorrect = isCorrect;
    }


    public void UpdateText(string text)
    {
        SetText(text);
    }

    public void MarkAsCorrect()
    {
        IsCorrect = true;
    }

    public void MarkAsIncorrect()
    {
        IsCorrect = false;
    }
    
    private void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Answer text cannot be null or empty.", nameof(text));
        }

        Text = text.Trim();
    }
}