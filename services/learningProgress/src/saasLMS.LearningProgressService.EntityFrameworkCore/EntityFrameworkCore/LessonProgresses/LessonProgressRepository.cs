using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.LearningProgressService.EntityFrameworkCore.LessonProgresses;

public class LessonProgressRepository
    : EfCoreRepository<LearningProgressServiceDbContext, LessonProgress, Guid>,
      ILessonProgressRepository
{
    public LessonProgressRepository(
        IDbContextProvider<LearningProgressServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<LessonProgress?> FindByLessonAndStudentAsync(
        Guid tenantId,
        Guid lessonId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                lp => lp.TenantId == tenantId
                   && lp.LessonId == lessonId
                   && lp.StudentId == studentId,
                cancellationToken);
    }

    public async Task<List<LessonProgress>> GetListByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        LessonProgressStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .Where(lp => lp.TenantId == tenantId
                      && lp.CourseId == courseId
                      && lp.StudentId == studentId
                      && (status == null || lp.Status == status))
            .OrderBy(lp => lp.LessonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByLessonAndStudentAsync(
        Guid tenantId,
        Guid lessonId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AnyAsync(
                lp => lp.TenantId == tenantId
                   && lp.LessonId == lessonId
                   && lp.StudentId == studentId,
                cancellationToken);
    }

    public async Task<LessonProgress?> GetLastViewedLessonAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .Where(lp => lp.TenantId == tenantId
                      && lp.CourseId == courseId
                      && lp.StudentId == studentId
                      && lp.Status != LessonProgressStatus.NotStarted)
            .OrderByDescending(lp => lp.LastViewedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<int> CountCompletedByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        IReadOnlyCollection<Guid> lessonIds,
        CancellationToken cancellationToken = default)
    {
        if (lessonIds.Count == 0)
        {
            return 0;
        }

        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .CountAsync(
                lp => lp.TenantId == tenantId
                   && lp.CourseId == courseId
                   && lp.StudentId == studentId
                   && lp.Status == LessonProgressStatus.Completed
                   && lessonIds.Contains(lp.LessonId),
                cancellationToken);
    }
}
