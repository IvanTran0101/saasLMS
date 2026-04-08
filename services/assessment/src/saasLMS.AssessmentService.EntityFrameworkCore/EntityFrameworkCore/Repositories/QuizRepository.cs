using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.AssessmentService.Quizzes;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace saasLMS.AssessmentService.EntityFrameworkCore.Repositories;

public class QuizRepository
    : EfCoreRepository<AssessmentServiceDbContext, Quiz, Guid>,
        IQuizRepository
{
    public QuizRepository(IDbContextProvider<AssessmentServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<List<Quiz>> GetListByCourseAsync(
        Guid tenantId,
        Guid courseId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TenantId == tenantId && x.CourseId == courseId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Quiz>> GetListByLessonAsync(
        Guid tenantId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        var dbSet = await GetDbSetAsync();
        return await dbSet
            .Where(x => x.TenantId == tenantId && x.LessonId == lessonId)
            .ToListAsync(cancellationToken);
    }
}
