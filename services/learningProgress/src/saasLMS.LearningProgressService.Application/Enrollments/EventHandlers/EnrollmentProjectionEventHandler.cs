using System;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Etos.Enrollments;
using saasLMS.LearningProgressService.Enrollments;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;

namespace saasLMS.LearningProgressService.Enrollments.EventHandlers;

public class EnrollmentProjectionEventHandler
    : IDistributedEventHandler<StudentEnrolledEto>,
      IDistributedEventHandler<StudentUnenrolledEto>,
      ITransientDependency
{
    private readonly IEnrollmentProjectionRepository _enrollmentRepository;
    private readonly IGuidGenerator _guidGenerator;

    public EnrollmentProjectionEventHandler(
        IEnrollmentProjectionRepository enrollmentRepository,
        IGuidGenerator guidGenerator)
    {
        _enrollmentRepository = enrollmentRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task HandleEventAsync(StudentEnrolledEto eventData)
    {
        var projection = await _enrollmentRepository.FindByCourseAndStudentAsync(
            eventData.TenantId, eventData.CourseId, eventData.StudentId);

        if (projection == null)
        {
            projection = new EnrollmentProjection(
                _guidGenerator.Create(),
                eventData.TenantId,
                eventData.EnrollmentId,
                eventData.CourseId,
                eventData.StudentId,
                eventData.EnrolledAt);

            await _enrollmentRepository.InsertAsync(projection, autoSave: true);
            return;
        }

        projection.Activate(eventData.EnrollmentId, eventData.EnrolledAt);
        await _enrollmentRepository.UpdateAsync(projection, autoSave: true);
    }

    public async Task HandleEventAsync(StudentUnenrolledEto eventData)
    {
        var projection = await _enrollmentRepository.FindByCourseAndStudentAsync(
            eventData.TenantId, eventData.CourseId, eventData.StudentId);

        if (projection == null)
        {
            projection = new EnrollmentProjection(
                _guidGenerator.Create(),
                eventData.TenantId,
                eventData.EnrollmentId,
                eventData.CourseId,
                eventData.StudentId,
                eventData.CancelledAt,
                isActive: false);

            projection.Deactivate(eventData.CancelledAt);
            await _enrollmentRepository.InsertAsync(projection, autoSave: true);
            return;
        }

        projection.Deactivate(eventData.CancelledAt);
        await _enrollmentRepository.UpdateAsync(projection, autoSave: true);
    }
}
