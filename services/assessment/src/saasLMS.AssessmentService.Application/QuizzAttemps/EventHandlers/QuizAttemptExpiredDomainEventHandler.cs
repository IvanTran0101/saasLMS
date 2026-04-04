using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.QuizAttempts.Etos;
using saasLMS.AssessmentService.QuizAttempts.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.QuizAttempts.EventHandlers;

public class QuizAttemptExpiredDomainEventHandler :
    ILocalEventHandler<QuizAttemptExpiredDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizAttemptExpiredDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizAttemptExpiredDomainEvent eventData)
    {
        var eto = new QuizAttemptExpiredEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.ExpiredAt,
            QuizAttemptId = eventData.QuizAttemptId,
            QuizId = eventData.QuizId,
            StudentId = eventData.StudentId,
            ExpiredAt = eventData.ExpiredAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
