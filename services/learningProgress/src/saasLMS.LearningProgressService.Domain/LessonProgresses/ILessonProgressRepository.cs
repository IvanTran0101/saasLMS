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
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByLessonAndStudentAsync(
        Guid tenantId,
        Guid lessonId,
        Guid studentId,
        CancellationToken cancellationToken = default);
}
