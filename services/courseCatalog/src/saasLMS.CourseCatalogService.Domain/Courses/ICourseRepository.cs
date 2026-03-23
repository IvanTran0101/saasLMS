using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.CourseCatalogService.Courses;

public interface ICourseRepository : IRepository<Course, Guid>
{
    Task<Course?> FindByTitleAsync(Guid tenantId, string title, CancellationToken cancellationToken = default);
    Task<Course?> FindWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<List<Course>> GetListByInstructorAsync(Guid tenantId, Guid instructorId, CancellationToken cancellationToken = default);
    Task<List<Course>> GetPublishedListAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<List<Course>> GetListByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);
    
    Task<bool> ExistsByTitleAsync(Guid tenantId, string title, Guid? excludeCourseId = null, CancellationToken cancellationToken = default);
}