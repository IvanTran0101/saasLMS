using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Etos.Lessons;
using saasLMS.LearningProgressService.CourseProgresses;
using saasLMS.LearningProgressService.CourseStructures;
using saasLMS.LearningProgressService.Enrollments;
using saasLMS.LearningProgressService.Etos.CourseProgresses;
using saasLMS.LearningProgressService.LessonProgresses;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;

namespace saasLMS.LearningProgressService.CourseStructures.EventHandlers;

public class LessonProjectionEventHandler
    : IDistributedEventHandler<LessonCreatedEto>,
      IDistributedEventHandler<LessonDeletedEto>,
      ITransientDependency
{
    private readonly ILessonProjectionRepository _lessonRepository;
    private readonly IEnrollmentProjectionRepository _enrollmentRepository;
    private readonly ICourseProgressRepository _courseProgressRepository;
    private readonly CourseProgressManager _courseProgressManager;
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IDistributedEventBus _distributedEventBus;

    public LessonProjectionEventHandler(
        ILessonProjectionRepository lessonRepository,
        IEnrollmentProjectionRepository enrollmentRepository,
        ICourseProgressRepository courseProgressRepository,
        CourseProgressManager courseProgressManager,
        ILessonProgressRepository lessonProgressRepository,
        IGuidGenerator guidGenerator,
        IDistributedEventBus distributedEventBus)
    {
        _lessonRepository = lessonRepository;
        _enrollmentRepository = enrollmentRepository;
        _courseProgressRepository = courseProgressRepository;
        _courseProgressManager = courseProgressManager;
        _lessonProgressRepository = lessonProgressRepository;
        _guidGenerator = guidGenerator;
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(LessonCreatedEto eventData)
    {
        var projection = await _lessonRepository.FindByLessonIdAsync(
            eventData.TenantId, eventData.LessonId);

        if (projection == null)
        {
            projection = new LessonProjection(
                _guidGenerator.Create(),
                eventData.TenantId,
                eventData.CourseId,
                eventData.ChapterId,
                eventData.LessonId,
                eventData.Title,
                eventData.SortOrder);

            await _lessonRepository.InsertAsync(projection, autoSave: true);
        }
        else
        {
            projection.Activate(eventData.Title, eventData.SortOrder);
            await _lessonRepository.UpdateAsync(projection, autoSave: true);
        }

        await RecalculateForCourseAsync(eventData.TenantId, eventData.CourseId);
    }

    public async Task HandleEventAsync(LessonDeletedEto eventData)
    {
        var projection = await _lessonRepository.FindByLessonIdAsync(
            eventData.TenantId, eventData.LessonId);

        if (projection == null)
        {
            projection = new LessonProjection(
                _guidGenerator.Create(),
                eventData.TenantId,
                eventData.CourseId,
                eventData.ChapterId,
                eventData.LessonId,
                eventData.Title,
                eventData.SortOrder,
                isActive: false);

            projection.Deactivate();
            await _lessonRepository.InsertAsync(projection, autoSave: true);
        }
        else
        {
            projection.Deactivate();
            await _lessonRepository.UpdateAsync(projection, autoSave: true);
        }

        await RecalculateForCourseAsync(eventData.TenantId, eventData.CourseId);
    }

    private async Task RecalculateForCourseAsync(Guid tenantId, Guid courseId)
    {
        var activeLessonIds = await _lessonRepository.GetActiveLessonIdsByCourseAsync(tenantId, courseId);
        var totalLessons = activeLessonIds.Count;
        var activeEnrollments = await _enrollmentRepository.GetActiveByCourseAsync(tenantId, courseId);

        foreach (var enrollment in activeEnrollments)
        {
            var courseProgress = await _courseProgressRepository.FindByCourseAndStudentAsync(
                tenantId, courseId, enrollment.StudentId);

            if (courseProgress == null)
            {
                courseProgress = await _courseProgressManager.CreateAsync(
                    tenantId, courseId, enrollment.StudentId, totalLessons);

                await _courseProgressRepository.InsertAsync(courseProgress, autoSave: true);
                await PublishCourseProgressUpdatedAsync(courseProgress);
                continue;
            }

            if (courseProgress.TotalLessonsCount != totalLessons)
            {
                await _courseProgressManager.UpdateTotalLessonsCountAsync(
                    courseProgress, totalLessons, DateTime.UtcNow);
            }

            var completedCount = await _lessonProgressRepository.CountCompletedByCourseAndStudentAsync(
                tenantId, courseId, enrollment.StudentId, activeLessonIds);
            await _courseProgressManager.UpdateCompletedLessonsAsync(
                courseProgress, completedCount, DateTime.UtcNow);

            await _courseProgressRepository.UpdateAsync(courseProgress, autoSave: true);
            await PublishCourseProgressUpdatedAsync(courseProgress);
        }
    }

    private Task PublishCourseProgressUpdatedAsync(CourseProgress courseProgress)
    {
        return _distributedEventBus.PublishAsync(new CourseProgressUpdatedEto
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId = courseProgress.TenantId,
            CourseId = courseProgress.CourseId,
            StudentId = courseProgress.StudentId,
            Status = courseProgress.Status,
            CompletedLessonsCount = courseProgress.CompletedLessonsCount,
            TotalLessonsCount = courseProgress.TotalLessonsCount,
            ProgressPercent = courseProgress.ProgressPercent,
            StartedAt = courseProgress.StartedAt,
            CompletedAt = courseProgress.CompletedAt,
            LastAccessedAt = courseProgress.LastAccessedAt,
            LastAccessedLessonId = courseProgress.LastAccessedLessonId
        });
    }
}
