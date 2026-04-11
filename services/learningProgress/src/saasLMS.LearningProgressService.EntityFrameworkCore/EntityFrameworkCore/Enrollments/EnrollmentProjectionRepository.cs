using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.LearningProgressService.Enrollments;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.LearningProgressService.EntityFrameworkCore.Enrollments;

public class EnrollmentProjectionRepository
    : EfCoreRepository<LearningProgressServiceDbContext, EnrollmentProjection, Guid>,
      IEnrollmentProjectionRepository
{
    public EnrollmentProjectionRepository(
        IDbContextProvider<LearningProgressServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<EnrollmentProjection?> FindByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .FirstOrDefaultAsync(
                ep => ep.TenantId == tenantId
                   && ep.CourseId == courseId
                   && ep.StudentId == studentId,
                cancellationToken);
    }

    public async Task<List<EnrollmentProjection>> GetActiveByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(ep => ep.TenantId == tenantId
                      && ep.CourseId == courseId
                      && ep.IsActive)
            .ToListAsync(cancellationToken);
    }
}
