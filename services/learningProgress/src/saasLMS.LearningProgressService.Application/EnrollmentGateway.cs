using System;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.DependencyInjection;

namespace saasLMS.LearningProgressService;

public class EnrollmentGateway : IEnrollmentGateway, ITransientDependency
{
    private readonly IEnrollmentAppService _enrollmentAppService;

    public EnrollmentGateway(IEnrollmentAppService enrollmentAppService)
    {
        _enrollmentAppService = enrollmentAppService;
    }

    public async Task<bool> IsEnrollmentActiveAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        if (courseId == Guid.Empty || studentId == Guid.Empty)
        {
            return false;
        }

        var result = await _enrollmentAppService.CheckActiveEnrollmentAsync(courseId, studentId);

        return result.IsActive;
    }
}