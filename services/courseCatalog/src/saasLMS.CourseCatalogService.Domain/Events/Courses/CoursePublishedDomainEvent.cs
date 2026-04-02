using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Courses;

public class CoursePublishedDomainEvent
{
    public Course Course { get; }

    public CoursePublishedDomainEvent(Course course)
    {
        Course = course;
    }
}