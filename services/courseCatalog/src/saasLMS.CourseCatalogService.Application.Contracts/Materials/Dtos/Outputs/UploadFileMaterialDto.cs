namespace saasLMS.CourseCatalogService.Materials.Dtos.Outputs;

public class UploadFileMaterialDto
{
    public string StorageKey { get; set; } = string.Empty;
    public string FileName   { get; set; } = string.Empty;
    public string MimeType   { get; set; } = string.Empty;
    public long   FileSize   { get; set; }
}