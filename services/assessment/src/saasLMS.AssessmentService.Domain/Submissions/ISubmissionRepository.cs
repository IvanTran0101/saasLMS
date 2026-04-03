using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.AssessmentService.Submissions;

public interface ISubmissionRepository : IRepository<Submission, Guid>
{
    Task<Submission?> FindByAssignmentAndStudentAsync(Guid tenantId,
        Guid assignmentId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<List<Submission>> GetListByAssignmentAsync(Guid tenantId,
        Guid assignmentId,
        CancellationToken cancellationToken = default);
    Task<List<Submission>> GetListByStudentAsync(Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByAssignmentAndStudentAsync(Guid tenantId,
        Guid assignmentId,
        Guid studentId,
        CancellationToken cancellationToken = default
    );
}