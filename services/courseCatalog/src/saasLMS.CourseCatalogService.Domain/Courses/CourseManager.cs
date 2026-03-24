using System;
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
        return new Course(GuidGenerator.Create(), tenantId, title, description, instructorId);
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

    public Task<Chapter> CreateChapterAsync(
        Course course,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(course, nameof(course));
        var chapter = course.AddChapter(GuidGenerator.Create(), title);
        return Task.FromResult(chapter);
    }

    public Task<Lesson> CreateLessonAsync(
        Chapter chapter,
        string title,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(chapter, nameof(chapter));
        var lesson = chapter.AddLesson(GuidGenerator.Create(), title);
        return Task.FromResult(lesson);
    }

    public Task<Material> CreateMaterialAsync(
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
        Check.NotNull(lesson, nameof(lesson));
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
        return Task.FromResult(material);
    }
}