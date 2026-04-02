using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Materials;

public class MaterialVideoLinkUpdatedDomainEvent
{
    public Material Material  { get; }
    public Guid     CourseId  { get; }
    public Guid     ChapterId { get; }

    public MaterialVideoLinkUpdatedDomainEvent(Material material, Guid courseId, Guid chapterId)
    {
        Material  = material;
        CourseId  = courseId;
        ChapterId = chapterId;
    }
}