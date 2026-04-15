using System;
using Volo.Abp.Domain.Entities;

namespace saasLMS.ReportingService.ReadModels;

public class TenantSummaryReportView : Entity<Guid>
{
    public Guid TenantId { get; set; }

    public int TotalStudents { get; set; }
    public int ActiveStudents { get; set; }
    public int TotalCourses { get; set; }
    public int ActiveCourses { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    protected TenantSummaryReportView()
    {
    }

    public TenantSummaryReportView(Guid id, Guid tenantId)
        : base(id)
    {
        TenantId = tenantId;
        LastUpdatedAt = DateTime.UtcNow;
    }
}
