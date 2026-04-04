using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.EnrollmentService.Enrollments;

public class EnrollmentManager : DomainService
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public EnrollmentManager(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<Enrollment> CreateAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        DateTime enrolledAt,
        CancellationToken cancellationToken = default)
    {
        var alreadyActive = await _enrollmentRepository.ExistsActiveAsync(
            tenantId,
            courseId,
            studentId,
            cancellationToken);
 
        if (alreadyActive)
        {
            throw new BusinessException(EnrollmentServiceErrorCodes.AlreadyEnrolled)
                .WithData("TenantId", tenantId)
                .WithData("CourseId", courseId)
                .WithData("StudentId", studentId);
        }
        return new Enrollment(GuidGenerator.Create(), tenantId, courseId, studentId, enrolledAt);
    }

    public Task CompleteAsync(
        Enrollment enrollment,
        DateTime completedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(enrollment, nameof(enrollment));
        enrollment.Complete(completedAt);
        return Task.CompletedTask;
    }

    public Task CancelAsync(
        Enrollment enrollment,
        DateTime cancelledAt,
        CancellationToken cancellationToken = default
    )
    {
        Check.NotNull(enrollment, nameof(enrollment));
        enrollment.Cancel(cancelledAt);
        return Task.CompletedTask;
    }
}