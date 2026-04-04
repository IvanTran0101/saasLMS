using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.LearningProgressService.CourseProgresses;

public class CourseProgressManager : DomainService
{
    private readonly ICourseProgressRepository _courseProgressRepository;

    public CourseProgressManager(ICourseProgressRepository courseProgressRepository)
    {
        _courseProgressRepository = courseProgressRepository;
    }

    public async Task<CourseProgress> CreateAsync(
        Guid tenantId,
        Guid courseId,
        Guid studentId,
        int totalLessonsCount,
        CancellationToken cancellationToken = default) 
    {
        var exists = await _courseProgressRepository.ExistsByCourseAndStudentAsync(
            tenantId,
            courseId,
            studentId,
            cancellationToken);
        if (exists)
        {
            throw new BusinessException("LearningProgressService:CourseProgressAlreadyExists")
                .WithData("TenantId", tenantId)
                .WithData("CourseId", courseId)
                .WithData("StudentId", studentId);
        }
        return new CourseProgress(
            GuidGenerator.Create(),
            tenantId,
            courseId,
            studentId,
            totalLessonsCount);
    }

    public Task UpdateLastAccessAsync(
        CourseProgress courseProgress,
        Guid lessonId,
        DateTime lastAccessAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(courseProgress, nameof(courseProgress));
        courseProgress.UpdateLastAccess(lessonId, lastAccessAt);
        return Task.CompletedTask;
    }

    public Task UpdateCompletedLessonsAsync(
        CourseProgress courseProgress,
        int completedLessonsCount,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(courseProgress, nameof(courseProgress));
        courseProgress.UpdateCompletedLessons(completedLessonsCount, updatedAt);
        return Task.CompletedTask;
    }

    public Task UpdateTotalLessonsCountAsync(
        CourseProgress courseProgress,
        int totalLessonsCount,
        DateTime updatedAt,
        CancellationToken cancellationToken = default
    )
    {
        Check.NotNull(courseProgress, nameof(courseProgress));
        courseProgress.UpdateTotalLessonsCount(totalLessonsCount, updatedAt);
        return Task.CompletedTask;
    }
}