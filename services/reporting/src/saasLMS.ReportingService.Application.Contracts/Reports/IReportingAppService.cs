using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp;

namespace saasLMS.ReportingService.Reports;

[RemoteService(Name = ReportingServiceRemoteServiceConsts.RemoteServiceName)]
public interface IReportingAppService : IApplicationService
{
    Task<StudentCourseProgressViewDto?> GetStudentCourseProgressAsync(Guid courseId);

    Task<ClassProgressViewDto?> GetClassProgressAsync(Guid courseId);

    Task<CourseOutcomeReportViewDto?> GetCourseOutcomeReportAsync(Guid courseId);

    Task<TenantSummaryReportViewDto?> GetTenantSummaryAsync();
}
