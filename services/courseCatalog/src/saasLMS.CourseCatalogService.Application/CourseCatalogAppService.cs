using System;
using System.Collections.Generic;
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
using Volo.Abp;

namespace saasLMS.CourseCatalogService;

public class CourseCatalogAppService : CourseCatalogServiceAppService, ICourseCatalogAppService
{
    private readonly ICourseRepository _courseRepository;
    private readonly CourseManager _courseManager;

    public CourseCatalogAppService(ICourseRepository courseRepository, CourseManager courseManager)
    {
        _courseRepository = courseRepository;
        _courseManager = courseManager;
    }
    public async Task<CourseDto> CreateCourseAsync(CreateCourseInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

        var course = await _courseManager.CreateAsync(
            tenantId.Value,
            input.Title,
            input.Description,
            input.InstructorId);
        
        await _courseRepository.InsertAsync(course, autoSave: true);
        return ObjectMapper.Map<Course,  CourseDto>(course);
    }

    public async Task<CourseDto> RenameCourseAsync(RenameCourseInput input)
    {
        Check.NotNull(input, nameof(input));
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
        await _courseManager.RenameAsync(course, input.NewTitle);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    public async Task<CourseDto> UpdateCourseAsync(UpdateCourseInput input)
    {
        Check.NotNull(input, nameof(input));
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
        await _courseManager.UpdateDetailsAsync(course, input.Title, input.Description);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

    public async Task PublishCourseAsync(PublishCourseInput input)
    {
        Check.NotNull(input, nameof(input));
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
        await _courseManager.PublishAsync(course);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    public async Task HideCourseAsync(HideCourseInput input)
    {
        Check.NotNull(input, nameof(input));
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
        await _courseManager.HideAsync(course);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

    public async Task ReopenCourseAsync(ReopenCourseInput input)
    {
        Check.NotNull(input, nameof(input));
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
        await _courseManager.ReopenCourseAsync(course);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

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
        return ObjectMapper.Map<Course, CourseDto>(course);
    }

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
        return ObjectMapper.Map<Course, CourseDetailDto>(course);
    }

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

    public async Task<ChapterDto> CreateChapterAsync(CreateChapterInput input)
    {
        Check.NotNull(input, nameof(input));
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

        var chapter = await _courseManager.CreateChapterAsync(course, input.Title);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        return ObjectMapper.Map<Chapter, ChapterDto>(chapter);
    }

    public async Task<ChapterDto> RenameChapterAsync(RenameChapterInput input)
    {
        Check.NotNull(input, nameof(input));
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
        await _courseManager.RenameChapterAsync(course, input.ChapterId, input.NewTitle);
        await _courseRepository.UpdateAsync(course, autoSave: true);
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == input.ChapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        return ObjectMapper.Map<Chapter,ChapterDto>(chapter);
    }

    public async Task RemoveChapterAsync(RemoveChapterInput input)
    {
        Check.NotNull(input, nameof(input));
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
        
        await _courseManager.RemoveChapterAsync(course, input.ChapterId);
        await _courseRepository.UpdateAsync(course, autoSave: true);
    }

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
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        return ObjectMapper.Map<Chapter, ChapterDto>(chapter);
    }

    public async Task<LessonDto> CreateLessonAsync(CreateLessonInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task<LessonDto> RenameLessonAsync(RenameLessonInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
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

    public async Task<LessonDto> UpdateLessonAsync(UpdateLessonInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
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

    public async Task  RemoveLessonAsync(RemoveLessonInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }
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
        var chapter = course.Chapters.FirstOrDefault(c => c.Id == chapterId);
        if (chapter == null)
        {
            throw new BusinessException("CourseCatalog:ChapterNotFound");
        }
        
        return chapter.Lessons.Select(lesson => MapLessonDetail(lesson, courseId, chapterId)).ToList();
    }

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

    public async Task<MaterialDto> CreateFileMaterialAsync(CreateFileMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task<MaterialDto> CreateVideoLinkMaterialAsync(CreateVideoLinkMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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
    

    public async Task<MaterialDto> CreateTextMaterialAsync(CreateTextMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task<MaterialDto> RenameMaterialAsync(RenameMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task<MaterialDto> UpdateFileMaterialAsync(UpdateFileMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task<MaterialDto> UpdateVideoLinkMaterialAsync(UpdateVideoLinkMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task<MaterialDto> UpdateTextMaterialAsync(UpdateTextMaterialInput input)
    {
        // This method generated by Codex
        Check.NotNull(input, nameof(input));

        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task HideMaterialAsync(HideMaterialInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task ActivateMaterialAsync(ActivateMaterialInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    public async Task RemoveMaterialAsync(RemoveMaterialInput input)
    {
        Check.NotNull(input, nameof(input));
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("CourseCatalog:TenantNotFound");
        }

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

    private MaterialDto MapMaterialDetail(Material material, Guid courseId, Guid chapterId)
    {
        // This method generated by Codex
        var dto = ObjectMapper.Map<Material, MaterialDto>(material);
        dto.CourseId = courseId;
        dto.ChapterId = chapterId;
        return dto;
    }
}