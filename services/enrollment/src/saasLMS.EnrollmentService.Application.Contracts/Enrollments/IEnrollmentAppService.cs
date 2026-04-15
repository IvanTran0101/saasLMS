using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace saasLMS.EnrollmentService.Enrollments;

[RemoteService(Name = EnrollmentServiceRemoteServiceConsts.RemoteServiceName)]
public interface IEnrollmentAppService : IApplicationService
{
    Task<EnrollmentDto> EnrollAsync(EnrollCourseInput input);
    Task<EnrollmentDto> CancelAsync(CancelEnrollmentInput input);
 
    Task<EnrollmentDto> GetByIdAsync(Guid id);
    Task<EnrollmentDto?> FindByCourseAsync(Guid courseId);
    Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync(GetMyEnrollmentsInput input);
    Task<List<EnrollmentListItemDto>> GetEnrollmentsByCourseAsync(Guid courseId);
    Task<ActiveEnrollmentDto> CheckActiveEnrollmentAsync(Guid courseId, Guid studentId);
}