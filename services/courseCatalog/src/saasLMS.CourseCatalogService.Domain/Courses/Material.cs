using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace saasLMS.CourseCatalogService.Courses;

public class Material : Entity<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid LessonId { get; protected set; }
    public string Title { get; protected set; }
    public MaterialType Type { get; protected set; }
    public int SortOrder { get; protected set; }
    public string? StorageKey { get; protected set; }
    public string? FileName { get; protected set; }
    public string? MimeType { get; protected set; }
    public long? FileSize { get; protected set; }
    public string? ExternalUrl { get; protected set; }
    public string? TextContent { get; protected set; }
    public TextFormat? TextFormat { get; protected set; }
    public MaterialStatus Status { get; protected set; }

    protected Material()
    {
        Title = string.Empty;
    }

    public Material(
        Guid id,
        Guid tenantId,
        Guid lessonId,
        string title,
        MaterialType type,
        int sortOrder,
        string? storageKey,
        string? fileName,
        string? mimeType,
        long? fileSize,
        string? externalUrl,
        string? textContent,
        TextFormat? textFormat) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        }

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson id cannot be empty.", nameof(lessonId));
        }

        if (sortOrder <= 0)
        {
            throw new ArgumentException("Sort order must be greater than 0.", nameof(sortOrder));
        }

        if (fileSize.HasValue && fileSize.Value < 0)
        {
            throw new ArgumentException("File size cannot be negative.", nameof(fileSize));
        }

        TenantId = tenantId;
        LessonId = lessonId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        Type = type;
        SortOrder = sortOrder;
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

    public void Rename(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    public void SetOrder(int sortOrder)
    {
        if (sortOrder <= 0)
        {
            throw new ArgumentException("Sort order must be greater than 0.", nameof(sortOrder));
        }

        SortOrder = sortOrder;
    }

    public void Hide()
    {
        if (Status == MaterialStatus.Hidden)
        {
            return;
        }

        Status = MaterialStatus.Hidden;
    }

    public void Activate()
    {
        if (Status == MaterialStatus.Active)
        {
            return;
        }

        Status = MaterialStatus.Active;
    }

    public void SetFileContent(string storageKey, string? fileName, string? mimeType, long? fileSize)
    {
        if (fileSize.HasValue && fileSize.Value < 0)
        {
            throw new ArgumentException("File size cannot be negative.", nameof(fileSize));
        }

        StorageKey = Check.NotNullOrWhiteSpace(storageKey, nameof(storageKey));
        FileName = fileName;
        MimeType = mimeType;
        FileSize = fileSize;
        ExternalUrl = null;
        TextContent = null;
        TextFormat = null;
        Type = MaterialType.File;
    }

    public void SetVideoLinkContent(string videoUrl)
    {
        ExternalUrl = Check.NotNullOrWhiteSpace(videoUrl, nameof(videoUrl));
        StorageKey = null;
        FileName = null;
        MimeType = null;
        FileSize = null;
        TextContent = null;
        TextFormat = null;
        Type = MaterialType.VideoLink;
    }

    public void SetTextContent(string textContent, TextFormat textFormat)
    {
        TextContent = Check.NotNullOrWhiteSpace(textContent, nameof(textContent));
        TextFormat = textFormat;
        StorageKey = null;
        FileName = null;
        MimeType = null;
        FileSize = null;
        ExternalUrl = null;
        Type = MaterialType.Text;
    }

    private void ValidateContentByType()
    {
        switch (Type)
        {
            case MaterialType.File:
                Check.NotNullOrWhiteSpace(StorageKey, nameof(StorageKey));
                break;

            case MaterialType.VideoLink:
                Check.NotNullOrWhiteSpace(ExternalUrl, nameof(ExternalUrl));
                break;

            case MaterialType.Text:
                Check.NotNullOrWhiteSpace(TextContent, nameof(TextContent));
                if (!TextFormat.HasValue)
                {
                    throw new BusinessException("CourseCatalog:TextFormatRequired");
                }
                break;

            default:
                throw new BusinessException("CourseCatalog:InvalidMaterialType");
        }
    }
}