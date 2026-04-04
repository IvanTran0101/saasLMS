using System;
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Enrollments.Dtos.Outputs;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace saasLMS.EnrollmentService.Enrollments;

[RemoteService(Name = "EnrollmentService")]
public interface IEnrollmentAppService : IApplicationService
{
    Task<ActiveEnrollmentDto> CheckActiveEnrollmentAsync(Guid courseId, Guid studentId);
}
