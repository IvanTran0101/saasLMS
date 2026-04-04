using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;
using saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;
using saasLMS.CourseCatalogService.Courses.Dtos.Inputs;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace saasLMS.CourseCatalogService.Courses;

public interface ICourseCatalogAppService : IApplicationService
{
    //Courses
    Task<CourseDto> CreateCourseAsync(CreateCourseInput input);
    Task<CourseDto> RenameCourseAsync(RenameCourseInput input);
    Task<CourseDto> UpdateCourseAsync(UpdateCourseInput input);
    Task PublishCourseAsync(PublishCourseInput input);
    Task HideCourseAsync(HideCourseInput input);
    Task ReopenCourseAsync(ReopenCourseInput input);
    
    Task<CourseDto> GetCourseAsync(Guid id);
    Task<CourseDetailDto> GetCourseDetailAsync(Guid id);
    Task<List<CourseListItemDto>> GetPublishedCoursesAsync();
    Task<List<CourseListItemDto>> GetCoursesByInstructorAsync(Guid instructorId);
    Task<List<CourseListItemDto>> GetCoursesByTenantAsync();
    
    //Chapters
    Task<ChapterDto> CreateChapterAsync(CreateChapterInput input);
    Task<ChapterDto> RenameChapterAsync(RenameChapterInput input);
    Task RemoveChapterAsync(RemoveChapterInput input);
    
    Task<ChapterDto> GetChapterAsync(Guid courseId, Guid chapterId);
    
    //Lessons
    Task<LessonDto> CreateLessonAsync(CreateLessonInput input);
    Task<LessonDto> RenameLessonAsync(RenameLessonInput input);
    Task<LessonDto> UpdateLessonAsync(UpdateLessonInput input);
    Task RemoveLessonAsync(RemoveLessonInput input);
    Task<List<LessonDto>> GetLessonsByChapterAsync(Guid courseId, Guid chapterId);
    Task<LessonDto> GetLessonAsync(Guid courseId, Guid chapterId, Guid lessonId);
    
    //Material
    Task<MaterialDto> CreateFileMaterialAsync(CreateFileMaterialInput input);
    Task<MaterialDto> CreateVideoLinkMaterialAsync(CreateVideoLinkMaterialInput input);
    Task<MaterialDto> CreateTextMaterialAsync(CreateTextMaterialInput input);
    Task<MaterialDto> RenameMaterialAsync(RenameMaterialInput input);
    Task<MaterialDto> UpdateFileMaterialAsync(UpdateFileMaterialInput input);
    Task<MaterialDto> UpdateVideoLinkMaterialAsync(UpdateVideoLinkMaterialInput input);
    Task<MaterialDto> UpdateTextMaterialAsync(UpdateTextMaterialInput input);
    Task HideMaterialAsync(HideMaterialInput input);
    Task ActivateMaterialAsync(ActivateMaterialInput input);
    Task RemoveMaterialAsync(RemoveMaterialInput input);
    Task<List<MaterialDto>> GetMaterialsByLessonAsync(Guid courseId, Guid chapterId, Guid lessonId);
    Task<MaterialDto> GetMaterialAsync(Guid courseId, Guid chapterId, Guid lessonId, Guid materialId);
    Task<UploadFileMaterialDto> UploadFileMaterialAsync(UploadFileMaterialInput input);
    Task<IRemoteStreamContent> DownloadFileMaterialAsync(Guid courseId, Guid chapterId, Guid lessonId, Guid materialId);
}