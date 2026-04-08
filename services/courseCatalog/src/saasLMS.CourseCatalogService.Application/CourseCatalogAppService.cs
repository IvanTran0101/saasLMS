using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;
using saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Inputs;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using saasLMS.CourseCatalogService.Permissions;
using Microsoft.AspNetCore.Authorization;
using saasLMS.CourseCatalogService.BlobStoring;
using Volo.Abp.Authorization;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp;

namespace saasLMS.CourseCatalogService;

[RemoteService(Name = CourseCatalogServiceRemoteServiceConsts.RemoteServiceName)]
public class CourseCatalogAppService : CourseCatalogServiceAppService, ICourseCatalogAppService
{
    private readonly ICourseRepository _courseRepository;
    private readonly CourseManager _courseManager;
    private readonly ICourseAccessChecker _courseAccessChecker;
    private readonly ICourseEnrollmentChecker _courseEnrollmentChecker;
    private readonly IBlobContainer<CourseMaterialContainer> _blobContainer;

    public CourseCatalogAppService(
        ICourseRepository courseRepository,
        CourseManager courseManager,
        ICourseAccessChecker courseAccessChecker,
        ICourseEnrollmentChecker courseEnrollmentChecker,
        IBlobContainer<CourseMaterialContainer> blobContainer)
    {
        _courseRepository = courseRepository;
        _courseManager = courseManager;
        _courseAccessChecker = courseAccessChecker;
        _courseEnrollmentChecker = courseEnrollmentChecker;
        _blobContainer = blobContainer;
    }
    private const long MaxMaterialFileSize = 20 * 1024 * 1024; // 20MB

    private static readonly Dictionary<string, HashSet<string>> AllowedFileTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".pptx"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/vnd.openxmlformats-officedocument.presentationml.presentation"
        },
        [".doc"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/msword"
        },
        [".zip"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "application/zip",
            "application/x-zip-compressed"
        }
    };

    private static void ValidateMaterialUpload(string fileName, string? contentType, long? contentLength)
    {
        if (contentLength <= 0)
        {
            throw new BusinessException("CourseCatalog:FileEmpty");
        }

        if (contentLength > MaxMaterialFileSize)
        {
            throw new BusinessException("CourseCatalog:FileTooLarge")
                .WithData("MaxBytes", MaxMaterialFileSize);
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedFileTypes.ContainsKey(extension))
        {
            throw new BusinessException("CourseCatalog:FileTypeNotAllowed")
                .WithData("Extension", extension ?? string.Empty);
        }

        if (!string.IsNullOrWhiteSpace(contentType) &&
            !AllowedFileTypes[extension].Contains(contentType))
        {
            throw new BusinessException("CourseCatalog:MimeTypeNotAllowed")
                .WithData("MimeType", contentType ?? string.Empty);
        }
    }
    
    [Authorize(CourseCatalogServicePermissions.Courses.Create)]
    public async Task<CourseDto> CreateCourseAsync(CreateCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var userId = CurrentUser.Id;
        if (!userId.HasValue)
        {
            throw new AbpAuthorizationException("You must be logged in to create a course.");
        }

        var course = await _courseManager.CreateAsync(
            tenantId.Value,
            input.Title,
            input.Description,
            userId.Value);
        
        await _courseRepository.InsertAsync(course, autoSave: true);
        return ObjectMapper.Map<Course,  CourseDto>(course);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.Update)]
    public async Task<CourseDto> RenameCourseAsync(RenameCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseManager.RenameAsync(course, input.NewTitle);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.Update)]
    public async Task<CourseDto> UpdateCourseAsync(UpdateCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseManager.UpdateDetailsAsync(course, input.Title, input.Description);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.Publish)]
    public async Task PublishCourseAsync(PublishCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseManager.PublishAsync(course);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.Hide)]
    public async Task HideCourseAsync(HideCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseManager.HideAsync(course);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.Reopen)]
    public async Task ReopenCourseAsync(ReopenCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseManager.ReopenCourseAsync(course);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.View)]
    public async Task<CourseDto> GetCourseAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(id, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ViewPublished)]
    public async Task<CourseDto> GetCourseStudentAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(id, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.GetOwner)]
    public async Task<CourseOwnerDto> GetOwnerAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(id, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        return new CourseOwnerDto
        {
            InstructorId = course.InstructorId
        };
    }

    [Authorize(CourseCatalogServicePermissions.Courses.CheckEligibility)]
    public async Task<CourseEligibilityDto?> GetEnrollmentEligibilityAsync(Guid courseId, Guid tenantId)
    {
        if (courseId == Guid.Empty || tenantId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }

        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId);
        if (course == null)
        {
            return null;
        }

        return new CourseEligibilityDto
        {
            CourseId = course.Id,
            TenantId = course.TenantId,
            Status = course.Status,
            IsHidden = course.Status == CourseStatus.Hidden
        };
    }

    [Authorize(CourseCatalogServicePermissions.Courses.View)]
    public async Task<CourseDetailDto> GetCourseDetailAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(id, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        return MapCourseDetail(course);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ViewPublished)]
    public async Task<CourseDetailDto> GetCourseDetailStudentAsync(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(id, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var dto = MapCourseDetail(course);
        FilterMaterialsForStudent(dto);
        return dto;
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ViewPublished)]
    public async Task<List<CourseListItemDto>> GetPublishedCoursesAsync()
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var courses = await _courseRepository.GetPublishedListAsync(tenantId.Value);
        

        return ObjectMapper.Map<List<Course>, List<CourseListItemDto>>(courses);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ListByInstructor)]
    public async Task<List<CourseListItemDto>> GetCoursesByInstructorAsync(Guid instructorId)
    {
        if (instructorId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:InstructorNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var courses = await _courseRepository.GetListByInstructorAsync(tenantId.Value,instructorId);
        return ObjectMapper.Map<List<Course>, List<CourseListItemDto>>(courses);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ViewPublished)]
    public async Task<List<CourseListItemDto>> GetPublishedCoursesByInstructorAsync(Guid instructorId)
    {
        if (instructorId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:InstructorNotFound");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var courses = await _courseRepository.GetListByInstructorAsync(tenantId.Value, instructorId);
        courses = courses
            .Where(c => c.Status == CourseStatus.Published)
            .ToList();
        return ObjectMapper.Map<List<Course>, List<CourseListItemDto>>(courses);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ListByTenant)]
    public async Task<List<CourseListItemDto>> GetCoursesByTenantAsync()
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var courses = await _courseRepository.GetListByTenantAsync(tenantId.Value);
        return ObjectMapper.Map<List<Course>, List<CourseListItemDto>>(courses);
    }

    [Authorize(CourseCatalogServicePermissions.Courses.ViewPublished)]
    public async Task<List<CourseListItemDto>> GetPublishedCoursesByTenantAsync()
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var courses = await _courseRepository.GetPublishedListAsync(tenantId.Value);
        return ObjectMapper.Map<List<Course>, List<CourseListItemDto>>(courses);
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.Create)]
    public async Task<ChapterDto> CreateChapterAsync(CreateChapterInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }

        var chapter = await _courseManager.CreateChapterAsync(course, input.Title);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return ObjectMapper.Map<Chapter, ChapterDto>(chapter);
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.Update)]
    public async Task<ChapterDto> RenameChapterAsync(RenameChapterInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        } 
        await _courseManager.RenameChapterAsync(course, input.ChapterId, input.NewTitle);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        return ObjectMapper.Map<Chapter,ChapterDto>(chapter);
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.Delete)]
    public async Task RemoveChapterAsync(RemoveChapterInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        
        await _courseManager.RemoveChapterAsync(course, input.ChapterId);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.View)]
    public async Task<ChapterDto> GetChapterAsync(Guid courseId, Guid chapterId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        return ObjectMapper.Map<Chapter, ChapterDto>(chapter);
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.View)]
    public async Task<List<ChapterDto>> GetChaptersByCourseAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        return ObjectMapper.Map<List<Chapter>, List<ChapterDto>>(course.Chapters.ToList());
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.ViewPublished)]
    public async Task<List<ChapterDto>> GetChaptersByCourseStudentAsync(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var chapters = ObjectMapper.Map<List<Chapter>, List<ChapterDto>>(course.Chapters.ToList());
        foreach (var chapter in chapters)
        {
            FilterMaterialsForStudent(chapter);
        }
        return chapters;
    }

    [Authorize(CourseCatalogServicePermissions.Chapters.ViewPublished)]
    public async Task<ChapterDto> GetChapterStudentAsync(Guid courseId, Guid chapterId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var dto = ObjectMapper.Map<Chapter, ChapterDto>(chapter);
        FilterMaterialsForStudent(dto);
        return dto;
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.Create)]
    public async Task<LessonDto> CreateLessonAsync(CreateLessonInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }
        var course =  await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = await _courseManager.CreateLessonAsync(course, input.ChapterId, input.Title);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapLessonDetail(lesson, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.Update)]
    public async Task<LessonDto> RenameLessonAsync(RenameLessonInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        await _courseManager.RenameLessonAsync(course, input.ChapterId, input.LessonId, input.NewTitle);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapLessonDetail(lesson, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.Update)]
    public async Task<LessonDto> UpdateLessonAsync(UpdateLessonInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        await _courseManager.UpdateLessonAsync(course, input.ChapterId, input.LessonId, input.Title, input.ContentState);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapLessonDetail(lesson, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.Delete)]
    public async Task  RemoveLessonAsync(RemoveLessonInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        await _courseManager.RemoveLessonAsync(course, input.ChapterId, input.LessonId);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.View)]
    public async Task<List<LessonDto>> GetLessonsByChapterAsync(Guid courseId, Guid chapterId)
    {
        // This method generated by Codex
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await  _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        
        return chapter.Lessons.Select(lesson => MapLessonDetail(lesson, courseId, chapterId)).ToList();
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.View)]
    public async Task<LessonDto> GetLessonAsync(Guid courseId,Guid chapterId, Guid lessonId)
    {
        // This method generated by Codex
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        return MapLessonDetail(lesson, courseId, chapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.ViewPublished)]
    public async Task<List<LessonDto>> GetLessonsByChapterStudentAsync(Guid courseId, Guid chapterId)
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lessons = chapter.Lessons.Select(l => MapLessonDetail(l, courseId, chapterId)).ToList();
        foreach (var lesson in lessons)
        {
            FilterMaterialsForStudent(lesson);
        }
        return lessons;
    }

    [Authorize(CourseCatalogServicePermissions.Lessons.ViewPublished)]
    public async Task<LessonDto> GetLessonStudentAsync(Guid courseId, Guid chapterId, Guid lessonId)
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var dto = MapLessonDetail(lesson, courseId, chapterId);
        FilterMaterialsForStudent(dto);
        return dto;
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Create)]
    public async Task<MaterialDto> CreateFileMaterialAsync(CreateFileMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var material = await _courseManager.CreateMaterialAsync(course, input.ChapterId, input.LessonId, input.Title, MaterialType.File, input.StorageKey,
            input.FileName, input.MimeType, input.FileSize);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Create)]
    public async Task<MaterialDto> CreateVideoLinkMaterialAsync(CreateVideoLinkMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var material = await _courseManager.CreateMaterialAsync(course, input.ChapterId, input.LessonId, input.Title, MaterialType.VideoLink,null, null, null, null, input.ExternalUrl);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }
    

    [Authorize(CourseCatalogServicePermissions.Materials.Create)]
    public async Task<MaterialDto> CreateTextMaterialAsync(CreateTextMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var material = await _courseManager.CreateMaterialAsync(course, input.ChapterId, input.LessonId, input.Title, MaterialType.Text,null, null, null, null, null,input.Content, input.Format);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Update)]
    public async Task<MaterialDto> RenameMaterialAsync(RenameMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(c => c.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == input.MaterialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }
        await _courseManager.RenameMaterialAsync(course, input.ChapterId, input.LessonId, input.MaterialId, input.NewTitle);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Update)]
    public async Task<MaterialDto> UpdateFileMaterialAsync(UpdateFileMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (input.MaterialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }

        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }

        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }

        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == input.MaterialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        await _courseManager.UpdateFileMaterialAsync(
            course, input.ChapterId, input.LessonId, input.MaterialId,
            input.StorageKey,
            input.FileName,
            input.MimeType,
            input.FileSize);

        await _courseRepository.UpdateAsync(course, autoSave: true);

        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Update)]
    public async Task<MaterialDto> UpdateVideoLinkMaterialAsync(UpdateVideoLinkMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (input.MaterialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }

        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }

        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }

        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == input.MaterialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        await _courseManager.UpdateVideoLinkMaterialAsync(
            course, input.ChapterId, input.LessonId, input.MaterialId,
            input.ExternalUrl);
        
        await _courseRepository.UpdateAsync(course, autoSave: true);

        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Update)]
    public async Task<MaterialDto> UpdateTextMaterialAsync(UpdateTextMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (input.MaterialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }

        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }

        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }

        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }

        var material = lesson.Materials.FirstOrDefault(m => m.Id == input.MaterialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        await _courseManager.UpdateTextMaterialAsync(
            course, input.ChapterId, input.LessonId, input.MaterialId,
            input.Content,
            input.Format);

        await _courseRepository.UpdateAsync(course, autoSave: true);

        return MapMaterialDetail(material, input.CourseId, input.ChapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Hide)]
    public async Task HideMaterialAsync(HideMaterialInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (input.MaterialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        
        await _courseManager.HideMaterialAsync(course, input.ChapterId, input.LessonId, input.MaterialId);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Activate)]
    public async Task ActivateMaterialAsync(ActivateMaterialInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (input.MaterialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        
        await _courseManager.ActivateMaterialAsync(course, input.ChapterId, input.LessonId, input.MaterialId);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Delete)]
    public async Task RemoveMaterialAsync(RemoveMaterialInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        if (input.CourseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (input.ChapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (input.LessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (input.MaterialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        var  chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        await _courseManager.RemoveMaterialAsync(course, input.ChapterId, input.LessonId, input.MaterialId);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.View)]
    public async Task<List<MaterialDto>> GetMaterialsByLessonAsync(Guid courseId, Guid chapterId, Guid lessonId)
    {
        // This method generated by Codex
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        return lesson.Materials.Select(material => MapMaterialDetail(material, courseId, chapterId)).ToList();
    }

    [Authorize(CourseCatalogServicePermissions.Materials.View)]
    public async Task<MaterialDto> GetMaterialAsync(Guid courseId,Guid chapterId, Guid lessonId, Guid materialId)
    {
        // This method generated by Codex
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (materialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        await _courseAccessChecker.CheckCanManageCourseAsync(course.Id);
        var  chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var material = lesson.Materials.FirstOrDefault(m => m.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }
        return MapMaterialDetail(material, courseId, chapterId);
    }

    [Authorize(CourseCatalogServicePermissions.Materials.ViewPublished)]
    public async Task<List<MaterialDto>> GetMaterialsByLessonStudentAsync(Guid courseId, Guid chapterId, Guid lessonId)
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }

        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        return lesson.Materials
            .Where(m => m.Status == MaterialStatus.Active)
            .Select(material => MapMaterialDetail(material, courseId, chapterId))
            .ToList();
    }

    [Authorize(CourseCatalogServicePermissions.Materials.ViewPublished)]
    public async Task<MaterialDto> GetMaterialStudentAsync(Guid courseId, Guid chapterId, Guid lessonId, Guid materialId)
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

        if (courseId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyCourseId");
        }

        if (chapterId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyChapterId");
        }
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyLessonId");
        }

        if (materialId == Guid.Empty)
        {
            throw new BusinessException("CourseCatalog:EmptyMaterialId");
        }
        var course = await _courseRepository.FindWithDetailsAsync(courseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var material = lesson.Materials.FirstOrDefault(m => m.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }
        if (material.Status != MaterialStatus.Active)
        {
            throw new BusinessException("CourseCatalog:MaterialNotAvailable");
        }
        return MapMaterialDetail(material, courseId, chapterId);
    }

    private LessonDto MapLessonDetail(Lesson lesson, Guid courseId, Guid chapterId)
    {
        // This method generated by Codex
        var dto = ObjectMapper.Map<Lesson, LessonDto>(lesson);
        dto.CourseId = courseId;
        dto.ChapterId = chapterId;
        foreach (var material in dto.Materials)
        {
            material.CourseId = courseId;
            material.ChapterId = chapterId;
        }

        return dto;
    }

    private static void EnsureCoursePublished(Course course)
    {
        if (course.Status != CourseStatus.Published)
        {
            throw new BusinessException("CourseCatalog:CourseNotPublished")
                .WithData("CourseId", course.Id)
                .WithData("Status", course.Status);
        }
    }

    private static void FilterMaterialsForStudent(LessonDto lesson)
    {
        lesson.Materials = lesson.Materials
            .Where(m => m.Status == MaterialStatus.Active)
            .ToList();
    }

    private static void FilterMaterialsForStudent(LessonInChapterDto lesson)
    {
        lesson.Materials = lesson.Materials
            .Where(m => m.Status == MaterialStatus.Active)
            .ToList();
    }

    private static void FilterMaterialsForStudent(ChapterDto chapter)
    {
        foreach (var lesson in chapter.Lessons)
        {
            FilterMaterialsForStudent(lesson);
        }
    }

    private static void FilterMaterialsForStudent(CourseDetailDto course)
    {
        foreach (var chapter in course.Chapters)
        {
            FilterMaterialsForStudent(chapter);
        }
    }

    private CourseDetailDto MapCourseDetail(Course course)
    {
        var dto = ObjectMapper.Map<Course, CourseDetailDto>(course);

        var chapterDtos = new List<ChapterDto>();
        foreach (var chapter in course.Chapters.OrderBy(c => c.OrderNo))
        {
            var chapterDto = ObjectMapper.Map<Chapter, ChapterDto>(chapter);
            chapterDto.CourseId = course.Id;

            var lessonDtos = new List<LessonInChapterDto>();
            foreach (var lesson in chapter.Lessons.OrderBy(l => l.SortOrder))
            {
                var lessonDto = ObjectMapper.Map<Lesson, LessonInChapterDto>(lesson);
                lessonDto.ChapterId = chapter.Id;

                var materialDtos = new List<MaterialInLessonDto>();
                foreach (var material in lesson.Materials.OrderBy(m => m.SortOrder))
                {
                    var materialDto = ObjectMapper.Map<Material, MaterialInLessonDto>(material);
                    materialDto.LessonId = lesson.Id;
                    materialDtos.Add(materialDto);
                }

                lessonDto.Materials = materialDtos;
                lessonDtos.Add(lessonDto);
            }

            chapterDto.Lessons = lessonDtos;
            chapterDtos.Add(chapterDto);
        }

        dto.Chapters = chapterDtos;
        return dto;
    }

    private MaterialDto MapMaterialDetail(Material material, Guid courseId, Guid chapterId)
    {
        // This method generated by Codex
        var dto = ObjectMapper.Map<Material, MaterialDto>(material);
        dto.CourseId = courseId;
        dto.ChapterId = chapterId;
        return dto;
    }

    [Authorize(CourseCatalogServicePermissions.Materials.Update)]
    [RemoteService(false)]
    public async Task<MaterialDto> UploadMaterialFileAsync(UploadMaterialFileInput input, IRemoteStreamContent file)
    {
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);
        if (file == null || file.ContentLength <= 0 || string.IsNullOrWhiteSpace(file.FileName))
            throw new BusinessException("CourseCatalog:FileEmpty");
        var safeFileName = Path.GetFileName(file.FileName);
        ValidateMaterialUpload(safeFileName, file.ContentType, file.ContentLength);

        var existingMaterial = await GetMaterialAsync(
            input.CourseId, input.ChapterId, input.LessonId, input.MaterialId);
        if (!string.IsNullOrWhiteSpace(existingMaterial.StorageKey))
        {
            await _blobContainer.DeleteAsync(existingMaterial.StorageKey);
        }

        var storageKey = $"materials/{input.CourseId}/{input.MaterialId}/{safeFileName}";
        await _blobContainer.SaveAsync(storageKey, file.GetStream());
        await UpdateFileMaterialAsync(new UpdateFileMaterialInput()
        {
            CourseId = input.CourseId,
            ChapterId = input.ChapterId,
            LessonId = input.LessonId,
            MaterialId = input.MaterialId,
            StorageKey = storageKey,
            FileName = safeFileName,
            MimeType = file.ContentType,
            FileSize = file.ContentLength
        });
        return await GetMaterialAsync(input.CourseId, input.ChapterId, input.LessonId, input.MaterialId);
    }
    [Authorize(CourseCatalogServicePermissions.Materials.View)]
    public async Task<IRemoteStreamContent> DownloadMaterialFileAsync(DownloadMaterialFileInput input)
    {
        // check permission or enrollment
        await _courseAccessChecker.CheckCanManageCourseAsync(input.CourseId);

        var material = await GetMaterialAsync(input.CourseId, input.ChapterId, input.LessonId, input.MaterialId);

        // load blob
        if (string.IsNullOrWhiteSpace(material.StorageKey))
        {
            throw new BusinessException("CourseCatalog:MaterialFileNotFound");
        }
        
        var stream = await _blobContainer.GetAsync(material.StorageKey);
        if (stream == null)
        {
            throw new BusinessException("CourseCatalog:MaterialFileNotFound");
        }

        return new RemoteStreamContent(
            stream,
            material.FileName,
            material.MimeType
        );
    }

    [Authorize(CourseCatalogServicePermissions.Materials.DownloadPublished)]
    public async Task<IRemoteStreamContent> DownloadMaterialFileStudentAsync(DownloadMaterialFileInput input)
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

        var course = await _courseRepository.FindWithDetailsAsync(input.CourseId, tenantId.Value);
        if (course == null)
        {
            throw new BusinessException("CourseCatalog:CourseNotFound");
        }
        EnsureCoursePublished(course);
        await _courseEnrollmentChecker.CheckStudentEnrolledAsync(course.Id);

        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        var lesson = chapter.Lessons.FirstOrDefault(l => l.Id == input.LessonId);
        if (lesson == null)
        {
            throw new BusinessException("CourseCatalog:LessonNotFound");
        }
        var material = lesson.Materials.FirstOrDefault(m => m.Id == input.MaterialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }
        if (material.Status != MaterialStatus.Active)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }
        if (string.IsNullOrWhiteSpace(material.StorageKey))
        {
            throw new BusinessException("CourseCatalog:MaterialFileNotFound");
        }

        var stream = await _blobContainer.GetAsync(material.StorageKey);
        if (stream == null)
        {
            throw new BusinessException("CourseCatalog:MaterialFileNotFound");
        }

        return new RemoteStreamContent(
            stream,
            material.FileName,
            material.MimeType
        );
    }
}
