using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Chapters;

public class ChapterUpdatedDomainEvent
{
    public Chapter Chapter  { get; }
    public Guid    CourseId { get; }

    public ChapterUpdatedDomainEvent(Chapter chapter, Guid courseId)
    {
        Chapter  = chapter;
        CourseId = courseId;
    }
}