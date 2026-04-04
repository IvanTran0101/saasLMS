using Volo.Abp.Content;

namespace saasLMS.CourseCatalogService.Materials.Dtos.Inputs;

public class UploadFileMaterialInput
{
    public IRemoteStreamContent File { get; set; } = default!;
    public string? OldStorageKey { get; set; }
}