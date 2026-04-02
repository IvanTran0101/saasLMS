using System;
using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Lessons;

public class LessonDeletedDomainEvent
{
    public Guid               TenantId     { get; }
    public Guid               CourseId     { get; }
    public Guid               ChapterId    { get; }
    public Guid               LessonId     { get; }
    public string             Title        { get; }
    public int                SortOrder    { get; }
    public LessonContentState ContentState { get; }

    public LessonDeletedDomainEvent(
        Guid               tenantId,
        Guid               courseId,
        Guid               chapterId,
        Guid               lessonId,
        string             title,
        int                sortOrder,
        LessonContentState contentState)
    {
        TenantId     = tenantId;
        CourseId     = courseId;
        ChapterId    = chapterId;
        LessonId     = lessonId;
        Title        = title;
        SortOrder    = sortOrder;
        ContentState = contentState;
    }
}