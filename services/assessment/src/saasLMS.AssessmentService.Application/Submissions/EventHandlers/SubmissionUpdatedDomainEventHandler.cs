using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Submissions.Etos;
using saasLMS.AssessmentService.Submissions.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Submissions.EventHandlers;

public class SubmissionUpdatedDomainEventHandler :
    ILocalEventHandler<SubmissionUpdatedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public SubmissionUpdatedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(SubmissionUpdatedDomainEvent eventData)
    {
        var eto = new SubmissionUpdatedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.UpdatedAt,
            SubmissionId = eventData.SubmissionId,
            AssignmentId = eventData.AssignmentId,
            StudentId = eventData.StudentId,
            ContentType = eventData.ContentType,
            SubmittedAt = eventData.UpdatedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
