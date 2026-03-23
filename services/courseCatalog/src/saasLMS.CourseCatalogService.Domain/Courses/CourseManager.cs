using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.CourseCatalogService.Courses;

public class CourseManager : DomainService
{
    private readonly ICourseRepository _courseRepository;

    public CourseManager(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<Course> CreateAsync(
        Guid tenantId,
        string title,
        string? description,
        Guid instructorId,
        CancellationToken cancellationToken = default)
    {
        var existedCourse = await _courseRepository.FindByTitleAsync(tenantId, title, cancellationToken);
        if (existedCourse != null)
        {
            throw new BusinessException("CourseCatalog:DuplicateCourseTitle").WithData("Title", title);
        }
        return new Course(GuidGenerator.Create(), tenantId, title, description, instructorId);
    }

    public async Task RenameAsync(Course course, string newTitle, CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        var existed = await _courseRepository.ExistsByTitleAsync(course.TenantId,
            newTitle,
            course.Id,
            cancellationToken);
        if (existed)
        {
            throw new BusinessException("CourseCatalog:DuplicateCourseTitle").WithData("Title", newTitle);
        }
        course.Rename(newTitle);
    }
}