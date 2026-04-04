using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Lessons;

public class LessonHiddenDomainEvent
{
    public Lesson Lesson   { get; }
    public Guid   CourseId { get; }

    public LessonHiddenDomainEvent(Lesson lesson, Guid courseId)
    {
        Lesson   = lesson;
        CourseId = courseId;
    }
}