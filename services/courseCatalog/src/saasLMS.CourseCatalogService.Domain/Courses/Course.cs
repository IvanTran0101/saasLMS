using System;
using System.Collections.Generic;
using System.Linq;
using saasLMS.CourseCatalogService.Etos.Chapters;
using saasLMS.CourseCatalogService.Etos.Courses;
using saasLMS.CourseCatalogService.Events.Chapters;
using saasLMS.CourseCatalogService.Events.Courses;
using saasLMS.CourseCatalogService.Events.Lessons;
using saasLMS.CourseCatalogService.Events.Materials;
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
        
        AddLocalEvent(new CourseCreatedDomainEvent(this));
    }

    public void Rename(string newTitle)
    {
        Title = Check.NotNullOrWhiteSpace(newTitle, nameof(newTitle));
        
        AddLocalEvent(new CourseUpdatedDomainEvent(this));
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
        
        AddLocalEvent(new CoursePublishedDomainEvent(this));
    }

    public void Hide()
    {
        if (Status == CourseStatus.Hidden)
        {
            return;
        }
        Status = CourseStatus.Hidden;
        
        AddLocalEvent(new CourseHiddenDomainEvent(this));
    }

    public void Reopen()
    {
        if (Status == CourseStatus.Published)
        {
            throw new BusinessException("CourseCatalog:CannotReopenPublishedCourse");
        }

        if (Status == CourseStatus.Draft)
        {
            throw new BusinessException("CourseCatalog:CannotReopenDraft");
        }
        Status = CourseStatus.Published;
        
        AddLocalEvent(new CourseUpdatedDomainEvent(this));
    }
    
    public void UpdateDetails(string title, string? description)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Description = description;
        
        AddLocalEvent(new CourseUpdatedDomainEvent(this));
    }
    
    //Chapter
    public Chapter AddChapter(Guid chapterId, string title)
    {
        if (chapterId == Guid.Empty)
        {
            throw new ArgumentException("Chapter id cannot be empty.", nameof(chapterId));
        }
        var chapter = new Chapter(chapterId, TenantId, Id, title, _chapters.Count + 1);
        _chapters.Add(chapter);
        
        AddLocalEvent(new ChapterCreatedDomainEvent(chapter, Id));
        return chapter;
    }
    
    public void RenameChapter(Guid chapterId, string title)
    {
        var chapter = GetChapter(chapterId);
        chapter.Rename(title);
        AddLocalEvent(new ChapterUpdatedDomainEvent(chapter, Id));
    }

    public void RemoveChapter(Guid chapterId)
    {
        var chapter = _chapters.FirstOrDefault(x => x.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");        
        }
        
        var deletedEvent = new ChapterDeletedDomainEvent(
            chapter.TenantId, Id, chapter.Id, chapter.Title, chapter.OrderNo);
        
        _chapters.Remove(chapter);
        NormalizeChaptersOrder();
        
        AddLocalEvent(deletedEvent);
    }
    
    //Lesson
    public Lesson AddLesson(Guid chapterId, Guid lessonId, string title)
    {
        var chapter = GetChapter(chapterId);
        var lesson  = chapter.AddLesson(lessonId, title);
 
        AddLocalEvent(new LessonCreatedDomainEvent(lesson, Id));
        return lesson;
    }
 
    public void RenameLesson(Guid chapterId, Guid lessonId, string title)
    {
        var (_, lesson) = GetChapterAndLesson(chapterId, lessonId);
        lesson.Rename(title);
        AddLocalEvent(new LessonUpdatedDomainEvent(lesson, Id));
    }
 
    public void UpdateLesson(Guid chapterId, Guid lessonId, string title, LessonContentState contentState)
    {
        var (_, lesson) = GetChapterAndLesson(chapterId, lessonId);
        lesson.UpdateDetails(title, contentState);
        AddLocalEvent(new LessonUpdatedDomainEvent(lesson, Id));
    }
 
    public void RemoveLesson(Guid chapterId, Guid lessonId)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
 
        var deletedEvent = new LessonDeletedDomainEvent(
            lesson.TenantId, Id, chapter.Id, lesson.Id,
            lesson.Title, lesson.SortOrder, lesson.ContentState);
 
        chapter.RemoveLesson(lessonId);
        AddLocalEvent(deletedEvent);
    }
    
    //Material
    public Material AddFileMaterial(
        Guid chapterId, Guid lessonId, Guid materialId,
        string title, string storageKey,
        string? fileName, string? mimeType, long? fileSize)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material = lesson.AddFileMaterial(materialId, title, storageKey, fileName, mimeType, fileSize);
 
        AddLocalEvent(new MaterialCreatedDomainEvent(material, Id, chapter.Id));
        return material;
    }
 
    public Material AddVideoLinkMaterial(
        Guid chapterId, Guid lessonId, Guid materialId, string title, string videoUrl)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material = lesson.AddVideoLinkMaterial(materialId, title, videoUrl);
 
        AddLocalEvent(new MaterialCreatedDomainEvent(material, Id, chapter.Id));
        return material;
    }
 
    public Material AddTextMaterial(
        Guid chapterId, Guid lessonId, Guid materialId,
        string title, string textContent, TextFormat textFormat)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material = lesson.AddTextMaterial(materialId, title, textContent, textFormat);
 
        AddLocalEvent(new MaterialCreatedDomainEvent(material, Id, chapter.Id));
        return material;
    }
 
    public void RenameMaterial(Guid chapterId, Guid lessonId, Guid materialId, string title)
    {
        var (_, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material    = GetMaterial(lesson, materialId);
 
        material.Rename(title);
        AddLocalEvent(new MaterialRenamedDomainEvent(material));
    }
 
    public void UpdateFileMaterial(
        Guid chapterId, Guid lessonId, Guid materialId,
        string storageKey, string? fileName, string? mimeType, long? fileSize)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material          = GetMaterial(lesson, materialId);
 
        material.SetFileContent(storageKey, fileName, mimeType, fileSize);
        AddLocalEvent(new MaterialFileUpdatedDomainEvent(material, Id, chapter.Id));
    }
 
    public void UpdateVideoLinkMaterial(
        Guid chapterId, Guid lessonId, Guid materialId, string externalUrl)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material          = GetMaterial(lesson, materialId);
 
        material.SetVideoLinkContent(externalUrl);
        AddLocalEvent(new MaterialVideoLinkUpdatedDomainEvent(material, Id, chapter.Id));
    }
 
    public void UpdateTextMaterial(
        Guid chapterId, Guid lessonId, Guid materialId,
        string textContent, TextFormat textFormat)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material          = GetMaterial(lesson, materialId);
 
        material.SetTextContent(textContent, textFormat);
        AddLocalEvent(new MaterialTextFormatUpdatedDomainEvent(material, Id, chapter.Id));
    }
 
    public void HideMaterial(Guid chapterId, Guid lessonId, Guid materialId)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        lesson.HideMaterial(materialId);
 
        var material = GetMaterial(lesson, materialId);
        AddLocalEvent(new MaterialHiddenDomainEvent(material, Id, chapter.Id));
    }
 
    public void ActivateMaterial(Guid chapterId, Guid lessonId, Guid materialId)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        lesson.ActivateMaterial(materialId);
 
        var material = GetMaterial(lesson, materialId);
        AddLocalEvent(new MaterialUpdatedDomainEvent(material, Id, chapter.Id));
    }
 
    public void RemoveMaterial(Guid chapterId, Guid lessonId, Guid materialId)
    {
        var (chapter, lesson) = GetChapterAndLesson(chapterId, lessonId);
        var material          = GetMaterial(lesson, materialId);
 
        // Snapshot trước khi remove
        var deletedEvent = new MaterialDeletedDomainEvent(
            material.TenantId, Id, chapter.Id, lesson.Id, material.Id,
            material.Title, material.Type, material.Status, material.SortOrder,
            material.FileName, material.MimeType, material.FileSize,
            material.ExternalUrl, material.TextContent, material.TextFormat);
 
        lesson.RemoveMaterial(materialId);
        AddLocalEvent(deletedEvent);
    }
    
    //Private Helper
    private Chapter GetChapter(Guid chapterId)
        => _chapters.FirstOrDefault(c => c.Id == chapterId)
           ?? throw new BusinessException("CourseCatalog:ChapterNotFound");
 
    private (Chapter, Lesson) GetChapterAndLesson(Guid chapterId, Guid lessonId)
    {
        var chapter = GetChapter(chapterId);
        var lesson  = chapter.Lessons.FirstOrDefault(l => l.Id == lessonId)
                      ?? throw new BusinessException("CourseCatalog:LessonNotFound");
        return (chapter, lesson);
    }
 
    private static Material GetMaterial(Lesson lesson, Guid materialId)
        => lesson.Materials.FirstOrDefault(m => m.Id == materialId)
           ?? throw new BusinessException("CourseCatalog:MaterialNotFound");
    
    private void NormalizeChaptersOrder()
    {
        for (int i = 0; i < _chapters.Count; i++)
        {
            _chapters[i].SetOrder(i + 1);
        }
    }
}
