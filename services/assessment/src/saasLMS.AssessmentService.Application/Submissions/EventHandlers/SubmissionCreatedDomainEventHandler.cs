using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Submissions.Etos;
using saasLMS.AssessmentService.Submissions.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Submissions.EventHandlers;

public class SubmissionCreatedDomainEventHandler :
    ILocalEventHandler<SubmissionCreatedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public SubmissionCreatedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(SubmissionCreatedDomainEvent eventData)
    {
        var eto = new SubmissionCreatedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.SubmittedAt,
            SubmissionId = eventData.SubmissionId,
            AssignmentId = eventData.AssignmentId,
            StudentId = eventData.StudentId,
            ContentType = eventData.ContentType,
            SubmittedAt = eventData.SubmittedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
