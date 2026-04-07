using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.AssessmentService.QuizAttempts;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace saasLMS.AssessmentService.EntityFrameworkCore.Repositories;

public class QuizAttemptRepository
    : EfCoreRepository<AssessmentServiceDbContext, QuizAttempt, Guid>,
        IQuizAttemptRepository
{
    public QuizAttemptRepository(IDbContextProvider<AssessmentServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<QuizAttempt>> GetListByQuizAsync(
        Guid tenantId,
        Guid quizId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TenantId == tenantId && x.QuizId == quizId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<QuizAttempt>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TenantId == tenantId && x.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetAttemptCountAsync(
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.CountAsync(
            x => x.TenantId == tenantId &&
                 x.QuizId == quizId &&
                 x.StudentId == studentId,
            cancellationToken);
    }

    public async Task<QuizAttempt?> FindLatestByQuizAndStudentAsync(
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.QuizId == quizId &&
                x.StudentId == studentId)
            .OrderByDescending(x => x.AttemptNumber)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsByQuizAndStudentAsync(
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.QuizId == quizId &&
                 x.StudentId == studentId,
            cancellationToken);
    }

    public async Task<QuizAttempt?> FindByQuizAndStudentAsync(
        Guid tenantId,
        Guid quizId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        return await FindLatestByQuizAndStudentAsync(
            tenantId,
            quizId,
            studentId,
            cancellationToken);
    }
}
