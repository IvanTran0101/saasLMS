using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments.Etos;
using saasLMS.AssessmentService.Assignments.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Assignments.EventHandlers;

public class AssignmentPublishedDomainEventHandler :
    ILocalEventHandler<AssignmentPublishedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public AssignmentPublishedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(AssignmentPublishedDomainEvent eventData)
    {
        var eto = new AssignmentPublishedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.PublishedAt,
            AssignmentId = eventData.AssignmentId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            PublishedAt = eventData.PublishedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
