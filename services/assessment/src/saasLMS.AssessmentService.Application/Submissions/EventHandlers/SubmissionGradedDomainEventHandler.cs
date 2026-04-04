using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Submissions.Etos;
using saasLMS.AssessmentService.Submissions.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Submissions.EventHandlers;

public class SubmissionGradedDomainEventHandler :
    ILocalEventHandler<SubmissionGradedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public SubmissionGradedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(SubmissionGradedDomainEvent eventData)
    {
        var eto = new SubmissionGradedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.GradedAt,
            SubmissionId = eventData.SubmissionId,
            AssignmentId = eventData.AssignmentId,
            StudentId = eventData.StudentId,
            Score = eventData.Score,
            GradedAt = eventData.GradedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
