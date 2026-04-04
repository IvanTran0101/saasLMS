using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Etos.Materials;
using saasLMS.CourseCatalogService.Events.Materials;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.CourseCatalogService.EventHandlers.Materials;

public class MaterialEventHandler
    : ILocalEventHandler<MaterialCreatedDomainEvent>,
      ILocalEventHandler<MaterialHiddenDomainEvent>,
      ILocalEventHandler<MaterialUpdatedDomainEvent>,
      ILocalEventHandler<MaterialDeletedDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public MaterialEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(MaterialCreatedDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialCreatedEto
        {
            EventId     = Guid.NewGuid(),
            OccurredAt  = DateTime.UtcNow,
            TenantId    = m.TenantId,
            CourseId    = eventData.CourseId,
            ChapterId   = eventData.ChapterId,
            LessonId    = m.LessonId,
            MaterialId  = m.Id,
            Title       = m.Title,
            Type        = m.Type,
            Status      = m.Status,
            SortOrder   = m.SortOrder,
            FileName    = m.FileName,
            MimeType    = m.MimeType,
            FileSize    = m.FileSize,
            ExternalUrl = m.ExternalUrl,
            TextContent = m.TextContent,
            TextFormat  = m.TextFormat
        });
    }

    public async Task HandleEventAsync(MaterialHiddenDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialHiddenEto
        {
            EventId     = Guid.NewGuid(),
            OccurredAt  = DateTime.UtcNow,
            TenantId    = m.TenantId,
            CourseId    = eventData.CourseId,
            ChapterId   = eventData.ChapterId,
            LessonId    = m.LessonId,
            MaterialId  = m.Id,
            Title       = m.Title,
            Type        = m.Type,
            Status      = m.Status,
            SortOrder   = m.SortOrder,
            FileName    = m.FileName,
            MimeType    = m.MimeType,
            FileSize    = m.FileSize,
            ExternalUrl = m.ExternalUrl,
            TextContent = m.TextContent,
            TextFormat  = m.TextFormat
        });
    }

    public async Task HandleEventAsync(MaterialUpdatedDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialUpdatedEto
        {
            EventId     = Guid.NewGuid(),
            OccurredAt  = DateTime.UtcNow,
            TenantId    = m.TenantId,
            CourseId    = eventData.CourseId,
            ChapterId   = eventData.ChapterId,
            LessonId    = m.LessonId,
            MaterialId  = m.Id,
            Title       = m.Title,
            Type        = m.Type,
            Status      = m.Status,
            SortOrder   = m.SortOrder,
            FileName    = m.FileName,
            MimeType    = m.MimeType,
            FileSize    = m.FileSize,
            ExternalUrl = m.ExternalUrl,
            TextContent = m.TextContent,
            TextFormat  = m.TextFormat
        });
    }

    public async Task HandleEventAsync(MaterialDeletedDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new MaterialDeletedEto
        {
            EventId     = Guid.NewGuid(),
            OccurredAt  = DateTime.UtcNow,
            TenantId    = eventData.TenantId,
            CourseId    = eventData.CourseId,
            ChapterId   = eventData.ChapterId,
            LessonId    = eventData.LessonId,
            MaterialId  = eventData.MaterialId,
            Title       = eventData.Title,
            Type        = eventData.Type,
            Status      = eventData.Status,
            SortOrder   = eventData.SortOrder,
            FileName    = eventData.FileName,
            MimeType    = eventData.MimeType,
            FileSize    = eventData.FileSize,
            ExternalUrl = eventData.ExternalUrl,
            TextContent = eventData.TextContent,
            TextFormat  = eventData.TextFormat
        });
    }
}