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
        course.RenameChapter(chapterId, title);
    }
    
    //Lesson 
    public async Task<Lesson> CreateLessonAsync(
        Course course, 
        Guid chapterId,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (chapterId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        var lesson = course.AddLesson(chapterId, GuidGenerator.Create(), title);
        
        return lesson;
    }

    public async Task RemoveLessonAsync(
        Course course, 
        Guid chapterId,
        Guid lessonId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (chapterId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        if (lessonId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyLessonId");

        course.RemoveLesson(chapterId, lessonId);
    }

    public async Task RenameLessonAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (chapterId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        if (lessonId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyLessonId");

        course.RenameLesson(chapterId, lessonId, title);
    }

    public async Task UpdateLessonAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId,
        string title,
        LessonContentState contentState,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (chapterId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        if (lessonId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyLessonId");

        course.UpdateLesson(chapterId, lessonId, title, contentState);
    }
    
    //Material
    public async Task<Material> CreateMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId,
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
        Check.NotNull(course, nameof(course));
        if (chapterId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        if (lessonId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyLessonId");

        return materialType switch
        {
            MaterialType.File =>
                course.AddFileMaterial(chapterId, lessonId, GuidGenerator.Create(),
                    title, storageKey!, fileName, mimeType, fileSize),
            MaterialType.VideoLink =>
                course.AddVideoLinkMaterial(chapterId, lessonId, GuidGenerator.Create(),
                    title, videoUrl!),
            MaterialType.Text =>
                course.AddTextMaterial(chapterId, lessonId, GuidGenerator.Create(),
                    title, textContent!,
                    textFormat ?? throw new BusinessException("CourseCatalog:TextFormatRequiredForTextMaterial")),
            _ => throw new BusinessException("CourseCatalog:UnsupportedMaterialType")
        };
    }

    public async Task RemoveMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.RemoveMaterial(chapterId, lessonId, materialId);
    }

    public async Task HideMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.HideMaterial(chapterId, lessonId, materialId);
    }

    public async Task ActivateMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.ActivateMaterial(chapterId, lessonId, materialId);
    }

    public async Task RenameMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.RenameMaterial(chapterId, lessonId, materialId, title);
    }

    public async Task UpdateFileMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        string storageKey,
        string? fileName,
        string? mimeType,
        long? fileSize,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.UpdateFileMaterial(chapterId, lessonId, materialId, storageKey, fileName, mimeType, fileSize);
    }
    
    public async Task UpdateTextMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        string textContent,
        TextFormat textFormat,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.UpdateTextMaterial(chapterId, lessonId, materialId, textContent, textFormat);
    }
    
    public async Task UpdateVideoLinkMaterialAsync(
        Course course, 
        Guid chapterId, 
        Guid lessonId, 
        Guid materialId,
        string externalUrl,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        if (materialId == Guid.Empty)
            throw new BusinessException("CourseCatalog:EmptyMaterialId");

        course.UpdateVideoLinkMaterial(chapterId, lessonId, materialId, externalUrl);
    }
}