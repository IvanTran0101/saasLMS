using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.EnrollmentService.EntityFrameworkCore.Repositories;

public class EnrollmentRepository
    : EfCoreRepository<EnrollmentServiceDbContext, Enrollment, Guid>,
        IEnrollmentRepository
{
    public EnrollmentRepository(IDbContextProvider<EnrollmentServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Enrollment?> FindByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.CourseId == courseId &&
                x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Enrollment>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        EnrollmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        var query = dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.StudentId == studentId);

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<List<Enrollment>> GetListByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.CourseId == courseId &&
            x.StudentId == studentId,
            cancellationToken);
    }

    public async Task<bool> ExistsActiveAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(x =>
            x.TenantId == tenantId &&
            x.CourseId == courseId &&
            x.StudentId == studentId &&
            x.Status == EnrollmentStatus.Active,
            cancellationToken);
    }
}
