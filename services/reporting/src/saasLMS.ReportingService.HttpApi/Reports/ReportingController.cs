using System;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using saasLMS.ReportingService.Reports;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using saasLMS.ReportingService.Permissions;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace saasLMS.ReportingService.Reports;

[RemoteService(Name = ReportingServiceRemoteServiceConsts.RemoteServiceName)]
[Area("ReportingService")]
[ControllerName("Reporting")]
[Route("api/reporting")]
public class ReportingController : ReportingServiceController
{
    private readonly IReportingAppService _reportingAppService;

    public ReportingController(IReportingAppService reportingAppService)
    {
        _reportingAppService = reportingAppService;
    }

    [HttpGet("student-course-progress")]
    [Authorize(ReportingServicePermissions.Reports.StudentView)]
    public Task<StudentCourseProgressViewDto?> GetStudentCourseProgressAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId)
    {
        return _reportingAppService.GetStudentCourseProgressAsync(tenantId, courseId, studentId);
    }

    [HttpGet("class-progress")]
    [Authorize(ReportingServicePermissions.Reports.View)]
    public Task<ClassProgressViewDto?> GetClassProgressAsync(
        Guid tenantId,
        Guid courseId)
    {
        return _reportingAppService.GetClassProgressAsync(tenantId, courseId);
    }

    [HttpGet("course-outcome")]
    [Authorize(ReportingServicePermissions.Reports.View)]
    public Task<CourseOutcomeReportViewDto?> GetCourseOutcomeReportAsync(
        Guid tenantId,
        Guid courseId)
    {
        return _reportingAppService.GetCourseOutcomeReportAsync(tenantId, courseId);
    }

    [HttpGet("tenant-summary")]
    [Authorize(ReportingServicePermissions.Reports.View)]
    public Task<TenantSummaryReportViewDto?> GetTenantSummaryAsync(Guid tenantId)
    {
        return _reportingAppService.GetTenantSummaryAsync(tenantId);
    }
}
