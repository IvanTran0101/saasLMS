using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.AssessmentService.QuizAttempts;

public interface IQuizAttemptRepository : IRepository<QuizAttempt, Guid>
{
    Task<List<QuizAttempt>> GetListByQuizAsync(Guid tenantId,
        Guid quizId,
        CancellationToken cancellationToken  = default);
    Task<List<QuizAttempt>> GetListByStudentAsync(Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<int> GetAttemptCountAsync(Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<QuizAttempt?> FindLatestByQuizAndStudentAsync(Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByQuizAndStudentAsync(
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<QuizAttempt?> FindByQuizAndStudentAsync(
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default);
}