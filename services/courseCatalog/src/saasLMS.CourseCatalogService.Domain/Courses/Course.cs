using System;
using System.Collections.Generic;
using System.Linq;
using saasLMS.CourseCatalogService.Etos.Chapters;
using saasLMS.CourseCatalogService.Etos.Courses;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Domain.Entities.Events.Distributed;


namespace saasLMS.CourseCatalogService.Courses;

public class Course : FullAuditedAggregateRoot<Guid>
{
    public Guid TenantId { get; protected set; }
    public string Title { get; protected set; }
    public string? Description { get; protected set; }
    public CourseStatus Status { get; protected set; }
    public Guid InstructorId { get; protected set; }
    private readonly List<Chapter> _chapters;
    public IReadOnlyCollection<Chapter> Chapters => _chapters.AsReadOnly();
    
    protected  Course()
    {
        Title = string.Empty;
        _chapters = new List<Chapter>();
    }

    public Course(Guid id, Guid tenantId, string title, string? description, Guid instructorId) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        }

        if (instructorId == Guid.Empty)
        {
            throw new ArgumentException("Instructor id cannot be empty.", nameof(instructorId));
        }

        TenantId = tenantId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        InstructorId = instructorId;
        Status = CourseStatus.Draft;
        _chapters = new List<Chapter>();
       
    }

    public void Rename(string newTitle)
    {
        Title = Check.NotNullOrWhiteSpace(newTitle, nameof(newTitle));
    }

    public Chapter AddChapter(Guid chapterId, string title)
    {
        if (chapterId == Guid.Empty)
        {
            throw new ArgumentException("Chapter id cannot be empty.", nameof(chapterId));
        }
        var chapter = new Chapter(chapterId, TenantId, Id, title, _chapters.Count + 1);
        _chapters.Add(chapter);
        AddDistributedEvent(new ChapterCreatedEto()
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId = TenantId,
            ChapterId = chapter.Id,
            CourseId = Id,
            OrderNo = chapter.OrderNo,
            Title = chapter.Title
        });
        return chapter;
        
        
    }

    public void RemoveChapter(Guid chapterId)
    {
        var chapter = _chapters.FirstOrDefault(x => x.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");        
        }
        
        _chapters.Remove(chapter);
        NormalizeChaptersOrder();
        
    }
    public void Publish()
    {
        if (Status == CourseStatus.Published)
        {
            return;
        }
        Check.NotNullOrWhiteSpace(Title, nameof(Title));
        Check.NotNullOrWhiteSpace(Description, nameof(Description));
        if (!_chapters.Any())
        {
            throw new BusinessException("CourseCatalog:CannotPublishWithoutChapter");
        }
        if (!_chapters.Any(c => c.Lessons.Any()))
        {
            throw new BusinessException("CourseCatalog:CannotPublishWithoutLesson");
        }
        Status = CourseStatus.Published;

        AddDistributedEvent(new CoursePublishedEto()
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId = TenantId,
            CourseId = Id,
            Title = Title,
            Description = Description,
            InstructorId = InstructorId,
            Status = Status
        });
    }

    public void Hide()
    {
        if (Status == CourseStatus.Hidden)
        {
            return;
        }
        Status = CourseStatus.Hidden;
        AddDistributedEvent(new CourseHiddenEto()
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId = TenantId,
            CourseId = Id,
            Title = Title,
            InstructorId = InstructorId,
            Description = Description,
            Status = Status
        });
    }
    private void NormalizeChaptersOrder()
    {
        for (int i = 0; i < _chapters.Count; i++)
        {
            _chapters[i].SetOrder(i + 1);
        }
    }
    public void UpdateDetails(string title, string? description)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        AddDistributedEvent(new CourseUpdatedEto
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId = TenantId,
            CourseId = Id,
            Title = Title,
            Description = Description,
            InstructorId = InstructorId,
            Status = Status
        });
    }
}
