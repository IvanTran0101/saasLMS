using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.AssessmentService.Quizzes;

public interface IQuizRepository : IRepository<Quiz, Guid>
{
    Task<List<Quiz>> GetListByCourseAsync(Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default);
    Task<List<Quiz>> GetListByLessonAsync(Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default);
}