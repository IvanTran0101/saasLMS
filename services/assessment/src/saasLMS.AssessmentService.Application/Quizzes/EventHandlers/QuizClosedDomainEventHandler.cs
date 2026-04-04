using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes.Etos;
using saasLMS.AssessmentService.Quizzes.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Quizzes.EventHandlers;

public class QuizClosedDomainEventHandler :
    ILocalEventHandler<QuizClosedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizClosedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizClosedDomainEvent eventData)
    {
        var eto = new QuizClosedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.ClosedAt,
            QuizId = eventData.QuizId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            ClosedAt = eventData.ClosedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}