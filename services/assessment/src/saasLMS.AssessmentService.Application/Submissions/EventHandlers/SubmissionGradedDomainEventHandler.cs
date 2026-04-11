using System;
using System.Threading.Tasks;
using saasLMS.AssessmentService.Assignments;
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
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IDistributedEventBus _distributedEventBus;

    public SubmissionGradedDomainEventHandler(
        IAssignmentRepository assignmentRepository,
        IDistributedEventBus distributedEventBus)
    {
        _assignmentRepository = assignmentRepository;
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(SubmissionGradedDomainEvent eventData)
    {
        var assignment = await _assignmentRepository.GetAsync(eventData.AssignmentId);
        var eto = new SubmissionGradedEto
        {
            EventId = Guid.NewGuid(),
            TenantId = eventData.TenantId,
            OccurredAt = eventData.GradedAt,
            SubmissionId = eventData.SubmissionId,
            AssignmentId = eventData.AssignmentId,
            CourseId = assignment.CourseId,
            StudentId = eventData.StudentId,
            Score = eventData.Score,
            GradedAt = eventData.GradedAt
        };

        await _distributedEventBus.PublishAsync(eto);
    }
}
