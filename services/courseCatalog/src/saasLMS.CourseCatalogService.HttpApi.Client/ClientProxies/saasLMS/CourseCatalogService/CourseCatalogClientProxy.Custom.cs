// This file adds missing proxy members for static client generation.
using System.Threading.Tasks;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.Content;
using Volo.Abp.Http.Client.ClientProxying;

// ReSharper disable once CheckNamespace
namespace saasLMS.CourseCatalogService;

public partial class CourseCatalogClientProxy
{
    public virtual async Task<MaterialDto> UploadMaterialFileAsync(
        UploadMaterialFileInput input,
        IRemoteStreamContent file)
    {
        return await RequestAsync<MaterialDto>(nameof(UploadMaterialFileAsync), new ClientProxyRequestTypeValue
        {
            { typeof(UploadMaterialFileInput), input },
            { typeof(IRemoteStreamContent), file }
        });
    }
}
