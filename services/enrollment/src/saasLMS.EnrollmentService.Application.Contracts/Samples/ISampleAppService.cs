using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace saasLMS.EnrollmentService.Samples;

public interface ISampleAppService : IApplicationService
{
    Task<SampleDto> GetAsync();

    Task<SampleDto> GetAuthorizedAsync();
}
