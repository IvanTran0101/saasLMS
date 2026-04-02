using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Etos.Lessons;
using saasLMS.CourseCatalogService.Events.Lessons;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.CourseCatalogService.EventHandlers.Lessons;

public class LessonEventHandler
    : ILocalEventHandler<LessonCreatedDomainEvent>,
      ILocalEventHandler<LessonUpdatedDomainEvent>,
      ILocalEventHandler<LessonHiddenDomainEvent>,
      ILocalEventHandler<LessonDeletedDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public LessonEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(LessonCreatedDomainEvent eventData)
    {
        var lesson = eventData.Lesson;

        await _distributedEventBus.PublishAsync(new LessonCreatedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = lesson.TenantId,
            CourseId     = eventData.CourseId,
            ChapterId    = lesson.ChapterId,
            LessonId     = lesson.Id,
            Title        = lesson.Title,
            SortOrder    = lesson.SortOrder,
            ContentState = lesson.ContentState
        });
    }

    public async Task HandleEventAsync(LessonUpdatedDomainEvent eventData)
    {
        var lesson = eventData.Lesson;

        await _distributedEventBus.PublishAsync(new LessonUpdatedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = lesson.TenantId,
            CourseId     = eventData.CourseId,
            ChapterId    = lesson.ChapterId,
            LessonId     = lesson.Id,
            Title        = lesson.Title,
            SortOrder    = lesson.SortOrder,
            ContentState = lesson.ContentState
        });
    }

    public async Task HandleEventAsync(LessonHiddenDomainEvent eventData)
    {
        var lesson = eventData.Lesson;

        await _distributedEventBus.PublishAsync(new LessonHiddenEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = lesson.TenantId,
            CourseId     = eventData.CourseId,
            ChapterId    = lesson.ChapterId,
            LessonId     = lesson.Id,
            Title        = lesson.Title,
            SortOrder    = lesson.SortOrder,
            ContentState = lesson.ContentState
        });
    }

    public async Task HandleEventAsync(LessonDeletedDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new LessonDeletedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = eventData.TenantId,
            CourseId     = eventData.CourseId,
            ChapterId    = eventData.ChapterId,
            LessonId     = eventData.LessonId,
            Title        = eventData.Title,
            SortOrder    = eventData.SortOrder,
            ContentState = eventData.ContentState
        });
    }
}