using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes.Events;
using saasLMS.AssessmentService.Quizzes.Etos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.EventBus.Local;

namespace saasLMS.AssessmentService.Quizzes.EventHandlers;

public class QuizPublishedDomainEventHandler :
    ILocalEventHandler<QuizPublishedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizPublishedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizPublishedDomainEvent eventData)
    {
        var eto = new QuizPublishedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.PublishedAt,
            QuizId = eventData.QuizId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            PublishedAt = eventData.PublishedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}