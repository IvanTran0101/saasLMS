using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace saasLMS.CourseCatalogService.Courses;

public class Lesson : Entity<Guid>
{
    public Guid TenantId { get; protected set; }
    public Guid ChapterId { get; protected set; }
    public string Title { get; protected set; }
    public int SortOrder { get; protected set; }
    public LessonContentState ContentState { get; protected set; }

    public bool HasContent => ContentState == LessonContentState.HasContent;
    private readonly List<Material> _materials;
    public IReadOnlyCollection<Material> Materials => _materials.AsReadOnly();

    protected Lesson()
    {
        Title = string.Empty;
        _materials = new List<Material>();
    }

    public Lesson(Guid id, Guid tenantId, Guid chapterId, string title, int sortOrder) : base(id)
    {
        if (tenantId == Guid.Empty)
        {
            throw new ArgumentException("Tenant id cannot be empty.", nameof(tenantId));
        }

        if (chapterId == Guid.Empty)
        {
            throw new ArgumentException("Chapter id cannot be empty.", nameof(chapterId));
        }

        if (sortOrder <= 0)
        {
            throw new ArgumentException("Sort order must be greater than 0.", nameof(sortOrder));
        }

        TenantId = tenantId;
        ChapterId = chapterId;
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
        SortOrder = sortOrder;
        ContentState = LessonContentState.Empty;
        _materials = new List<Material>();
    }

    public void Rename(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));
    }

    public void UpdateDetails(string title, LessonContentState contentState)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title));

        if (!Enum.IsDefined(typeof(LessonContentState), contentState))
        {
            throw new ArgumentOutOfRangeException(nameof(contentState));
        }

        ContentState = contentState;
    }

    public void SetOrder(int sortOrder)
    {
        if (sortOrder <= 0)
        {
            throw new ArgumentException("Sort order must be greater than 0.", nameof(sortOrder));
        }

        SortOrder = sortOrder;
    }

    public void MarkAsContainingContent()
    {
        if (ContentState == LessonContentState.HasContent)
        {
            return;
        }

        ContentState = LessonContentState.HasContent;
    }

    public void MarkAsEmpty()
    {
        if (ContentState == LessonContentState.Empty)
        {
            return;
        }

        ContentState = LessonContentState.Empty;
    }
    public Material AddFileMaterial(
        Guid materialId,
        string title,
        string storageKey,
        string? fileName,
        string? mimeType,
        long? fileSize)
    {
        if (materialId == Guid.Empty)
        {
            throw new ArgumentException("Material id cannot be empty.", nameof(materialId));
        }

        var material = new Material(
            materialId,
            TenantId,
            Id,
            title,
            MaterialType.File,
            _materials.Count + 1,
            storageKey,
            fileName,
            mimeType,
            fileSize,
            null,
            null,
            null
        );

        _materials.Add(material);
        MarkAsContainingContent();
        return material;
    }
    public Material AddVideoLinkMaterial(
        Guid materialId,
        string title,
        string videoUrl)
    {
        if (materialId == Guid.Empty)
        {
            throw new ArgumentException("Material id cannot be empty.", nameof(materialId));
        }

        var material = new Material(
            materialId,
            TenantId,
            Id,
            title,
            MaterialType.VideoLink,
            _materials.Count + 1,
            null,
            null,
            null,
            null,
            videoUrl,
            null,
            null
        );

        _materials.Add(material);
        MarkAsContainingContent();
        return material;
    }
    public Material AddTextMaterial(
        Guid materialId,
        string title,
        string textContent,
        TextFormat textFormat)
    {
        if (materialId == Guid.Empty)
        {
            throw new ArgumentException("Material id cannot be empty.", nameof(materialId));
        }

        var material = new Material(
            materialId,
            TenantId,
            Id,
            title,
            MaterialType.Text,
            _materials.Count + 1,
            null,
            null,
            null,
            null,
            null,
            textContent,
            textFormat
        );

        _materials.Add(material);
        MarkAsContainingContent();
        return material;
    }

    public void RemoveMaterial(Guid materialId)
    {
        var material = _materials.FirstOrDefault(x => x.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        _materials.Remove(material);
        NormalizeMaterialsOrder();

        if (!_materials.Any())
        {
            MarkAsEmpty();
        }
    }

    public void HideMaterial(Guid materialId)
    {
        var material = GetMaterial(materialId);
        material.Hide();
    }

    public void ActivateMaterial(Guid materialId)
    {
        var material = GetMaterial(materialId);
        material.Activate();
        MarkAsContainingContent();
    }

    private Material GetMaterial(Guid materialId)
    {
        var material = _materials.FirstOrDefault(x => x.Id == materialId);
        if (material == null)
        {
            throw new BusinessException("CourseCatalog:MaterialNotFound");
        }

        return material;
    }

    private void NormalizeMaterialsOrder()
    {
        for (var i = 0; i < _materials.Count; i++)
        {
            _materials[i].SetOrder(i + 1);
        }
    }
}