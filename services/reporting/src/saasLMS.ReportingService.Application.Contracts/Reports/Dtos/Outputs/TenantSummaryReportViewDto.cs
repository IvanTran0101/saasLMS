using System;

namespace saasLMS.ReportingService.Reports.Dtos.Outputs;

public class TenantSummaryReportViewDto
{
    public Guid TenantId { get; set; }

    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalCourses { get; set; }
    public int ActiveCourses { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}
