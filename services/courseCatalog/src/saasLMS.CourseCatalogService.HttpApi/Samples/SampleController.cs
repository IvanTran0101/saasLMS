using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Volo.Abp;

namespace saasLMS.CourseCatalogService.Samples;

[RemoteService(Name = CourseCatalogServiceRemoteServiceConsts.RemoteServiceName)]
[Area("CourseCatalogService")]
[ControllerName("CourseCatalogService")]
[Route("api/CourseCatalogService/sample")]
public class SampleController : CourseCatalogServiceController, ISampleAppService
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
