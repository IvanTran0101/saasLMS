using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace saasLMS.AssessmentService.Courses;

[RemoteService(Name = "CourseCatalog")]
public interface ICourseCatalogAppService : IApplicationService
{
    Task<CourseOwnerDto> GetOwnerAsync(Guid id);
}
