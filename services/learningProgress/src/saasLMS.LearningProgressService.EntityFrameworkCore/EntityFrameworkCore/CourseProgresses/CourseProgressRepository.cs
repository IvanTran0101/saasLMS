using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.CourseProgresses;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.LearningProgressService.EntityFrameworkCore.CourseProgresses;

public class CourseProgressRepository
    : EfCoreRepository<LearningProgressServiceDbContext, CourseProgress, Guid>,
      ICourseProgressRepository
{
    public CourseProgressRepository(
        IDbContextProvider<LearningProgressServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<CourseProgress?> FindByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                cp => cp.TenantId == tenantId
                   && cp.CourseId == courseId
                   && cp.StudentId == studentId,
                cancellationToken);
    }

    public async Task<List<CourseProgress>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AsNoTracking()
            .Where(cp => cp.TenantId == tenantId
                      && cp.StudentId == studentId)
            .OrderByDescending(cp => cp.LastAccessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .AnyAsync(
                cp => cp.TenantId == tenantId
                   && cp.CourseId == courseId
                   && cp.StudentId == studentId,
                cancellationToken);
    }
}