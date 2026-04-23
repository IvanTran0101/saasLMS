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
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace saasLMS.CourseCatalogService.Courses;

[RemoteService(Name = CourseCatalogServiceRemoteServiceConsts.RemoteServiceName)]
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
    Task<CourseDto> GetCourseStudentAsync(Guid id);
    Task<CourseOwnerDto> GetOwnerAsync(Guid id);
    Task<CourseEligibilityDto?> GetEnrollmentEligibilityAsync(Guid courseId, Guid tenantId);
    Task<CourseDetailDto> GetCourseDetailAsync(Guid id);
    Task<CourseDetailDto> GetCourseDetailStudentAsync(Guid id);
    Task<List<CourseListItemDto>> GetPublishedCoursesAsync();
    Task<List<CourseListItemDto>> GetCoursesByInstructorAsync(Guid instructorId);
    Task<List<CourseListItemDto>> GetPublishedCoursesByInstructorAsync(Guid instructorId);
    Task<List<CourseListItemDto>> GetCoursesByTenantAsync();
    Task<List<CourseListItemDto>> GetPublishedCoursesByTenantAsync();
    
    //Chapters
    Task<ChapterDto> CreateChapterAsync(CreateChapterInput input);
    Task<ChapterDto> RenameChapterAsync(RenameChapterInput input);
    Task RemoveChapterAsync(RemoveChapterInput input);
    Task ReorderChaptersAsync(ReorderChaptersInput input);
    
    Task<ChapterDto> GetChapterAsync(Guid courseId, Guid chapterId);
    Task<List<ChapterDto>> GetChaptersByCourseAsync(Guid courseId);
    Task<List<ChapterDto>> GetChaptersByCourseStudentAsync(Guid courseId);
    Task<ChapterDto> GetChapterStudentAsync(Guid courseId, Guid chapterId);
    
    //Lessons
    Task<LessonDto> CreateLessonAsync(CreateLessonInput input);
    Task<LessonDto> RenameLessonAsync(RenameLessonInput input);
    Task<LessonDto> UpdateLessonAsync(UpdateLessonInput input);
    Task RemoveLessonAsync(RemoveLessonInput input);
    Task<List<LessonDto>> GetLessonsByChapterAsync(Guid courseId, Guid chapterId);
    Task<LessonDto> GetLessonAsync(Guid courseId, Guid chapterId, Guid lessonId);
    Task<List<LessonDto>> GetLessonsByChapterStudentAsync(Guid courseId, Guid chapterId);
    Task<LessonDto> GetLessonStudentAsync(Guid courseId, Guid chapterId, Guid lessonId);
    
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
    Task<List<MaterialDto>> GetMaterialsByLessonStudentAsync(Guid courseId, Guid chapterId, Guid lessonId);
    Task<MaterialDto> GetMaterialStudentAsync(Guid courseId, Guid chapterId, Guid lessonId, Guid materialId);
    Task<MaterialDto> UploadMaterialFileAsync(UploadMaterialFileInput input, IRemoteStreamContent file);
    Task<IRemoteStreamContent> DownloadMaterialFileAsync(DownloadMaterialFileInput input);
    Task<IRemoteStreamContent> DownloadMaterialFileStudentAsync(DownloadMaterialFileInput input);
}
