using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp;
using Volo.Abp.Content;

namespace saasLMS.CourseCatalogService;

[RemoteService(Name = CourseCatalogServiceRemoteServiceConsts.RemoteServiceName)]
[Area(CourseCatalogServiceRemoteServiceConsts.RemoteServiceName)]
[Route("api/course-catalog/course-catalog")]
public class MaterialFileController : CourseCatalogServiceController
{
    private readonly CourseCatalogAppService _courseCatalogAppService;

    public MaterialFileController(CourseCatalogAppService courseCatalogAppService)
    {
        _courseCatalogAppService = courseCatalogAppService;
    }

    [HttpPost("upload-material-file")]
    [Consumes("multipart/form-data")]
    public async Task<MaterialDto> UploadMaterialFileAsync(
        [FromForm] UploadMaterialFileRequest input)
    {
        if (input.File == null || input.File.Length <= 0)
            throw new BusinessException("CourseCatalog:FileEmpty");

        using var stream = input.File.OpenReadStream();
        var remoteStream = new RemoteStreamContent(stream, input.File.FileName, input.File.ContentType);
        var uploadInput = new UploadMaterialFileInput
        {
            CourseId   = input.CourseId,
            ChapterId  = input.ChapterId,
            LessonId   = input.LessonId,
            MaterialId = input.MaterialId,
            Title      = input.Title
        };
        return await _courseCatalogAppService.UploadMaterialFileAsync(uploadInput, remoteStream);
    }
}
