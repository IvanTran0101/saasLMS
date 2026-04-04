using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments.Etos;
using saasLMS.AssessmentService.Assignments.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Assignments.EventHandlers;

public class AssignmentClosedDomainEventHandler :
    ILocalEventHandler<AssignmentClosedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public AssignmentClosedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(AssignmentClosedDomainEvent eventData)
    {
        var eto = new AssignmentClosedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.ClosedAt,
            AssignmentId = eventData.AssignmentId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            ClosedAt = eventData.ClosedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
