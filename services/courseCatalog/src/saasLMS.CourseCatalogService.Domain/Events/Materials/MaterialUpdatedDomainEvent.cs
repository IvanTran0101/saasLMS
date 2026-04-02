using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Materials;

public class MaterialUpdatedDomainEvent
{
    public Material Material  { get; }
    public Guid     CourseId  { get; }
    public Guid     ChapterId { get; }

    public MaterialUpdatedDomainEvent(Material material, Guid courseId, Guid chapterId)
    {
        Material  = material;
        CourseId  = courseId;
        ChapterId = chapterId;
    }
}