using System;
using saasLMS.CourseCatalogService.Courses;

using Volo.Abp.EventBus;
namespace saasLMS.CourseCatalogService.Etos.Courses;

[EventName("lms.coursecatalog.coursecreated.v1")]
public class CourseCreatedEto : CourseCatalogEtoBase
{
   
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InstructorId  { get; set; }
    public CourseStatus Status { get; set; }
    
}