using System;
using saasLMS.CourseCatalogService.Courses;

namespace saasLMS.CourseCatalogService.Events.Materials;

public class MaterialDeletedDomainEvent
{
    public Guid           TenantId    { get; }
    public Guid           CourseId    { get; }
    public Guid           ChapterId   { get; }
    public Guid           LessonId    { get; }
    public Guid           MaterialId  { get; }
    public string         Title       { get; }
    public MaterialType   Type        { get; }
    public MaterialStatus Status      { get; }
    public int            SortOrder   { get; }
    public string?        FileName    { get; }
    public string?        MimeType    { get; }
    public long?          FileSize    { get; }
    public string?        ExternalUrl { get; }
    public string?        TextContent { get; }
    public TextFormat?    TextFormat  { get; }

    public MaterialDeletedDomainEvent(
        Guid           tenantId,
        Guid           courseId,
        Guid           chapterId,
        Guid           lessonId,
        Guid           materialId,
        string         title,
        MaterialType   type,
        MaterialStatus status,
        int            sortOrder,
        string?        fileName,
        string?        mimeType,
        long?          fileSize,
        string?        externalUrl,
        string?        textContent,
        TextFormat?    textFormat)
    {
        TenantId    = tenantId;
        CourseId    = courseId;
        ChapterId   = chapterId;
        LessonId    = lessonId;
        MaterialId  = materialId;
        Title       = title;
        Type        = type;
        Status      = status;
        SortOrder   = sortOrder;
        FileName    = fileName;
        MimeType    = mimeType;
        FileSize    = fileSize;
        ExternalUrl = externalUrl;
        TextContent = textContent;
        TextFormat  = textFormat;
    }
}