using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Forms;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace saasLMS.AssessmentService.QuizAttempts;

public class QuizGradingService : DomainService
{
    private readonly IQuizFormResponseReader _responseReader;
    private readonly IRepository<QuizQuestionMap, Guid> _quizQuestionMapRepository;

    public QuizGradingService(
        IQuizFormResponseReader responseReader,
        IRepository<QuizQuestionMap, Guid> quizQuestionMapRepository)
    {
        _responseReader = responseReader;
        _quizQuestionMapRepository = quizQuestionMapRepository;
    }

    public async Task<QuizScoreResult> GradeAsync(
        Quiz quiz,
        QuizAttempt quizAttempt,
        Guid formResponseId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        Check.NotNull(quizAttempt, nameof(quizAttempt));

        if (quiz.Id != quizAttempt.QuizId)
        {
            throw new BusinessException("AssessmentService:QuizAttemptDoesNotBelongToQuiz")
                .WithData("QuizId", quiz.Id)
                .WithData("QuizAttemptId", quizAttempt.Id);
        }

        quiz.EnsureFormAttached();

        var maps = await _quizQuestionMapRepository.GetListAsync(
            x => x.QuizId == quiz.Id && x.TenantId == quiz.TenantId,
            includeDetails: false,
            cancellationToken: cancellationToken);

        if (maps.Count == 0)
        {
            throw new BusinessException("AssessmentService:QuizQuestionMapNotFound")
                .WithData("QuizId", quiz.Id);
        }

        var orderedQuestionIds = maps
            .OrderBy(x => x.QuestionIndex)
            .Select(x => x.FormQuestionId)
            .ToList();

        var answers = await _responseReader.GetAnswersAsync(formResponseId, cancellationToken);
        var keys = await _responseReader.GetQuestionKeysAsync(orderedQuestionIds, cancellationToken);

        var answerByQuestionId = answers.ToDictionary(x => x.QuestionId, x => x);
        var keyByQuestionId = keys.ToDictionary(x => x.QuestionId, x => x);

        var correctCount = 0;
        foreach (var questionId in orderedQuestionIds)
        {
            if (!answerByQuestionId.TryGetValue(questionId, out var answer))
            {
                continue;
            }
            if (!keyByQuestionId.TryGetValue(questionId, out var key))
            {
                continue;
            }
            if (IsCorrect(answer, key))
            {
                correctCount++;
            }
        }

        var total = orderedQuestionIds.Count;
        var score = CalculateScore(correctCount, total, quiz.MaxScore);
        return new QuizScoreResult(score, correctCount, total);
    }

    private static bool IsCorrect(QuizFormAnswer answer, QuizFormQuestionKey key)
    {
        if (key.CorrectChoiceId.HasValue)
        {
            return answer.SelectedChoiceId.HasValue &&
                   answer.SelectedChoiceId.Value == key.CorrectChoiceId.Value;
        }

        if (!string.IsNullOrWhiteSpace(key.CorrectText))
        {
            return string.Equals(
                (answer.Text ?? string.Empty).Trim(),
                key.CorrectText.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static decimal CalculateScore(int correctCount, int totalQuestionCount, decimal maxScore)
    {
        if (totalQuestionCount <= 0)
        {
            return 0;
        }
        return Math.Round((decimal)correctCount / totalQuestionCount * maxScore, 2, MidpointRounding.AwayFromZero);
    }
}
