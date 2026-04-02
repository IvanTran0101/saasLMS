using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Chapters;

public class ChapterCreatedDomainEvent
{
    public Chapter Chapter  { get; }
    public Guid    CourseId { get; }

    public ChapterCreatedDomainEvent(Chapter chapter, Guid courseId)
    {
        Chapter  = chapter;
        CourseId = courseId;
    }
}