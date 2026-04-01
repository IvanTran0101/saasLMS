using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace saasLMS.AssessmentService.Quizzes.Models;

public class QuizQuestion
{
    private readonly List<QuizAnswer> _answers = new();
    public string Text { get; private set; }
    public IReadOnlyCollection<QuizAnswer> Answers => new ReadOnlyCollection<QuizAnswer>(_answers);
    protected QuizQuestion()
    {
        Text = string.Empty;
    }

    public QuizQuestion(string text, IEnumerable<QuizAnswer> answers)
    {
        SetText(text);
        SetAnswers(answers);
        ValidateInvariants();
    }

    public void UpdateText(string text)
    {
        SetText(text);
    }
    
    public void ReplaceAnswers(IEnumerable<QuizAnswer> answers)
    {
        SetAnswers(answers);
        ValidateInvariants();
    }

    public void AddAnswer(string text, bool isCorrect)
    {
        _answers.Add(new QuizAnswer(text, isCorrect));
        ValidateInvariants();
    }

    public void RemoveAnswerAt(int index)
    {
        if (index < 0 || index >= _answers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        _answers.RemoveAt(index);
        ValidateInvariants();
    }

    public void MarkCorrectAnswer(int index)
    {
        if (index < 0 || index >= _answers.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        for (var i = 0; i < _answers.Count; i++)
        {
            if (i == index)
            {
                _answers[i].MarkAsCorrect();
            }
            else
            {
                _answers[i].MarkAsIncorrect();
            }
        }
        ValidateInvariants();
    }

    private void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));
        }
        Text = text.Trim();
    }

    private void SetAnswers(IEnumerable<QuizAnswer> answers)
    {
        if (answers == null)
        {
            throw new ArgumentNullException(nameof(answers));
        }
        _answers.Clear();
        _answers.AddRange(answers);
        if (_answers.Any(answer => answer == null))
        {
            throw new ArgumentException("Answers cannot contain null items.", nameof(answers));
        }
    }
    private void ValidateInvariants()
    {
        if (_answers.Count < 2)
        {
            throw new InvalidOperationException("Quiz question must have at least two answers.");
        }
        var correctAnswerCount = _answers.Count(x => x.IsCorrect);
        if (correctAnswerCount == 0)
        {
            throw new InvalidOperationException("A question must have exactly one correct answer.");
        }

        if (correctAnswerCount > 1)
        {
            throw new InvalidOperationException("A question must have exactly one correct answer.");
        }
    }

    
}