using System;
using System.Threading.Tasks;
using saasLMS.LearningProgressService.Etos.LessonProgresses;
using saasLMS.LearningProgressService.LessonProgresses.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.LearningProgressService.LearningProgresses.EventHandlers;

public class LessonProgressEventHandler
    : ILocalEventHandler<LessonViewedDomainEvent>,
      ILocalEventHandler<LessonStatusChangedDomainEvent>,
      ILocalEventHandler<LessonCompletedDomainEvent>,
      ILocalEventHandler<LastLearningPositionUpdatedDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public LessonProgressEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(LessonViewedDomainEvent eventData)
    {
        var progress = eventData.LessonProgress;

        await _distributedEventBus.PublishAsync(new LessonViewedEto
        {
            EventId    = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId   = progress.TenantId,
            StudentId  = progress.StudentId,
            CourseId   = progress.CourseId,
            LessonId   = progress.LessonId,
            ViewedAt   = eventData.ViewedAt
        });
    }

    public async Task HandleEventAsync(LessonStatusChangedDomainEvent eventData)
    {
        var progress = eventData.LessonProgress;

        await _distributedEventBus.PublishAsync(new LessonStatusChangedEto
        {
            EventId    = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId   = progress.TenantId,
            StudentId  = progress.StudentId,
            CourseId   = progress.CourseId,
            LessonId   = progress.LessonId,
            From       = eventData.From,
            To         = eventData.To,
            ChangedAt  = eventData.ChangedAt
        });
    }

    public async Task HandleEventAsync(LessonCompletedDomainEvent eventData)
    {
        var progress = eventData.LessonProgress;

        await _distributedEventBus.PublishAsync(new LessonCompletedEto
        {
            EventId     = Guid.NewGuid(),
            OccurredAt  = DateTime.UtcNow,
            TenantId    = progress.TenantId,
            StudentId   = progress.StudentId,
            CourseId    = progress.CourseId,
            LessonId    = progress.LessonId,
            CompletedAt = eventData.CompletedAt
        });
    }

    public async Task HandleEventAsync(LastLearningPositionUpdatedDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new LastLearningPositionUpdatedEto
        {
            EventId    = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId   = eventData.TenantId,
            StudentId  = eventData.StudentId,
            CourseId   = eventData.CourseId,
            LessonId   = eventData.LessonId,
            LessonStatus     = eventData.LessonStatus,
            UpdatedAt  = eventData.UpdatedAt
        });
    }
}