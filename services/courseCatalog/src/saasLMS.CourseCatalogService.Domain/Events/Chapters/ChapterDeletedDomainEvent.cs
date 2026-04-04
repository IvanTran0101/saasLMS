using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Chapters;

public class ChapterDeletedDomainEvent
{
    public Guid   TenantId  { get; }
    public Guid   CourseId  { get; }
    public Guid   ChapterId { get; }
    public string Title     { get; }
    public int    OrderNo   { get; }

    public ChapterDeletedDomainEvent(
        Guid   tenantId,
        Guid   courseId,
        Guid   chapterId,
        string title,
        int    orderNo)
    {
        TenantId  = tenantId;
        CourseId  = courseId;
        ChapterId = chapterId;
        Title     = title;
        OrderNo   = orderNo;
    }
}