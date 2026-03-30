using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using saasLMS.EnrollmentService.Enrollments.Dtos.Inputs;
using saasLMS.EnrollmentService.Enrollments.Dtos.Outputs;
using Volo.Abp.Application.Services;

namespace saasLMS.EnrollmentService.Enrollments;

public interface IEnrollmentAppService : IApplicationService
{
    Task<EnrollmentDto> EnrollAsync(EnrollCourseInput input);
    Task<EnrollmentDto> CancelAsync(CancelEnrollmentInput input);
 
    Task<EnrollmentDto> GetByIdAsync(Guid id);
    Task<EnrollmentDto?> FindByCourseAsync(Guid courseId);
    Task<List<EnrollmentListItemDto>> GetMyEnrollmentsAsync();
    Task<List<EnrollmentListItemDto>> GetEnrollmentsByCourseAsync(Guid courseId);
}