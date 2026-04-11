using System;
using Volo.Abp.Domain.Entities;

namespace saasLMS.LearningProgressService.CourseStructures;

public class LessonProjection : Entity<Guid>
{
    public Guid TenantId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid ChapterId { get; private set; }
    public Guid LessonId { get; private set; }
    public string Title { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; }

    protected LessonProjection()
    {
        Title = string.Empty;
    }

    public LessonProjection(
        Guid id,
        Guid tenantId,
        Guid courseId,
        Guid chapterId,
        Guid lessonId,
        string title,
        int sortOrder,
        bool isActive = true)
        : base(id)
    {
        TenantId = tenantId;
        CourseId = courseId;
        ChapterId = chapterId;
        LessonId = lessonId;
        Title = title;
        SortOrder = sortOrder;
        IsActive = isActive;
    }

    public void Activate(string title, int sortOrder)
    {
        Title = title;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
