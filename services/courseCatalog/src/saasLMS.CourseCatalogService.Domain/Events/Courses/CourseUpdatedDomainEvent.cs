using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Courses;

public class CourseUpdatedDomainEvent
{
    public Course Course { get; }

    public CourseUpdatedDomainEvent(Course course)
    {
        Course = course;
    }
}