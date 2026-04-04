using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments.Etos;
using saasLMS.AssessmentService.Assignments.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Assignments.EventHandlers;

public class AssignmentCreatedDomainEventHandler :
    ILocalEventHandler<AssignmentCreatedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public AssignmentCreatedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(AssignmentCreatedDomainEvent eventData)
    {
        var eto = new AssignmentCreatedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.CreatedAt,
            AssignmentId = eventData.AssignmentId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            Title = eventData.Title,
            Deadline = eventData.Deadline,
            MaxScore = eventData.MaxScore,
            CreatedAt = eventData.CreatedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
