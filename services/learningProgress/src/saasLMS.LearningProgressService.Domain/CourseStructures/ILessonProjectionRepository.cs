using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.LearningProgressService.CourseStructures;

public interface ILessonProjectionRepository : IRepository<LessonProjection, Guid>
{
    Task<LessonProjection?> FindByLessonIdAsync(
        Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Guid>> GetActiveLessonIdsByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default);

    Task<Guid?> GetFirstActiveLessonIdByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default
    );
}
