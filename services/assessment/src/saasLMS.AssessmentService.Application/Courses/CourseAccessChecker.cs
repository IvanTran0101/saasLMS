using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Courses;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace saasLMS.AssessmentService.Courses;

public class CourseAccessChecker : ICourseAccessChecker, ITransientDependency
{
    private readonly ICurrentUser _currentUser;
    private readonly ICourseCatalogAppService _courseCatalogAppService;

    public CourseAccessChecker(
        ICurrentUser currentUser,
        ICourseCatalogAppService courseCatalogAppService)
    {
        _currentUser = currentUser;
        _courseCatalogAppService = courseCatalogAppService;
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

        var owner = await _courseCatalogAppService.GetOwnerAsync(courseId);
        return owner.InstructorId == resolvedUserId.Value;
    }
}
