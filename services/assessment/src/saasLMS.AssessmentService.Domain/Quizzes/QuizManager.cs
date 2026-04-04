using System;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.AssessmentService.Quizzes;

public class QuizManager : DomainService
{
    private readonly IQuizRepository _quizRepository;

    public QuizManager(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public Task<Quiz> CreateAsync(
        Guid tenantId,
        Guid courseId,
        Guid lessonId,
        string title,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        string questionJson,
        DateTime createdAt,
        CancellationToken cancellationToken = default)
    {
        if (attemptPolicy != AttemptPolicy.OneTime)
        {
            throw new BusinessException("Only one time policy can be supported.");
        }
        
        var quiz = Quiz.Create(
            GuidGenerator.Create(),
            tenantId,
            courseId,
            lessonId,
            title,
            timeLimitMinutes,
            maxScore,
            attemptPolicy,
            questionJson,
            createdAt);
        return Task.FromResult(quiz);
    }

    public Task PublishAsync(
        Quiz quiz,
        DateTime publishedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        quiz.Publish(publishedAt);
        return Task.CompletedTask;
    }
    public Task CloseAsync(
        Quiz quiz,
        DateTime closedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));
        quiz.Close(closedAt);
        return Task.CompletedTask;
    }
    public Task UpdateInfoAsync(
        Quiz quiz,
        string title,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        string questionJson,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));

        if (attemptPolicy != AttemptPolicy.OneTime)
        {
            throw new BusinessException("Only one time policy can be supported.");
        }

        quiz.UpdateInfo(
            title,
            timeLimitMinutes,
            maxScore,
            attemptPolicy,
            questionJson,
            updatedAt);

        return Task.CompletedTask;
    }
    
}