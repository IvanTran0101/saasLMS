using System;

namespace saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;

public class RenameChapterInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
    public string NewTitle { get; set; } =  String.Empty;
}