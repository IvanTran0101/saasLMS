using System;
using System.Threading;
using System.Threading.Tasks;

namespace saasLMS.EnrollmentService;

public interface ICourseCatalogGateway
{
    /// Lấy thông tin eligibility của course.
    /// Trả null nếu course không tồn tại.
    Task<CourseEligibilityResult?> GetEnrollmentEligibility(
        Guid courseId,
        Guid tenantId,
        CancellationToken cancellationToken = default);
}

public sealed class CourseEligibilityResult
{
    public Guid CourseId { get; init; }
    public Guid TenantId { get; init; }
    public saasLMS.CourseCatalogService.Courses.CourseStatus Status { get; init; }
 
    public bool IsHidden { get; init; }
}
