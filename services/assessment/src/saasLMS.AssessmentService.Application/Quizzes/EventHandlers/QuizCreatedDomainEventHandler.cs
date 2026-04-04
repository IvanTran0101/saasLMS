using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Quizzes.Etos;
using saasLMS.AssessmentService.Quizzes.Events;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.AssessmentService.Quizzes.EventHandlers;

public class QuizCreatedDomainEventHandler :
    ILocalEventHandler<QuizCreatedDomainEvent>,
    ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public QuizCreatedDomainEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(QuizCreatedDomainEvent eventData)
    {
        var eto = new QuizCreatedEto()
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.CreatedAt,
            QuizId = eventData.QuizId,
            CourseId = eventData.CourseId,
            LessonId = eventData.LessonId,
            Title = eventData.Title,
            TimeLimitMinutes = eventData.TimeLimitMinutes,
            MaxScore = eventData.MaxScore,
            AttemptPolicy = eventData.AttemptPolicy
            
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}