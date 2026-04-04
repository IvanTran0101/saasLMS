using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes.Etos;
using saasLMS.AssessmentService.Quizzes.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Quizzes.EventHandlers;

public class QuizUpdatedDomainEventHandler :
    ILocalEventHandler<QuizUpdatedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizUpdatedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizUpdatedDomainEvent eventData)
    {
        var eto = new QuizUpdatedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.UpdatedAt,
            QuizId = eventData.QuizId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            Title = eventData.Title,
            TimeLimitMinutes = eventData.TimeLimitMinutes,
            MaxScore = eventData.MaxScore,
            AttemptPolicy = eventData.AttemptPolicy,
            UpdatedAt = eventData.UpdatedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}

