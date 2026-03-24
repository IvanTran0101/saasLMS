using System;

namespace saasLMS.CourseCatalogService.Etos;

public abstract class CourseCatalogEtoBase
{
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid TenantId { get; set; }
}