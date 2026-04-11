using System;
using Volo.Abp.Domain.Entities;

namespace saasLMS.ReportingService.ReadModels;

public class ClassProgressView : Entity<Guid>
{
    public Guid TenantId { get; set; }
    public Guid CourseId { get; set; }

    public int ActiveEnrollmentCount { get; set; }
    public int TotalStudents { get; set; }
    public int CompletedCount { get; set; }
    public int InProgressCount { get; set; }

    public int Bucket_0_25 { get; set; }
    public int Bucket_26_50 { get; set; }
    public int Bucket_51_75 { get; set; }
    public int Bucket_76_99 { get; set; }
    public int Bucket_100 { get; set; }

    public DateTime? LastRecalculatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }

    protected ClassProgressView()
    {
    }

    public ClassProgressView(Guid id, Guid tenantId, Guid courseId)
        : base(id)
    {
        TenantId = tenantId;
        CourseId = courseId;
        LastUpdatedAt = DateTime.UtcNow;
    }
}
