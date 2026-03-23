using System;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Shared;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.AssessmentService.QuizAttempts;

public class QuizAttemptManager : DomainService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;

    public QuizAttemptManager(IQuizAttemptRepository quizAttemptRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
    }

    public async Task<QuizAttempt> CreateAsync(
        Quiz quiz,
        Guid tenantId,
        Guid studentId,
        DateTime startedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quiz, nameof(quiz));

        var currentAttemptCount = await _quizAttemptRepository.GetAttemptCountAsync(
            tenantId, quiz.Id, studentId, cancellationToken);
        if (quiz.AttemptPolicy == AttemptPolicy.OneTime && currentAttemptCount > 0)
        {
            throw new BusinessException("AssessmentService:QuizAttemptLimitExceeded")
                .WithData("QuizId", quiz.Id)
                .WithData("StudentId", studentId);
        }

        return new QuizAttempt(
            GuidGenerator.Create(),            tenantId,
            quiz.Id,
            studentId,
            currentAttemptCount + 1,
            startedAt);
        
    }

    public Task CompleteAsync(
        QuizAttempt quizAttempt,
        decimal score,
        DateTime completedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quizAttempt, nameof(quizAttempt));
        quizAttempt.Complete(score, completedAt);
        return Task.CompletedTask;
    }

    public Task ExpireAsync(QuizAttempt quizAttempt,
        DateTime expiredAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(quizAttempt, nameof(quizAttempt));
        quizAttempt.Expire(expiredAt);
        return Task.CompletedTask;
    }
}