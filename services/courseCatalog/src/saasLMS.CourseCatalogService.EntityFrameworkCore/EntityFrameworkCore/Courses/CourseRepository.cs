using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.CourseCatalogService.Courses;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;

namespace saasLMS.CourseCatalogService.EntityFrameworkCore.Courses;

public class CourseRepository
    : EfCoreRepository<CourseCatalogServiceDbContext, Course, Guid>,
      ICourseRepository
{
    public CourseRepository(IDbContextProvider<CourseCatalogServiceDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Course?> FindByTitleAsync(
        Guid tenantId,
        string title,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(
                course => course.TenantId == tenantId && course.Title == title,
                cancellationToken);
    }

    public async Task<Course?> FindWithDetailsAsync(
        Guid id,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .Include(course => course.Chapters)
                .ThenInclude(chapter => chapter.Lessons)
                    .ThenInclude(lesson => lesson.Materials)
            .FirstOrDefaultAsync(
                course => course.Id == id && course.TenantId == tenantId,
                cancellationToken);
    }

    public async Task<List<Course>> GetListByInstructorAsync(
        Guid tenantId,
        Guid instructorId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .Where(course => course.TenantId == tenantId && course.InstructorId == instructorId)
            .OrderBy(course => course.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Course>> GetPublishedListAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .Where(course => course.TenantId == tenantId && course.Status == CourseStatus.Published)
            .OrderBy(course => course.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Course>> GetListByTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .Where(course => course.TenantId == tenantId)
            .OrderBy(course => course.Title)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByTitleAsync(
        Guid tenantId,
        string title,
        Guid? excludeCourseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetDbSetAsync();
        return await query
            .AsNoTracking()
            .Where(course => course.TenantId == tenantId && course.Title == title)
            .Where(course => !excludeCourseId.HasValue || course.Id != excludeCourseId.Value)
            .AnyAsync(cancellationToken);
    }
}

