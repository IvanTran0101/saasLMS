using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Lessons;

public class LessonCreatedDomainEvent
{
    public Lesson Lesson    { get; }
    public Guid   CourseId  { get; }

    public LessonCreatedDomainEvent(Lesson lesson, Guid courseId)
    {
        Lesson   = lesson;
        CourseId = courseId;
    }
}