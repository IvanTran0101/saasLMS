using System;
using System.Collections.Generic;

namespace saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;

public class ReorderChaptersInput
{
    public Guid CourseId { get; set; }

    /// <summary>Chapter IDs in the desired display order (first = OrderNo 1).</summary>
    public List<Guid> OrderedChapterIds { get; set; } = new();
}
