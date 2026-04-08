using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.AssessmentService.Assignments;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace saasLMS.AssessmentService.EntityFrameworkCore.Repositories;

public class AssignmentRepository
    : EfCoreRepository<AssessmentServiceDbContext, Assignment, Guid>,
        IAssignmentRepository
{
    public AssignmentRepository(IDbContextProvider<AssessmentServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Assignment>> GetListByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TenantId == tenantId && x.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Assignment>> GetListByLessonAsync(
        Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TenantId == tenantId && x.LessonId == lessonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Assignment>> GetPublishedListByLessonAsync(
        Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.LessonId == lessonId &&
                x.Status == AssignmentStatus.Published)
            .ToListAsync(cancellationToken);
    }

    public async Task<Assignment?> FindPublishedByLessonAsync(
        Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.LessonId == lessonId &&
                x.Status == AssignmentStatus.Published)
            .OrderByDescending(x => x.Deadline)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
