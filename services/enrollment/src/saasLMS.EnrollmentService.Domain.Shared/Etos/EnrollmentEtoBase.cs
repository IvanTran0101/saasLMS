using System;

namespace saasLMS.EnrollmentService.Etos;

public abstract class EnrollmentEtoBase
{
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid? TenantId { get; set; }
}