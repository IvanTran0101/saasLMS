using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.LessonProgresses;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Inputs;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.Permissions;
using Volo.Abp;
using Volo.Abp.Users;

namespace saasLMS.LearningProgressService;

public class LearningProgressAppService : LearningProgressServiceAppService, ILearningProgressAppService
{
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly ICourseProgressRepository _courseProgressRepository;
    private readonly LessonProgressManager _lessonProgressManager;
    private readonly CourseProgressManager _courseProgressManager;
    private readonly IEnrollmentGateway _enrollmentGateway;
 
    public LearningProgressAppService(
        ILessonProgressRepository lessonProgressRepository,
        ICourseProgressRepository courseProgressRepository,
        LessonProgressManager lessonProgressManager,
        CourseProgressManager courseProgressManager,
        IEnrollmentGateway enrollmentGateway)
    {
        _lessonProgressRepository = lessonProgressRepository;
        _courseProgressRepository = courseProgressRepository;
        _lessonProgressManager = lessonProgressManager;
        _courseProgressManager = courseProgressManager;
        _enrollmentGateway = enrollmentGateway;
    }
    
    [Authorize(LearningProgressServicePermissions.LessonProgresses.Record)]
    public async Task<LessonProgressDto> StartLessonAsync(StartLessonInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var (tenantId, studentId) = ResolveAndValidateStudentContext();
        ValidateCourseId(input.CourseId);
        ValidateLessonId(input.LessonId);
        await EnsureEnrollmentActiveAsync(tenantId, input.CourseId, studentId);
 
        var lessonProgress = await FindOrCreateLessonProgressAsync(
            tenantId, input.CourseId, input.LessonId, studentId);
 
        if (lessonProgress.Status == LessonProgressStatus.NotStarted)
        {
            await _lessonProgressManager.MarkAsViewedAsync(lessonProgress, Clock.Now);
            await _lessonProgressRepository.UpdateAsync(lessonProgress, autoSave: true);
        }
 
        await UpdateCourseProgressLastAccessAsync(
            tenantId, input.CourseId, studentId, input.LessonId, input.TotalLessonsCount);
 
        return ObjectMapper.Map<LessonProgress, LessonProgressDto>(lessonProgress);
    }
    
    [Authorize(LearningProgressServicePermissions.LessonProgresses.Record)]
    public async Task<LessonProgressDto> ViewLessonAsync(ViewLessonInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var (tenantId, studentId) = ResolveAndValidateStudentContext();
        ValidateCourseId(input.CourseId);
        ValidateLessonId(input.LessonId);
        await EnsureEnrollmentActiveAsync(tenantId, input.CourseId, studentId);
 
        var lessonProgress = await FindOrCreateLessonProgressAsync(
            tenantId, input.CourseId, input.LessonId, studentId);
 
        await _lessonProgressManager.MarkAsViewedAsync(lessonProgress, Clock.Now);
        await _lessonProgressRepository.UpdateAsync(lessonProgress, autoSave: true);
 
        await UpdateCourseProgressLastAccessAsync(
            tenantId, input.CourseId, studentId, input.LessonId, input.TotalLessonsCount);
 
        return ObjectMapper.Map<LessonProgress, LessonProgressDto>(lessonProgress);
    }
    
    [Authorize(LearningProgressServicePermissions.LessonProgresses.Record)]
    public async Task<LessonProgressDto> CompleteLessonAsync(CompleteLessonInput input)
    {
        Check.NotNull(input, nameof(input));
 
        var (tenantId, studentId) = ResolveAndValidateStudentContext();
        ValidateCourseId(input.CourseId);
        ValidateLessonId(input.LessonId);
        await EnsureEnrollmentActiveAsync(tenantId, input.CourseId, studentId);
 
        var lessonProgress = await FindOrCreateLessonProgressAsync(
            tenantId, input.CourseId, input.LessonId, studentId);
 
        if (lessonProgress.Status == LessonProgressStatus.Completed)
        {
            return ObjectMapper.Map<LessonProgress, LessonProgressDto>(lessonProgress);
        }
 
        await _lessonProgressManager.MarkAsCompletedAsync(lessonProgress, Clock.Now);
        await _lessonProgressRepository.UpdateAsync(lessonProgress, autoSave: true);
 
        await RecalculateCourseProgressAsync(
            tenantId, input.CourseId, studentId, input.LessonId, input.TotalLessonsCount);
 
        return ObjectMapper.Map<LessonProgress, LessonProgressDto>(lessonProgress);
    }
    
    [Authorize(LearningProgressServicePermissions.LessonProgresses.ViewOwn)]
    public async Task<List<LessonProgressDto>> GetMyProgressAsync(Guid courseId)
    {
        var (tenantId, studentId) = ResolveAndValidateStudentContext();
        ValidateCourseId(courseId);
 
        var lessonProgresses = await _lessonProgressRepository.GetListByCourseAndStudentAsync(
            tenantId, courseId, studentId);
 
        return lessonProgresses
            .Select(MapLessonProgressDetail)
            .ToList();
    }
    
    [Authorize(LearningProgressServicePermissions.LessonProgresses.ViewOwn)]
    public async Task<CourseProgressDto> GetMyCourseProgressAsync(Guid courseId)
    {
        var (tenantId, studentId) = ResolveAndValidateStudentContext();
        ValidateCourseId(courseId);
 
        var courseProgress = await _courseProgressRepository.FindByCourseAndStudentAsync(
            tenantId, courseId, studentId);
 
        // Sinh viên chưa học bài nào → CourseProgress chưa tồn tại 
        // Trả về default thay vì throw để UI không bị lỗi ở màn hình đầu tiên
        if (courseProgress == null)
        {
            return new CourseProgressDto
            {
                CourseId           = courseId,
                StudentId          = studentId,
                Status             = CourseProgressStatus.NotStarted,
                CompletedLessonsCount = 0,
                TotalLessonsCount  = 0,
                ProgressPercent    = 0,
                StartedAt          = null,
                CompletedAt        = null,
                LastAccessedAt     = null,
                LastAccessedLessonId = null
            };
        }
 
        return ObjectMapper.Map<CourseProgress, CourseProgressDto>(courseProgress);
    }
    
    [Authorize(LearningProgressServicePermissions.LessonProgresses.ViewOwn)]
    public async Task<ResumeResultDto> GetResumePositionAsync(Guid courseId)
    {
        var (tenantId, studentId) = ResolveAndValidateStudentContext();
        ValidateCourseId(courseId);
 
        var inProgressLesson = await _lessonProgressRepository.GetListByCourseAndStudentAsync(
            tenantId, courseId, studentId, status: LessonProgressStatus.InProgress);
 
        var target = inProgressLesson.FirstOrDefault();
 
        if (target != null)
        {
            return ObjectMapper.Map<LessonProgress, ResumeResultDto>(target);
        }
 
        var lastViewedLesson = await _lessonProgressRepository.GetLastViewedLessonAsync(
            tenantId, courseId, studentId);
 
        if (lastViewedLesson != null)
        {
            return ObjectMapper.Map<LessonProgress, ResumeResultDto>(lastViewedLesson);
        }
 
        return new ResumeResultDto
        {
            CourseId     = courseId,
            LessonId     = null,
            LessonStatus = null,
            LastViewedAt = null
        };
    }
    
    //Private Helper
    private (Guid tenantId, Guid studentId) ResolveAndValidateStudentContext()
    {
        var tenantId = CurrentTenant.Id;
        if (!tenantId.HasValue)
        {
            throw new BusinessException("LearningProgressService:TenantNotFound");
        }
 
        var studentId = CurrentUser.GetId();
        return (tenantId.Value, studentId);
    }
 
    private static void ValidateCourseId(Guid courseId)
    {
        if (courseId == Guid.Empty)
        {
            throw new BusinessException("LearningProgressService:EmptyCourseId");
        }
    }
 
    private static void ValidateLessonId(Guid lessonId)
    {
        if (lessonId == Guid.Empty)
        {
            throw new BusinessException("LearningProgressService:EmptyLessonId");
        }
    }
 
    private async Task EnsureEnrollmentActiveAsync(Guid tenantId, Guid courseId, Guid studentId)
    {
        var isActive = await _enrollmentGateway.IsEnrollmentActiveAsync(
            tenantId, courseId, studentId);
 
        if (!isActive)
        {
            throw new BusinessException("LearningProgressService:NotEnrolled");
        }
    }
 
    private async Task<LessonProgress> FindOrCreateLessonProgressAsync(
        Guid tenantId, Guid courseId, Guid lessonId, Guid studentId)
    {
        var lessonProgress = await _lessonProgressRepository.FindByLessonAndStudentAsync(
            tenantId, lessonId, studentId);
 
        if (lessonProgress != null)
        {
            return lessonProgress;
        }
 
        lessonProgress = await _lessonProgressManager.CreateAsync(
            tenantId, courseId, lessonId, studentId);
 
        await _lessonProgressRepository.InsertAsync(lessonProgress, autoSave: true);
        return lessonProgress;
    }
 
    private async Task<CourseProgress> FindOrCreateCourseProgressAsync(
        Guid tenantId, Guid courseId, Guid studentId, int? clientTotalLessonsCount)
    {
        var courseProgress = await _courseProgressRepository.FindByCourseAndStudentAsync(
            tenantId, courseId, studentId);
 
        if (courseProgress != null)
        {
            return courseProgress;
        }
 
        if (!clientTotalLessonsCount.HasValue || clientTotalLessonsCount.Value <= 0)
        {
            throw new BusinessException("LearningProgressService:TotalLessonsCountRequired");
        }
 
        courseProgress = await _courseProgressManager.CreateAsync(
            tenantId, courseId, studentId, clientTotalLessonsCount.Value);
 
        await _courseProgressRepository.InsertAsync(courseProgress, autoSave: true);
        return courseProgress;
    }
 
    private async Task UpdateCourseProgressLastAccessAsync(
        Guid tenantId, Guid courseId, Guid studentId, Guid lessonId, int? clientTotalLessonsCount)
    {
        var courseProgress = await FindOrCreateCourseProgressAsync(
            tenantId, courseId, studentId, clientTotalLessonsCount);
 
        if (clientTotalLessonsCount.HasValue
            && clientTotalLessonsCount.Value > 0
            && clientTotalLessonsCount.Value != courseProgress.TotalLessonsCount)
        {
            await _courseProgressManager.UpdateTotalLessonsCountAsync(
                courseProgress, clientTotalLessonsCount.Value, Clock.Now);
        }
 
        await _courseProgressManager.UpdateLastAccessAsync(courseProgress, lessonId, Clock.Now);
        await _courseProgressRepository.UpdateAsync(courseProgress, autoSave: true);
    }
 
    private async Task RecalculateCourseProgressAsync(
        Guid tenantId, Guid courseId, Guid studentId, Guid lessonId, int? clientTotalLessonsCount)
    {
        var courseProgress = await FindOrCreateCourseProgressAsync(
            tenantId, courseId, studentId, clientTotalLessonsCount);
 
        if (clientTotalLessonsCount.HasValue
            && clientTotalLessonsCount.Value > 0
            && clientTotalLessonsCount.Value != courseProgress.TotalLessonsCount)
        {
            await _courseProgressManager.UpdateTotalLessonsCountAsync(
                courseProgress, clientTotalLessonsCount.Value, Clock.Now);
        }
 
        var lessonProgresses = await _lessonProgressRepository.GetListByCourseAndStudentAsync(
            tenantId, courseId, studentId);
 
        var completedCount = lessonProgresses.Count(lp => lp.Status == LessonProgressStatus.Completed);
 
        await _courseProgressManager.UpdateCompletedLessonsAsync(
            courseProgress, completedCount, Clock.Now);
 
        await _courseProgressManager.UpdateLastAccessAsync(courseProgress, lessonId, Clock.Now);
        await _courseProgressRepository.UpdateAsync(courseProgress, autoSave: true);
    }
    
    private LessonProgressDto MapLessonProgressDetail(LessonProgress lessonProgress)
    {
        return ObjectMapper.Map<LessonProgress, LessonProgressDto>(lessonProgress);
    }
}