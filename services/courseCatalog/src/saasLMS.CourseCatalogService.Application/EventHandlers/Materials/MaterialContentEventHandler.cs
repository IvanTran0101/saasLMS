using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Etos.Materials;
using saasLMS.CourseCatalogService.Events.Materials;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.CourseCatalogService.EventHandlers.Materials;

public class MaterialContentEventHandler
    : ILocalEventHandler<MaterialRenamedDomainEvent>,
      ILocalEventHandler<MaterialFileUpdatedDomainEvent>,
      ILocalEventHandler<MaterialVideoLinkUpdatedDomainEvent>,
      ILocalEventHandler<MaterialTextFormatUpdatedDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public MaterialContentEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(MaterialRenamedDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialRenamedEto
        {
            EventId     = Guid.NewGuid(),
            OccurredAt  = DateTime.UtcNow,
            TenantId    = m.TenantId,
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
            TextFormat  = m.TextFormat
        });
    }

    public async Task HandleEventAsync(MaterialFileUpdatedDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialFileUpdatedEto
        {
            EventId    = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            TenantId   = m.TenantId,
            CourseId   = eventData.CourseId,
            ChapterId  = eventData.ChapterId,
            LessonId   = m.LessonId,
            MaterialId = m.Id,
            Title      = m.Title,
            Type       = m.Type,
            Status     = m.Status,
            SortOrder  = m.SortOrder,
            FileName   = m.FileName,
            MimeType   = m.MimeType,
            FileSize   = m.FileSize
        });
    }

    public async Task HandleEventAsync(MaterialVideoLinkUpdatedDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialVideoLinkUpdatedEto
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
            ExternalUrl = m.ExternalUrl
        });
    }

    public async Task HandleEventAsync(MaterialTextFormatUpdatedDomainEvent eventData)
    {
        var m = eventData.Material;

        await _distributedEventBus.PublishAsync(new MaterialTextFormatUpdatedEto
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
            TextContent = m.TextContent,
            TextFormat  = m.TextFormat
        });
    }
}