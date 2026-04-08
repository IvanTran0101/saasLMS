using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace saasLMS.EnrollmentService.EntityFrameworkCore.Enrollments;

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
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(
                enrollment => enrollment.TenantId == tenantId
                              && enrollment.CourseId == courseId
                              && enrollment.StudentId == studentId,
                cancellationToken);
    }

    public async Task<List<Enrollment>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        EnrollmentStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        var filtered = query
            .AsNoTracking()
            .Where(enrollment => enrollment.TenantId == tenantId
                                 && enrollment.StudentId == studentId);

        if (status.HasValue)
        {
            filtered = filtered.Where(enrollment => enrollment.Status == status.Value);
        }

        return await filtered
            .OrderByDescending(enrollment => enrollment.EnrolledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Enrollment>> GetListByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .Where(enrollment => enrollment.TenantId == tenantId
                                 && enrollment.CourseId == courseId)
            .OrderByDescending(enrollment => enrollment.EnrolledAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .AnyAsync(
                enrollment => enrollment.TenantId == tenantId
                              && enrollment.CourseId == courseId
                              && enrollment.StudentId == studentId,
                cancellationToken);
    }

    public async Task<bool> ExistsActiveAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .AnyAsync(
                enrollment => enrollment.TenantId == tenantId
                              && enrollment.CourseId == courseId
                              && enrollment.StudentId == studentId
                              && enrollment.Status == EnrollmentStatus.Active,
                cancellationToken);
    }
}
