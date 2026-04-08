using System;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using Volo.Abp.DependencyInjection;

namespace saasLMS.EnrollmentService;

public class CourseCatalogGateway : ICourseCatalogGateway, ITransientDependency
{
    private readonly ICourseCatalogAppService _courseCatalogAppService;

    public CourseCatalogGateway(
        ICourseCatalogAppService courseCatalogAppService)
    {
        _courseCatalogAppService = courseCatalogAppService;
    }

    public async Task<CourseEligibilityResult?> GetEnrollmentEligibility(
        Guid courseId,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        CourseEligibilityDto? course = await _courseCatalogAppService.GetEnrollmentEligibilityAsync(
            courseId,
            tenantId);
        if (course == null)
        {
            return null;
        }

        return new CourseEligibilityResult
        {
            CourseId = course.CourseId,
            TenantId = course.TenantId,
            Status = course.Status,
            IsHidden = course.IsHidden
        };
    }
}
