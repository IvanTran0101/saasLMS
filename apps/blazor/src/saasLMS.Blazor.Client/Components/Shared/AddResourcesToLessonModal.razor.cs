using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Components.Shared;

/// <summary>Phân loại tab trong modal Add Resource to Lesson.</summary>
public enum MaterialTabType
{
    FileUpload,
    VideoLink,
    Text
}

public partial class AddResourcesToLessonModal : AbpComponentBase
{
    // ── Dependencies ──────────────────────────────────────────────────────────────

    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Inject]
    private IAccessTokenProvider AccessTokenProvider { get; set; } = default!;

    [Inject]
    private IConfiguration Configuration { get; set; } = default!;

    // ── Parameters ────────────────────────────────────────────────────────────────

    [Parameter]
    public EventCallback<MaterialDto> OnMaterialAdded { get; set; }

    // ── State ─────────────────────────────────────────────────────────────────────

    private bool _isVisible;
    private bool _isSaving;

    // Context của lesson đang mở modal
    private Guid _courseId;
    private Guid _chapterId;
    private Guid _lessonId;
    private string _lessonTitle = string.Empty;

    // Tab đang active
    private MaterialTabType _activeTab = MaterialTabType.FileUpload;

    // ── Fields chung ──────────────────────────────────────────────────────────────

    private string _resourceTitle = string.Empty;
    private string? _titleError;

    // ── Fields: File Upload ───────────────────────────────────────────────────────

    private IBrowserFile? _selectedFile;
    private string? _fileError;
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

    // ── Fields: Video Link ────────────────────────────────────────────────────────

    private string _videoUrl      = string.Empty;
    private string _videoNotes    = string.Empty;   // UI-only, không gửi lên server (DTO không có field này)
    private string? _videoUrlError;

    // ── Fields: Text ──────────────────────────────────────────────────────────────

    private string _textContent = string.Empty;
    private string? _textError;

    // ── Public API ────────────────────────────────────────────────────────────────

    /// <summary>Mở modal trong context của một lesson cụ thể.</summary>
    public void Show(Guid courseId, Guid chapterId, Guid lessonId, string lessonTitle)
    {
        _courseId    = courseId;
        _chapterId   = chapterId;
        _lessonId    = lessonId;
        _lessonTitle = lessonTitle;

        ResetForm();
        _isVisible = true;
        StateHasChanged();
    }

    // ── Private Methods ───────────────────────────────────────────────────────────

    private void ResetForm()
    {
        _activeTab     = MaterialTabType.FileUpload;
        _resourceTitle = string.Empty;
        _titleError    = null;
        _isSaving      = false;

        _selectedFile = null;
        _fileError    = null;

        _videoUrl      = string.Empty;
        _videoNotes    = string.Empty;
        _videoUrlError = null;

        _textContent = string.Empty;
        _textError   = null;
    }

    private void SelectTab(MaterialTabType tab)
    {
        _activeTab     = tab;
        _titleError    = null;
        _fileError     = null;
        _videoUrlError = null;
        _textError     = null;
    }

    private void Close() => _isVisible = false;

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        _fileError    = null;
        _selectedFile = e.File;

        if (_selectedFile.Size > MaxFileSizeBytes)
        {
            _fileError    = "File size exceeds the 50 MB limit.";
            _selectedFile = null;
        }
    }

    private bool ValidateCommonFields()
    {
        _titleError = null;
        if (string.IsNullOrWhiteSpace(_resourceTitle))
        {
            _titleError = "Resource title is required.";
            return false;
        }
        return true;
    }

    private async Task AddResourceAsync()
    {
        if (!ValidateCommonFields())
        {
            return;
        }

        try
        {
            _isSaving = true;

            MaterialDto result = _activeTab switch
            {
                MaterialTabType.FileUpload => await AddFileMaterialAsync(),
                MaterialTabType.VideoLink  => await AddVideoLinkMaterialAsync(),
                MaterialTabType.Text       => await AddTextMaterialAsync(),
                _                          => throw new InvalidOperationException("Unknown tab type.")
            };

            _isVisible = false;
            await OnMaterialAdded.InvokeAsync(result);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isSaving = false;
        }
    }

    private async Task<MaterialDto> AddFileMaterialAsync()
    {
        if (_selectedFile is null)
        {
            _fileError = "Please select a file.";
            throw new InvalidOperationException(_fileError);
        }

        // Single-step: truyền MaterialId = Guid.Empty để backend tạo mới Material
        // + upload blob trong một transaction — tránh domain check NotNullOrWhiteSpace(StorageKey).
        var tokenResult = await AccessTokenProvider.RequestAccessToken();
        if (!tokenResult.TryGetToken(out var token))
            throw new InvalidOperationException("Could not obtain access token for file upload.");

        var baseUrl = (Configuration["RemoteServices:course-catalog:BaseUrl"]
                       ?? Configuration["RemoteServices:Default:BaseUrl"]!)
                      .TrimEnd('/');

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(_courseId.ToString()),       "CourseId");
        form.Add(new StringContent(_chapterId.ToString()),      "ChapterId");
        form.Add(new StringContent(_lessonId.ToString()),       "LessonId");
        form.Add(new StringContent(Guid.Empty.ToString()),      "MaterialId"); // Empty = tạo mới
        form.Add(new StringContent(_resourceTitle.Trim()),      "Title");

        await using var stream = _selectedFile.OpenReadStream(MaxFileSizeBytes);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrEmpty(_selectedFile.ContentType)
                ? "application/octet-stream"
                : _selectedFile.ContentType);
        form.Add(fileContent, "File", _selectedFile.Name);

        var httpClient = HttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Value);

        var response = await httpClient.PostAsync(
            $"{baseUrl}/api/course-catalog/course-catalog/upload-material-file", form);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MaterialDto>(
            json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    private async Task<MaterialDto> AddVideoLinkMaterialAsync()
    {
        _videoUrlError = null;

        if (string.IsNullOrWhiteSpace(_videoUrl))
        {
            _videoUrlError = "Video URL is required.";
            throw new InvalidOperationException(_videoUrlError);
        }

        if (!Uri.TryCreate(_videoUrl.Trim(), UriKind.Absolute, out _))
        {
            _videoUrlError = "Please enter a valid URL.";
            throw new InvalidOperationException(_videoUrlError);
        }

        // FIX: field đúng là ExternalUrl, không phải VideoUrl
        var input = new CreateVideoLinkMaterialInput
        {
            CourseId    = _courseId,
            ChapterId   = _chapterId,
            LessonId    = _lessonId,
            Title       = _resourceTitle.Trim(),
            ExternalUrl = _videoUrl.Trim()
        };

        return await CourseCatalogAppService.CreateVideoLinkMaterialAsync(input);
    }

    private async Task<MaterialDto> AddTextMaterialAsync()
    {
        _textError = null;

        if (string.IsNullOrWhiteSpace(_textContent))
        {
            _textError = "Content cannot be empty.";
            throw new InvalidOperationException(_textError);
        }

        var input = new CreateTextMaterialInput
        {
            CourseId  = _courseId,
            ChapterId = _chapterId,
            LessonId  = _lessonId,
            Title     = _resourceTitle.Trim(),
            Content   = _textContent.Trim(),
            Format    = TextFormat.Plain
        };

        return await CourseCatalogAppService.CreateTextMaterialAsync(input);
    }
}