using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace saasLMS.CourseCatalogService.Courses;

public class Lesson : Entity<Guid>
{
    public Guid ChapterId { get; protected set; }
    public string Title { get; protected set; }
    public int SortOrder { get; protected set; }
    public LessonContentState ContentState { get; protected set; }

    protected Lesson()
    {
        
    }

    public Lesson(Guid id, Guid chapterId, string title, int sortOrder) : base(id)
    {
        if (chapterId == Guid.Empty)
        {
            throw new ArgumentException("Chapter id cannot be empty.", nameof(chapterId));
        }

        if (sortOrder <= 0)
        {
            throw new ArgumentException("Sort order must be greater than 0.", nameof(sortOrder));        }
        ChapterId = chapterId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        SortOrder = sortOrder;
        ContentState = LessonContentState.Empty;
    }

    public void MarkAsContainingContent()
    {
        ContentState = LessonContentState.HasContent;
    }
    public void MarkAsEmpty()
    {
        ContentState = LessonContentState.Empty;
    }
}