using System;

namespace saasLMS.ReportingService.Reports;

public static class ReportingCacheKeys
{
    public static string Student(Guid tenantId, Guid courseId, Guid studentId)
        => $"report:student:{tenantId}:{courseId}:{studentId}";

    public static string Class(Guid tenantId, Guid courseId)
        => $"report:class:{tenantId}:{courseId}";

    public static string CourseOutcome(Guid tenantId, Guid courseId)
        => $"report:course:{tenantId}:{courseId}";

    public static string Tenant(Guid tenantId)
        => $"report:tenant:{tenantId}";
}
