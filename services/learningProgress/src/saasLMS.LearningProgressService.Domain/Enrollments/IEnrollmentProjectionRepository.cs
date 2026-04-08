using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.LearningProgressService.Enrollments;

public interface IEnrollmentProjectionRepository : IRepository<EnrollmentProjection, Guid>
{
    Task<EnrollmentProjection?> FindByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<List<EnrollmentProjection>> GetActiveByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default);
}
