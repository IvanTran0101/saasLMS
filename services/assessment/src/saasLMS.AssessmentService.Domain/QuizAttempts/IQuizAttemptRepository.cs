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
        CancellationToken cancellationToken);
    Task<List<QuizAttempt>> GetListByStudentAsync(Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<int> GetAttemptCountAsync(Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<QuizAttempt?> FindLastedByQuizAndStudentAsync(Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default);
}