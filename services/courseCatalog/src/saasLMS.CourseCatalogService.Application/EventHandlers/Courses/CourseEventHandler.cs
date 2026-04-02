using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Events.Courses;
using saasLMS.CourseCatalogService.Etos.Courses;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

namespace saasLMS.CourseCatalogService.EventHandlers.Courses;

public class CourseEventHandler
    : ILocalEventHandler<CourseCreatedDomainEvent>,
      ILocalEventHandler<CourseUpdatedDomainEvent>,
      ILocalEventHandler<CoursePublishedDomainEvent>,
      ILocalEventHandler<CourseHiddenDomainEvent>,
      ITransientDependency
{
    private readonly IDistributedEventBus _distributedEventBus;

    public CourseEventHandler(IDistributedEventBus distributedEventBus)
    {
        _distributedEventBus = distributedEventBus;
    }

    public async Task HandleEventAsync(CourseCreatedDomainEvent eventData)
    {
        var course = eventData.Course;

        await _distributedEventBus.PublishAsync(new CourseCreatedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = course.TenantId,
            CourseId     = course.Id,
            Title        = course.Title,
            Description  = course.Description,
            InstructorId = course.InstructorId,
            Status       = course.Status
        });
    }

    public async Task HandleEventAsync(CourseUpdatedDomainEvent eventData)
    {
        var course = eventData.Course;

        await _distributedEventBus.PublishAsync(new CourseUpdatedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = course.TenantId,
            CourseId     = course.Id,
            Title        = course.Title,
            Description  = course.Description,
            InstructorId = course.InstructorId,
            Status       = course.Status
        });
    }

    public async Task HandleEventAsync(CoursePublishedDomainEvent eventData)
    {
        var course = eventData.Course;

        await _distributedEventBus.PublishAsync(new CoursePublishedEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = course.TenantId,
            CourseId     = course.Id,
            Title        = course.Title,
            Description  = course.Description,
            InstructorId = course.InstructorId,
            Status       = course.Status
        });
    }

    public async Task HandleEventAsync(CourseHiddenDomainEvent eventData)
    {
        var course = eventData.Course;

        await _distributedEventBus.PublishAsync(new CourseHiddenEto
        {
            EventId      = Guid.NewGuid(),
            OccurredAt   = DateTime.UtcNow,
            TenantId     = course.TenantId,
            CourseId     = course.Id,
            Title        = course.Title,
            Description  = course.Description,
            InstructorId = course.InstructorId,
            Status       = course.Status
        });
    }
}