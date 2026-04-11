using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.LearningProgressService.LessonProgresses;

public interface ILessonProgressRepository : IRepository<LessonProgress, Guid>
{
    Task<LessonProgress?> FindByLessonAndStudentAsync(
        Guid tenantId,
        Guid lessonId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<List<LessonProgress>> GetListByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        LessonProgressStatus? status = null,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByLessonAndStudentAsync(
        Guid tenantId,
        Guid lessonId,
        Guid studentId,
        CancellationToken cancellationToken = default);
    Task<LessonProgress?> GetLastViewedLessonAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        CancellationToken cancellationToken = default);

    Task<int> CountCompletedByCourseAndStudentAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        IReadOnlyCollection<Guid> lessonIds,
        CancellationToken cancellationToken = default);
}
