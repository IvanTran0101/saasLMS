using System;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Enrollments;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Users;

namespace saasLMS.CourseCatalogService.Courses;

public class CourseEnrollmentChecker : ICourseEnrollmentChecker, ITransientDependency
{
    private readonly ICurrentUser _currentUser;
    private readonly IEnrollmentAppService _enrollmentAppService;

    public CourseEnrollmentChecker(
        ICurrentUser currentUser,
        IEnrollmentAppService enrollmentAppService)
    {
        _currentUser = currentUser;
        _enrollmentAppService = enrollmentAppService;
    }

    public async Task CheckStudentEnrolledAsync(Guid courseId, Guid? studentId = null)
    {
        var isEnrolled = await IsStudentEnrolledAsync(courseId, studentId);
        if (!isEnrolled)
        {
            throw new AbpAuthorizationException("You are not enrolled in this course.");
        }
    }

    public async Task<bool> IsStudentEnrolledAsync(Guid courseId, Guid? studentId = null)
    {
        var resolvedStudentId = studentId ?? _currentUser.Id;
        if (!resolvedStudentId.HasValue)
        {
            return false;
        }

        if (courseId == Guid.Empty)
        {
            return false;
        }

        var result = await _enrollmentAppService.CheckActiveEnrollmentAsync(
            courseId,
            resolvedStudentId.Value);

        return result.IsActive;
    }
}
