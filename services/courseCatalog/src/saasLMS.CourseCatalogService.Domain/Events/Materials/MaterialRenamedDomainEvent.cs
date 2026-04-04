using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Materials;

public class MaterialRenamedDomainEvent
{
    public Material Material { get; }

    public MaterialRenamedDomainEvent(Material material)
    {
        Material = material;
    }
}