using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace saasLMS.AssessmentService.EntityFrameworkCore.Repositories;

public class SubmissionRepository
    : EfCoreRepository<AssessmentServiceDbContext, Submission, Guid>,
        ISubmissionRepository
{
    public SubmissionRepository(IDbContextProvider<AssessmentServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Submission?> FindByAssignmentAndStudentAsync(
        Guid tenantId,
        Guid assignmentId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.AssignmentId == assignmentId &&
                x.StudentId == studentId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Submission>> GetListByAssignmentAsync(
        Guid tenantId,
        Guid assignmentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.AssignmentId == assignmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Submission>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x =>
                x.TenantId == tenantId &&
                x.StudentId == studentId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByAssignmentAndStudentAsync(
        Guid tenantId,
        Guid assignmentId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet.AnyAsync(
            x => x.TenantId == tenantId &&
                 x.AssignmentId == assignmentId &&
                 x.StudentId == studentId,
            cancellationToken);
    }
}
