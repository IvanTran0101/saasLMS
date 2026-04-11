using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.CourseStructures;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.LearningProgressService.EntityFrameworkCore.CourseStructures;

public class LessonProjectionRepository
    : EfCoreRepository<LearningProgressServiceDbContext, LessonProjection, Guid>,
      ILessonProjectionRepository
{
    public LessonProjectionRepository(
        IDbContextProvider<LearningProgressServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<LessonProjection?> FindByLessonIdAsync(
        Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(
                lp => lp.TenantId == tenantId
                   && lp.LessonId == lessonId,
                cancellationToken);
    }

    public async Task<int> CountActiveByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .CountAsync(
                lp => lp.TenantId == tenantId
                   && lp.CourseId == courseId
                   && lp.IsActive,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Guid>> GetActiveLessonIdsByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .Where(lp => lp.TenantId == tenantId
                      && lp.CourseId == courseId
                      && lp.IsActive)
            .Select(lp => lp.LessonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Guid?> GetFirstActiveLessonIdByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(lp => lp.TenantId == tenantId
                         && lp.CourseId == courseId
                         && lp.IsActive).OrderBy(lp => lp.SortOrder)
            .Select(lp => (Guid?)lp.LessonId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
