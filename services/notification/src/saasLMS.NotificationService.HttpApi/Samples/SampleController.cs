using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Volo.Abp;

namespace saasLMS.NotificationService.Samples;

[RemoteService(Name = NotificationServiceRemoteServiceConsts.RemoteServiceName)]
[Area("NotificationService")]
[ControllerName("NotificationService")]
[Route("api/NotificationService/sample")]
public class SampleController : NotificationServiceController, ISampleAppService
{
    private readonly ISampleAppService _sampleAppService;

    public SampleController(ISampleAppService sampleAppService)
    {
        _sampleAppService = sampleAppService;
    }

    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
