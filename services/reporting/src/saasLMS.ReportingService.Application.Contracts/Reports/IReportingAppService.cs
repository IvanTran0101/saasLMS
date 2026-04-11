using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using saasLMS.ReportingService.Reports.Dtos.Outputs;

namespace saasLMS.ReportingService.Reports;

public interface IReportingAppService : IApplicationService
{
    Task<StudentCourseProgressViewDto?> GetStudentCourseProgressAsync(Guid courseId);

    Task<ClassProgressViewDto?> GetClassProgressAsync(Guid courseId);

    Task<CourseOutcomeReportViewDto?> GetCourseOutcomeReportAsync(Guid courseId);

    Task<TenantSummaryReportViewDto?> GetTenantSummaryAsync();
}
