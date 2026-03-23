using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.LearningProgressService.CourseProgresses;

public interface ICourseProgressRepository : IRepository<CourseProgress, Guid>
{
    Task<CourseProgress?> FindByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<List<CourseProgress>> GetListByStudentAsync(
        Guid tenantId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);
}
