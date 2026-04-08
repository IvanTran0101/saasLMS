using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using saasLMS.ReportingService.Reports.Dtos.Outputs;

namespace saasLMS.ReportingService.Reports;

public interface IReportingAppService : IApplicationService
{
    Task<StudentCourseProgressViewDto?> GetStudentCourseProgressAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId);

    Task<ClassProgressViewDto?> GetClassProgressAsync(
        Guid tenantId,
        Guid courseId);

    Task<CourseOutcomeReportViewDto?> GetCourseOutcomeReportAsync(
        Guid tenantId,
        Guid courseId);

    Task<TenantSummaryReportViewDto?> GetTenantSummaryAsync(Guid tenantId);
}
