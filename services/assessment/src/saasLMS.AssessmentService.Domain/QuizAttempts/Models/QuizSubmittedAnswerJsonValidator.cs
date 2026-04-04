using System;
using System.Collections.Generic;
using System.Linq;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using System.Text.Json;


namespace saasLMS.AssessmentService.QuizAttempts.Models;

public static class QuizSubmittedAnswerJsonValidator
{
    public static IReadOnlyList<QuizSubmittedAnswerJsonModel> ValidateAndParse(string submittedAnswersJson)
    {
        if (string.IsNullOrWhiteSpace(submittedAnswersJson))
        {
            throw new ArgumentException("Submitted answers json cannot be empty");
        }

        List<QuizSubmittedAnswerJsonModel>? submittedAnswers;

        try
        {
            submittedAnswers = JsonSerializer.Deserialize<List<QuizSubmittedAnswerJsonModel>>(
                submittedAnswersJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Submitted answers json is not a valid JSON format.", nameof(submittedAnswersJson), ex);
        }

        if (submittedAnswers == null || submittedAnswers.Count == 0)
        {
            throw new InvalidOperationException("Submitted answers must contain at least one item.");
        }
        
        if (submittedAnswers.Any(x => x.QuestionIndex < 0))
        {
            throw new InvalidOperationException("Question index cannot be negative.");
        }
        if (submittedAnswers.Any(x=> x.SelectedAnswerIndex < 0))
        {
            throw new InvalidOperationException("Selected answer index cannot be negative.");
        }
        var duplicateQuestionIndexes = submittedAnswers
            .GroupBy(x=> x.QuestionIndex)
            .Where(g=> g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicateQuestionIndexes.Count != 0)
        {
            throw new InvalidOperationException("A question can only have one selected answer index.");
        }
        return submittedAnswers;
    }
}