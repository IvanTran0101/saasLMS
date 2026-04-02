using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.CourseCatalogService.Courses;

public class CourseManager : DomainService
{
    private readonly ICourseRepository _courseRepository;

    public CourseManager(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
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
    }

    public async Task HideAsync(
        Course course,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        
        course.Hide();
    }

    public async Task ReopenCourseAsync(
        Course course,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        course.Reopen();
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
    }
    //Chapter
    public async Task<Chapter> CreateChapterAsync(
        Course course,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));

        var chapter = course.AddChapter(GuidGenerator.Create(), title);

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
        
        course.RemoveChapter(chapterId);
    }
    
    public async Task RenameChapterAsync(
        Course course,
        Guid chapterId,
        string title,
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
        chapter.Rename(title);
    }
    
    //Lesson 
    public async Task<Lesson> CreateLessonAsync(
        Chapter chapter,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        var lesson = chapter.AddLesson(GuidGenerator.Create(), title);
        
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

        chapter.RemoveLesson(lessonId);
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
    }

    public async Task UpdateLessonAsync(
        Chapter chapter,
        Lesson lesson,
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

        lesson.RemoveMaterial(materialId);
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
    }

    public async Task RenameMaterialAsync(
        Material material,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(material, nameof(material));
        material.Rename(title);
    }

    public async Task UpdateFileMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        Material material,
        string storageKey,
        string? fileName,
        string? mimeType,
        long? fileSize,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));
        Check.NotNull(material, nameof(material));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        if (material.LessonId != lesson.Id)
        {
            throw new BusinessException("CourseCatalog:MaterialDoesNotBelongToLesson");
        }

        material.SetFileContent(storageKey, fileName, mimeType, fileSize);
    }
    
    public async Task UpdateTextMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        Material material,
        string textContent,
        TextFormat textFormat,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));
        Check.NotNull(material, nameof(material));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        if (material.LessonId != lesson.Id)
        {
            throw new BusinessException("CourseCatalog:MaterialDoesNotBelongToLesson");
        }

        material.SetTextContent(textContent, textFormat);
    }
    
    public async Task UpdateVideoLinkMaterialAsync(
        Chapter chapter,
        Lesson lesson,
        Material material,
        string externalUrl,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        Check.NotNull(lesson, nameof(lesson));
        Check.NotNull(material, nameof(material));

        if (lesson.ChapterId != chapter.Id)
        {
            throw new BusinessException("CourseCatalog:LessonDoesNotBelongToChapter");
        }

        if (material.LessonId != lesson.Id)
        {
            throw new BusinessException("CourseCatalog:MaterialDoesNotBelongToLesson");
        }

        material.SetVideoLinkContent(externalUrl);
    }
}