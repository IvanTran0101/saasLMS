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
        string description,
        int? timeLimitMinutes,
        decimal maxScore,
        AttemptPolicy attemptPolicy,
        string questionJson,
        CancellationToken cancellationToken = default)
    {
        var quiz = new Quiz(
            GuidGenerator.Create(),
            tenantId,
            courseId,
            lessonId, title, timeLimitMinutes, maxScore, attemptPolicy, questionJson);
        return Task.FromResult(quiz);
    }

    public Task PublicAsync(
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
    
    
}