using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Etos.Chapters;
using saasLMS.CourseCatalogService.Etos.Courses;
using saasLMS.CourseCatalogService.Etos.Lessons;
using saasLMS.CourseCatalogService.Etos.Materials;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.CourseCatalogService.Courses;

public class CourseManager : DomainService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IDistributedEventBus _distributedEventBus;

    public CourseManager(ICourseRepository courseRepository, IDistributedEventBus distributedEventBus)
    {
        _courseRepository = courseRepository;
        _distributedEventBus = distributedEventBus;
    }
//Course
    public async Task<Course> CreateAsync(
        Guid tenantId,
        string title,
        string? description,
        Guid instructorId,
        CancellationToken cancellationToken = default)
    {
        var existedCourse = await _courseRepository.FindByTitleAsync(tenantId, title, cancellationToken);
        if (existedCourse != null)
        {
            throw new BusinessException("CourseCatalog:DuplicateCourseTitle").WithData("Title", title);
        }

        var course = new Course(
            GuidGenerator.Create(),
            tenantId,
            title,
            description,
            instructorId);
        await _distributedEventBus.PublishAsync(new CourseCreatedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = course.TenantId,
            CourseId = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            Status = course.Status
            
        });
        return course;
    }

    public async Task RenameAsync(Course course, string newTitle, CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        var existed = await _courseRepository.ExistsByTitleAsync(course.TenantId,
            newTitle,
            course.Id,
            cancellationToken);
        if (existed)
        {
            throw new BusinessException("CourseCatalog:DuplicateCourseTitle").WithData("Title", newTitle);
        }
        course.Rename(newTitle);
    }

    public async Task PublishAsync(
        Course course,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        
        course.Publish();
        await _distributedEventBus.PublishAsync(new CoursePublishedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = course.TenantId,
            CourseId = course.Id,
            InstructorId = course.InstructorId,
            Title = course.Title,
            Description = course.Description,
            Status = course.Status

        });
    }

    public async Task HideAsync(
        Course course,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        
        course.Hide();
        await _distributedEventBus.PublishAsync(new CourseHiddenEto()
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId = course.TenantId,
            CourseId = course.Id,
            Title = course.Title,
            InstructorId = course.InstructorId,
            Description = course.Description,
            Status = course.Status
        });
    }

    public async Task UpdateDetailsAsync(
        Course course,
        string title,
        string? description,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        var existed = await _courseRepository.ExistsByTitleAsync(course.TenantId,
            title,
            course.Id,
            cancellationToken);
        if (existed)
        {
            throw new BusinessException("CourseCatalog:DuplicateCourseTitle").WithData("Title", title);
        }
        course.UpdateDetails(title, description);
        await _distributedEventBus.PublishAsync(new CourseUpdatedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = course.TenantId,
            CourseId = course.Id,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            Status = course.Status
        });
    }
//Chapter
    public async Task<Chapter> CreateChapterAsync(
        Course course,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));

        var chapter = course.AddChapter(GuidGenerator.Create(), title);

        await _distributedEventBus.PublishAsync(new ChapterCreatedEto
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = chapter.TenantId,
            CourseId = course.Id,
            ChapterId = chapter.Id,
            Title = chapter.Title,
            OrderNo = chapter.OrderNo
        });

        return chapter;
    }

    public async Task RemoveChapterAsync(
        Course course,
        Guid chapterId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        var chapter = course.Chapters.FirstOrDefault(x => x.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }

        var tenantId = chapter.TenantId;
        var courseId = course.Id;
        var deletedChapterId = chapter.Id;
        var title = chapter.Title;
        var orderNo = chapter.OrderNo;

        course.RemoveChapter(chapterId);

        await _distributedEventBus.PublishAsync(new ChapterDeletedEto
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = tenantId,
            CourseId = courseId,
            ChapterId = deletedChapterId,
            Title = title,
            OrderNo = orderNo
        });
    }
    public async Task RenameChapterAsync(
        Chapter chapter,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));

        chapter.Rename(title);

        await _distributedEventBus.PublishAsync(new ChapterUpdatedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = chapter.TenantId,
            ChapterId = chapter.Id,
            CourseId = chapter.CourseId,
            Title = chapter.Title,
            OrderNo = chapter.OrderNo
        });
    }
//Lesson 
    public async Task<Lesson> CreateLessonAsync(
        Chapter chapter,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        var lesson = chapter.AddLesson(GuidGenerator.Create(), title);
        await _distributedEventBus.PublishAsync(new LessonCreatedEto
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = lesson.TenantId,
            CourseId = chapter.CourseId,
            ChapterId = chapter.Id,
            LessonId = lesson.Id,
            Title = lesson.Title,
            SortOrder = lesson.SortOrder,
            ContentState = lesson.ContentState
        });
        return lesson;
    }

    public async Task RemoveLessonAsync(
        Chapter chapter,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));

        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        var lesson = chapter.Lessons.FirstOrDefault(x => x.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        var tenantId = lesson.TenantId;
        var courseId = chapter.CourseId;
        var chapterId = chapter.Id;
        var deletedLessonId = lesson.Id;
        var lessonTitle = lesson.Title;
        var sortOrder = lesson.SortOrder;
        var contentState = lesson.ContentState;

        chapter.RemoveLesson(lessonId);

        await _distributedEventBus.PublishAsync(new LessonDeletedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = tenantId,
            CourseId = courseId,
            ChapterId = chapterId,
            LessonId = deletedLessonId,
            Title = lessonTitle,
            SortOrder = sortOrder,
            ContentState = contentState
        });
    }

    public async Task RenameLessonAsync(
        Chapter chapter,
        Lesson lesson,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        lesson.Rename(title);

        await _distributedEventBus.PublishAsync(new LessonUpdatedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = lesson.TenantId,
            CourseId = chapter.CourseId,
            ChapterId = chapter.Id,
            LessonId = lesson.Id,
            Title = lesson.Title,
            SortOrder = lesson.SortOrder,
            ContentState = lesson.ContentState
        });
    }

    public async Task UpdateLessonAsync(
        Lesson lesson,
        Chapter chapter,
        string title,
        LessonContentState contentState,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));
        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }
        lesson.UpdateDetails(title, contentState);
        await _distributedEventBus.PublishAsync(new LessonUpdatedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = lesson.TenantId,
            CourseId = chapter.CourseId,
            ChapterId = chapter.Id,
            LessonId = lesson.Id,
            Title = lesson.Title,
            SortOrder = lesson.SortOrder,
            ContentState = lesson.ContentState
        });
    }
    
//Material
    public async Task<Material> CreateMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        string title,
        MaterialType materialType,
        string? storageKey = null,
        string? fileName = null,
        string? mimeType = null,
        long? fileSize = null,
        string? videoUrl = null,
        string? textContent = null,
        TextFormat? textFormat = null,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        Material material = materialType switch
        {
            MaterialType.File =>
                lesson.AddFileMaterial(GuidGenerator.Create(), title, storageKey!, fileName, mimeType, fileSize),
            MaterialType.VideoLink =>
                lesson.AddVideoLinkMaterial(GuidGenerator.Create(), title, videoUrl!),
            MaterialType.Text =>
                lesson.AddTextMaterial(GuidGenerator.Create(), title, textContent!, textFormat ??
                    throw new BusinessException("CourseCatalog:TextFormatRequiredForTextMaterial")),
            _ => throw new BusinessException("CourseCatalog:UnsupportedMaterialType")
        };

        await _distributedEventBus.PublishAsync(new MaterialCreatedEto
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = material.TenantId,
            CourseId = chapter.CourseId,
            ChapterId = chapter.Id,
            LessonId = lesson.Id,
            MaterialId = material.Id,
            Title = material.Title,
            Type = material.Type,
            Status = material.Status,
            SortOrder = material.SortOrder,
            FileName = material.FileName,
            MimeType = material.MimeType,
            FileSize = material.FileSize,
            ExternalUrl = material.ExternalUrl,
            TextFormat = material.TextFormat
        });

        return material;
    }

    public async Task RemoveMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        Guid materialId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        if (materialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        var tenantId = material.TenantId;
        var courseId = chapter.CourseId;
        var chapterId = chapter.Id;
        var currentLessonId = lesson.Id;
        var deletedMaterialId = material.Id;
        var title = material.Title;
        var type = material.Type;
        var status = material.Status;
        var sortOrder = material.SortOrder;
        var fileName = material.FileName;
        var mimeType = material.MimeType;
        var fileSize = material.FileSize;
        var externalUrl = material.ExternalUrl;
        var textFormat = material.TextFormat;

        lesson.RemoveMaterial(materialId);

        await _distributedEventBus.PublishAsync(new MaterialDeletedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = tenantId,
            CourseId = courseId,
            ChapterId = chapterId,
            LessonId = currentLessonId,
            MaterialId = deletedMaterialId,
            Title = title,
            Type = type,
            Status = status,
            SortOrder = sortOrder,
            FileName = fileName,
            MimeType = mimeType,
            FileSize = fileSize,
            ExternalUrl = externalUrl,
            TextFormat = textFormat
        });
    }

    public async Task HideMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        Guid materialId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        if (materialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        lesson.HideMaterial(materialId);

        await _distributedEventBus.PublishAsync(new MaterialHiddenEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = material.TenantId,
            CourseId = chapter.CourseId,
            ChapterId = chapter.Id,
            LessonId = lesson.Id,
            MaterialId = material.Id,
            Title = material.Title,
            Type = material.Type,
            Status = material.Status,
            SortOrder = material.SortOrder,
            FileName = material.FileName,
            MimeType = material.MimeType,
            FileSize = material.FileSize,
            ExternalUrl = material.ExternalUrl,
            TextFormat = material.TextFormat
        });
    }

    public async Task ActivateMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        Guid materialId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        if (materialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        lesson.ActivateMaterial(materialId);

        await _distributedEventBus.PublishAsync(new MaterialUpdatedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = material.TenantId,
            CourseId = chapter.CourseId,
            ChapterId = chapter.Id,
            LessonId = lesson.Id,
            MaterialId = material.Id,
            Title = material.Title,
            Type = material.Type,
            Status = material.Status,
            SortOrder = material.SortOrder,
            FileName = material.FileName,
            MimeType = material.MimeType,
            FileSize = material.FileSize,
            ExternalUrl = material.ExternalUrl,
            TextFormat = material.TextFormat
        });
    }

    public async Task RenameMaterialAsync(
        Material material,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(material, nameof(material));
        material.Rename(title);
        await _distributedEventBus.PublishAsync(new MaterialRenamedEto()
        {
            EventId = GuidGenerator.Create(),
            OccurredAt = DateTime.UtcNow,
            TenantId = material.TenantId,
            LessonId = material.LessonId,
            MaterialId = material.Id,
            Title = material.Title,
            Type = material.Type,
            Status = material.Status,
            SortOrder = material.SortOrder,
            FileName = material.FileName,
            MimeType = material.MimeType,
            FileSize = material.FileSize,
            ExternalUrl = material.ExternalUrl,
            TextFormat = material.TextFormat
        });
    }
}