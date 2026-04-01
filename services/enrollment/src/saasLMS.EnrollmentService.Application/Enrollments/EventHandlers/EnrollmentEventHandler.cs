using System;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Enrollments.Events;
using saasLMS.EnrollmentService.Etos.Enrollments;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.EnrollmentService.Enrollments.EventHandlers;

public class EnrollmentEventHandler
    : ILocalEventHandler<StudentEnrolledDomainEvent>,
      ILocalEventHandler<StudentUnenrolledDomainEvent>,
      ILocalEventHandler<StudentEnrollmentCompletedDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public EnrollmentEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(StudentEnrolledDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new StudentEnrolledEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = eventData.Enrollment.TenantId,
            EnrollmentId = eventData.Enrollment.Id,
            CourseId     = eventData.Enrollment.CourseId,
            StudentId    = eventData.Enrollment.StudentId,
            EnrolledAt   = eventData.Enrollment.EnrolledAt
        });
    }

    public async Task HandleEventAsync(StudentUnenrolledDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new StudentUnenrolledEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = eventData.Enrollment.TenantId,
            EnrollmentId = eventData.Enrollment.Id,
            CourseId     = eventData.Enrollment.CourseId,
            StudentId    = eventData.Enrollment.StudentId,
            CancelledAt  = eventData.Enrollment.CancelledAt!.Value
        });
    }

    public async Task HandleEventAsync(StudentEnrollmentCompletedDomainEvent eventData)
    {
        await _distributedEventBus.PublishAsync(new StudentEnrollmentCompletedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = eventData.Enrollment.TenantId,
            EnrollmentId = eventData.Enrollment.Id,
            CourseId     = eventData.Enrollment.CourseId,
            StudentId    = eventData.Enrollment.StudentId,
            EnrolledAt   = eventData.Enrollment.EnrolledAt,
            CompletedAt  = eventData.Enrollment.CompletedAt!.Value
        });
    }
}