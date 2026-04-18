using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Learn.Components;

public partial class MaterialViewer : AbpComponentBase
{
    // ── Parameters ────────────────────────────────────────────────────────────

    /// <summary>The lesson whose file materials to display.</summary>
    [Parameter, EditorRequired] public LessonInChapterDto Lesson     { get; set; } = default!;
    [Parameter, EditorRequired] public Guid               CourseId   { get; set; }
    [Parameter, EditorRequired] public Guid               ChapterId  { get; set; }

    /// <summary>Optional callback for the "Done" button (pass EventCallback.Empty to hide it).</summary>
    [Parameter] public EventCallback OnDone { get; set; }

    // ── Services ──────────────────────────────────────────────────────────────

    [Inject] private IHttpClientFactory      HttpClientFactory      { get; set; } = default!;
    [Inject] private IAccessTokenProvider    AccessTokenProvider    { get; set; } = default!;
    [Inject] private IConfiguration          Configuration          { get; set; } = default!;
    [Inject] private IJSRuntime              JS                     { get; set; } = default!;

    // ── State ─────────────────────────────────────────────────────────────────

    private List<MaterialInLessonDto> _fileMaterials = new();
    private Guid? _downloadingId;

    protected override void OnParametersSet()
    {
        _fileMaterials = Lesson.Materials
            .Where(m => m.Type == MaterialType.File && m.Status == MaterialStatus.Active)
            .OrderBy(m => m.SortOrder)
            .ToList();
    }

    // ── Static helpers ────────────────────────────────────────────────────────

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)       return $"{bytes} B";
        if (bytes < 1048576)    return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1073741824) return $"{bytes / 1048576.0:F1} MB";
        return $"{bytes / 1073741824.0:F1} GB";
    }

    private static string GetFileIcon(string? fileName)
    {
        var ext = System.IO.Path.GetExtension(fileName ?? "").ToLowerInvariant();
        return ext switch
        {
            ".pdf"  => "fa-file-pdf-o",
            ".ppt" or ".pptx" => "fa-file-powerpoint-o",
            ".doc" or ".docx" => "fa-file-word-o",
            ".zip" or ".rar" or ".7z" => "fa-file-archive-o",
            _       => "fa-file-o",
        };
    }

    private static string GetIconClass(string? fileName)
    {
        var ext = System.IO.Path.GetExtension(fileName ?? "").ToLowerInvariant();
        return ext switch
        {
            ".pdf"  => "mv-item__icon--pdf",
            ".ppt" or ".pptx" => "mv-item__icon--ppt",
            ".doc" or ".docx" => "mv-item__icon--word",
            ".zip" or ".rar" or ".7z" => "mv-item__icon--zip",
            _       => "mv-item__icon--file",
        };
    }

    // ── Download ──────────────────────────────────────────────────────────────

    private async Task DownloadAsync(MaterialInLessonDto material)
    {
        if (_downloadingId.HasValue) return;
        _downloadingId = material.Id;
        try
        {
            var tokenResult = await AccessTokenProvider.RequestAccessToken();
            if (!tokenResult.TryGetToken(out var token))
            {
                await HandleErrorAsync(new Exception("Could not obtain access token."));
                return;
            }

            var baseUrl = (Configuration["RemoteServices:course-catalog:BaseUrl"]
                           ?? Configuration["RemoteServices:Default:BaseUrl"]!)
                          .TrimEnd('/');

            var input = new DownloadMaterialFileInput
            {
                CourseId   = CourseId,
                ChapterId  = ChapterId,
                LessonId   = Lesson.Id,
                MaterialId = material.Id
            };

            var json = JsonSerializer.Serialize(input);
            var httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Value);

            var response = await httpClient.PostAsync(
                $"{baseUrl}/api/course-catalog/course-catalog/download-material-file-student",
                new StringContent(json, Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();

            var bytes    = await response.Content.ReadAsByteArrayAsync();
            var base64   = Convert.ToBase64String(bytes);
            var mimeType = response.Content.Headers.ContentType?.MediaType
                           ?? "application/octet-stream";
            var fileName = material.FileName ?? material.Title;

            await JS.InvokeVoidAsync("downloadFileFromBytes", fileName, base64, mimeType);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _downloadingId = null;
            StateHasChanged();
        }
    }
}
