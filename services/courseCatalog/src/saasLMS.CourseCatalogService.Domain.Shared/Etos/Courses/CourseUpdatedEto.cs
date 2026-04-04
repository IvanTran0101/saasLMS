using System;
using saasLMS.CourseCatalogService.Courses;

using Volo.Abp.EventBus;
namespace saasLMS.CourseCatalogService.Etos.Courses;

[EventName("lms.coursecatalog.courseupdated.v1")]
public class CourseUpdatedEto : CourseCatalogEtoBase

{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InstructorId { get; set; }
    public CourseStatus Status { get; set; }
}