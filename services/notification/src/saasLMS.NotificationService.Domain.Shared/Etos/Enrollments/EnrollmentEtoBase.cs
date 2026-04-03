using System;

namespace saasLMS.NotificationService.Etos.Enrollments;

public class EnrollmentEtoBase
{
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid TenantId { get; set; }
}