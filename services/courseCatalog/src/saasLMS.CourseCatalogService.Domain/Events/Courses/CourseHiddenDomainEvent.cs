using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Courses;

public class CourseHiddenDomainEvent
{
    public Course Course { get; }

    public CourseHiddenDomainEvent(Course course)
    {
        Course = course;
    }
}