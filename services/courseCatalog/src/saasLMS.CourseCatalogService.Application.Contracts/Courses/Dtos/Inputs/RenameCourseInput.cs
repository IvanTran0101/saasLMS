using System;

namespace saasLMS.CourseCatalogService.Courses.Dtos.Inputs;

public class RenameCourseInput
{
  public Guid CourseId  { get; set; }
  public string NewTitle { get; set; } = string.Empty;
}