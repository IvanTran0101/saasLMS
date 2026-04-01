using System;
using System.Threading;
using System.Threading.Tasks;

namespace saasLMS.LearningProgressService;

public interface IEnrollmentGateway
{
    Task<bool> IsEnrollmentActiveAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);
}