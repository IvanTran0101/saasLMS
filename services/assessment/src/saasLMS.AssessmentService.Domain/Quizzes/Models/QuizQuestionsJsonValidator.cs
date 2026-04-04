using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace saasLMS.AssessmentService.Quizzes.Models;

public static class QuizQuestionsJsonValidator
{
    public static IReadOnlyList<QuizQuestion> ValidateAndParse(string questionsJson)
    {
        if (string.IsNullOrWhiteSpace(questionsJson))
        {
            throw new ArgumentException("QuestionsJson cannot be null or empty.", nameof(questionsJson));
        }

        List<QuizQuestionJsonModel>? rawQuestions;

        try
        {
            rawQuestions = JsonSerializer.Deserialize<List<QuizQuestionJsonModel>>(
                questionsJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("QuestionsJson is not a valid JSON format.", nameof(questionsJson), ex);
        }

        if (rawQuestions == null || rawQuestions.Count == 0)
        {
            throw new InvalidOperationException("Quiz must contain at least one question.");
        }

        var questions = new List<QuizQuestion>();

        foreach (var rawQuestion in rawQuestions)
        {
            if (rawQuestion == null)
            {
                throw new InvalidOperationException("Questions cannot contain null items.");
            }

            if (string.IsNullOrWhiteSpace(rawQuestion.Text))
            {
                throw new InvalidOperationException("Question text cannot be null or empty.");
            }

            if (rawQuestion.Answers == null || rawQuestion.Answers.Count < 2)
            {
                throw new InvalidOperationException("Each question must have at least two answers.");
            }

            if (rawQuestion.Answers.Any(a => a == null))
            {
                throw new InvalidOperationException("Answers cannot contain null items.");
            }

            if (rawQuestion.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text)))
            {
                throw new InvalidOperationException("Answer text cannot be null or empty.");
            }

            var correctAnswerCount = rawQuestion.Answers.Count(a => a.IsCorrect);

            if (correctAnswerCount != 1)
            {
                throw new InvalidOperationException("Each question must have exactly one correct answer.");
            }

            var answers = rawQuestion.Answers
                .Select(a => new QuizAnswer(a.Text, a.IsCorrect))
                .ToList();

            var question = new QuizQuestion(rawQuestion.Text, answers);

            questions.Add(question);
        }

        return questions;
    }
}