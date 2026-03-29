using System;

namespace saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;

public class CreateChapterInput
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } =  string.Empty;
}