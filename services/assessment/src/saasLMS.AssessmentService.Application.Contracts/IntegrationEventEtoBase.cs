using System;

namespace saasLMS.AssessmentService;

public abstract class IntegrationEventEtoBase
{
    public Guid EventId { get; set; }
    public Guid TenantId { get; set; }
    public DateTime OccurredAt { get; set; }
}