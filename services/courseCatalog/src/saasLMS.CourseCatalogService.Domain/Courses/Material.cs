using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace saasLMS.CourseCatalogService.Courses;

public class Material : Entity<Guid>
{
    public Guid TenantId { get; protected  set; }
    public Guid LessonId { get; protected set; }
    public string Title { get; protected set; }
    public MaterialType Type { get; protected set; }
    public string? StorageKey { get; protected set; }
    public string? FileName { get; protected set; }
    public string? MimeType { get; protected set; }
    public long? FileSize { get; protected set; }
    public string? ExternalUrl { get; protected set; }
    public string? TextContent { get; protected set; }
    public TextFormat? TextFormat { get; protected set; }
    public MaterialStatus Status { get; protected set; }
    
    protected  Material()
    {}

    public Material(Guid id, Guid tenantId, Guid lessonId, string title, MaterialType type, string? storageKey, string? fileName, string? mimeType, long? fileSize, string? externalUrl, string? textContent, TextFormat? textFormat) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentNullException("Tenant id cannot be empty", nameof(tenantId));
        }

        if (lessonId == Guid.Empty)
        {
            
            throw new ArgumentException("Lesson id cannot be empty", nameof(lessonId));
        }

        if (fileSize.HasValue && fileSize.Value < 0)
        {
            throw new ArgumentException("File size cannot be negative", nameof(fileSize));
        }
        
        TenantId = tenantId;
        LessonId = lessonId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Type = type;
        StorageKey = storageKey;
        FileName = fileName;
        MimeType = mimeType;
        FileSize = fileSize;
        ExternalUrl = externalUrl;
        TextContent = textContent;
        TextFormat = textFormat;
        Status = MaterialStatus.Active;

        ValidateContentByType();
    }

    public void Hide()
    {
        Status = MaterialStatus.Hidden;
    }

    public void Activate()
    {
        Status = MaterialStatus.Active;
    }

    public void SetFileContent(string storageKey,
        string? fileName,
        string? mimeType,
        long? fileSize)
    {
        StorageKey = Check.NotNullOrWhiteSpace(storageKey, nameof(storageKey));
        FileName = fileName;
        MimeType = mimeType;
        FileSize = fileSize;
        ExternalUrl = null;
        TextContent = null;
        TextFormat = null;
        Type = MaterialType.File;
    }

    private void ValidateContentByType()
    {
        throw new NotImplementedException();
    }
}