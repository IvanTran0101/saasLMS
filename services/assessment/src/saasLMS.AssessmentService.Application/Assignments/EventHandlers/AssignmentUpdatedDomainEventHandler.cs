using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments.Etos;
using saasLMS.AssessmentService.Assignments.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Assignments.EventHandlers;

public class AssignmentUpdatedDomainEventHandler :
    ILocalEventHandler<AssignmentUpdatedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public AssignmentUpdatedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(AssignmentUpdatedDomainEvent eventData)
    {
        var eto = new AssignmentUpdatedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.UpdatedAt,
            AssignmentId = eventData.AssignmentId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            Title = eventData.Title,
            Deadline = eventData.Deadline,
            MaxScore = eventData.MaxScore,
            UpdatedAt = eventData.UpdatedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
