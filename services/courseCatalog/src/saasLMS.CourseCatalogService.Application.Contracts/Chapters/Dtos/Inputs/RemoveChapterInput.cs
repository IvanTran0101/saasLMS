using System;

namespace saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;

public class RemoveChapterInput
{
    public Guid CourseId { get; set; }
    public Guid ChapterId { get; set; }
}