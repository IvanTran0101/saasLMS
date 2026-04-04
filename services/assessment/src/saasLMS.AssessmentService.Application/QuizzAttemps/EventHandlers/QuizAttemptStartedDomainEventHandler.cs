using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.QuizAttempts.Etos;
using saasLMS.AssessmentService.QuizAttempts.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.QuizAttempts.EventHandlers;

public class QuizAttemptStartedDomainEventHandler :
    ILocalEventHandler<QuizAttemptStartedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizAttemptStartedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizAttemptStartedDomainEvent eventData)
    {
        var eto = new QuizAttemptStartedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.StartedAt,
            QuizAttemptId = eventData.QuizAttemptId,
            QuizId = eventData.QuizId,
            StudentId = eventData.StudentId,
            StartedAt = eventData.StartedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
