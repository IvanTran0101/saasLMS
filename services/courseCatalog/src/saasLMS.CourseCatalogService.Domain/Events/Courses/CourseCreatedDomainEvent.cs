using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Courses;

public class CourseCreatedDomainEvent
{
    public Course Course { get; }

    public CourseCreatedDomainEvent(Course course)
    {
        Course = course;
    }
}