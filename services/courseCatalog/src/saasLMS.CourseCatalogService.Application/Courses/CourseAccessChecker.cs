using System;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace saasLMS.CourseCatalogService.Courses;

public class CourseAccessChecker : ICourseAccessChecker, ITransientDependency
{
    private readonly ICurrentUser _currentUser;
    private readonly ICourseRepository _courseRepository;

    public CourseAccessChecker(
        ICurrentUser currentUser,
        ICourseRepository courseRepository)
    {
        _currentUser = currentUser;
        _courseRepository = courseRepository;
    }

    public async Task CheckCanManageCourseAsync(Guid courseId, Guid? userId = null)
    {
        var canManage = await CanManageCourseAsync(courseId, userId);
        if (!canManage)
        {
            throw new AbpAuthorizationException("You are not allowed to manage this course.");
        }
    }

    public async Task<bool> CanManageCourseAsync(Guid courseId, Guid? userId = null)
    {
        var resolvedUserId = userId ?? _currentUser.Id;
        if (!resolvedUserId.HasValue)
        {
            return false;
        }

        if (courseId == Guid.Empty)
        {
            return false;
        }

        var tenantId = _currentUser.TenantId;
        if (!tenantId.HasValue)
        {
            return false;
        }

        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            return false;
        }

        return course.InstructorId == resolvedUserId.Value;
    }
}
