using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Lessons;

public class LessonUpdatedDomainEvent
{
    public Lesson Lesson    { get; }
    public Guid   CourseId  { get; }

    public LessonUpdatedDomainEvent(Lesson lesson, Guid courseId)
    {
        Lesson   = lesson;
        CourseId = courseId;
    }
}