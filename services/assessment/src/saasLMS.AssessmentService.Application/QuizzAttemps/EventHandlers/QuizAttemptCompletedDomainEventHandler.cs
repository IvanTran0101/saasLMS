using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.QuizAttempts.Etos;
using saasLMS.AssessmentService.QuizAttempts.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.QuizAttempts.EventHandlers;

public class QuizAttemptCompletedDomainEventHandler :
    ILocalEventHandler<QuizAttemptCompletedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizAttemptCompletedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizAttemptCompletedDomainEvent eventData)
    {
        var eto = new QuizAttemptCompletedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.CompletedAt,
            QuizAttemptId = eventData.QuizAttemptId,
            QuizId = eventData.QuizId,
            StudentId = eventData.StudentId,
            Score = eventData.Score,
            CompletedAt = eventData.CompletedAt,
            CompletionMode = eventData.CompletionMode
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
