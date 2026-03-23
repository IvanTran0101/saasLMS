using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.AssessmentService.Assignments;

public interface IAssignmentRepository : IRepository<Assignment, Guid>
{
    Task<List<Assignment>> GetListByCourseAsync(Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default);
    Task<List<Assignment>> GetListByLessonAsync(Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default);
}