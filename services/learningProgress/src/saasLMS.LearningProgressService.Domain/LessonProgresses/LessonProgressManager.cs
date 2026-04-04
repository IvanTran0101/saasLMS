using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace saasLMS.LearningProgressService.LessonProgresses;

public class LessonProgressManager : DomainService
{
    private readonly ILessonProgressRepository _lessonProgressRepository;

    public LessonProgressManager(ILessonProgressRepository lessonProgressRepository)
    {
        _lessonProgressRepository = lessonProgressRepository;
    }

    public async Task<LessonProgress> CreateAsync(
        Guid tenantId,
        Guid courseId,
        Guid lessonId,
        Guid studentId,
        CancellationToken cancellationToken = default)
    {
        var exists = await _lessonProgressRepository.ExistsByLessonAndStudentAsync(
            tenantId,
            lessonId,
            studentId,
            cancellationToken);
        if (exists)
        {
            throw new BusinessException("LearningProgressService:LessonProgress:AlreadyExists")
                .WithData("TenantId", tenantId)
                .WithData("LessonId", lessonId)
                .WithData("StudentId", studentId);
        }
        return new LessonProgress(
            GuidGenerator.Create(),
            tenantId,
            courseId,
            lessonId,
            studentId);
    }

    public Task MarkAsViewedAsync(
        LessonProgress lessonProgress,
        DateTime viewedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(lessonProgress, nameof(lessonProgress));
        lessonProgress.MarkAsViewed(viewedAt);
        return Task.CompletedTask;
    }

    public Task MarkAsCompletedAsync(
        LessonProgress lessonProgress,
        DateTime completedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(lessonProgress, nameof(lessonProgress));
        lessonProgress.MarkAsCompleted(completedAt);
        return Task.CompletedTask;
    }

    public Task ResetToInProgressAsync(
        LessonProgress lessonProgress,
        DateTime updatedAt,
        CancellationToken cancellationToken = default)
    {
        Check.NotNull(lessonProgress, nameof(lessonProgress));
        lessonProgress.ResetToInProgress(updatedAt);
        return Task.CompletedTask;
    }
    
}