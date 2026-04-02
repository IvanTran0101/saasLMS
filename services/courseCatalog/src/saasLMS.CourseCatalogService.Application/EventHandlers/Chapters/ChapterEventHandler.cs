using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Etos.Chapters;
using saasLMS.CourseCatalogService.Events.Chapters;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.CourseCatalogService.EventHandlers.Chapters;

public class ChapterEventHandler
    : ILocalEventHandler<ChapterCreatedDomainEvent>,
      ILocalEventHandler<ChapterUpdatedDomainEvent>,
      ILocalEventHandler<ChapterDeletedDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public ChapterEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(ChapterCreatedDomainEvent eventData)
    {
        var chapter = eventData.Chapter;

        await _distributedEventBus.PublishAsync(new ChapterCreatedEto
        {
            EventId   = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId  = chapter.TenantId,
            CourseId  = eventData.CourseId,
            ChapterId = chapter.Id,
            Title     = chapter.Title,
            OrderNo   = chapter.OrderNo
        });
    }

    public async Task HandleEventAsync(ChapterUpdatedDomainEvent eventData)
    {
        var chapter = eventData.Chapter;

        await _distributedEventBus.PublishAsync(new ChapterUpdatedEto
        {
            EventId    = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId   = chapter.TenantId,
            CourseId   = eventData.CourseId,
            ChapterId  = chapter.Id,
            Title      = chapter.Title,
            OrderNo    = chapter.OrderNo
        });
    }

    public async Task HandleEventAsync(ChapterDeletedDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new ChapterDeletedEto
        {
            EventId    = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId   = eventData.TenantId,
            CourseId   = eventData.CourseId,
            ChapterId  = eventData.ChapterId,
            Title      = eventData.Title,
            OrderNo    = eventData.OrderNo
        });
    }
}