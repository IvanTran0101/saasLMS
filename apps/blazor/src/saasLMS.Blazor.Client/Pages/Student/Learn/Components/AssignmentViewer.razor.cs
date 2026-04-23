using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Shared;
using saasLMS.AssessmentService.Submissions;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Learn.Components;

public partial class AssignmentViewer : AbpComponentBase
{
    private const long MaxFileSizeBytes = 20 * 1024 * 1024; // 50 MB

    // ── Parameters ────────────────────────────────────────────────────────────

    /// <summary>ID of the assignment to display.</summary>
    [Parameter, EditorRequired] public Guid AssignmentId { get; set; }

    /// <summary>Optional callback for the "Done" button.</summary>
    [Parameter] public EventCallback OnDone { get; set; }

    // ── Services ──────────────────────────────────────────────────────────────

    [Inject] private IAssignmentAppService    AssignmentAppService  { get; set; } = default!;
    [Inject] private ISubmissionAppService    SubmissionAppService  { get; set; } = default!;
    [Inject] private IHttpClientFactory       HttpClientFactory     { get; set; } = default!;
    [Inject] private IAccessTokenProvider     AccessTokenProvider   { get; set; } = default!;
    [Inject] private IConfiguration           Configuration         { get; set; } = default!;
    [Inject] private IJSRuntime               JS                    { get; set; } = default!;

    // ── State ─────────────────────────────────────────────────────────────────

    private AssignmentDto?  _assignment;
    private SubmissionDto?  _submission;

    private bool _isLoading    = true;
    private bool _isEditing    = false;
    private bool _isSubmitting = false;
    private bool _isDownloading = false;

    /// <summary>Evaluated once on load. True when deadline has already passed.</summary>
    private bool _isExpired;

    /// <summary>
    /// True when the assignment cannot accept new submissions (closed or expired on load).
    /// Controls UI visibility of upload zone and edit button.
    /// </summary>
    private bool _isLocked => _assignment?.Status == AssignmentStatus.Closed || _isExpired;

    /// <summary>
    /// Set when Submit is clicked but a runtime check finds the deadline has since
    /// passed or the assignment was closed — shown as a warning without hiding the
    /// upload zone (which disappears only on reload).
    /// </summary>
    private string? _submitWarning;

    private IBrowserFile? _selectedFile;
    private string?       _fileError;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    protected override async Task OnParametersSetAsync()
    {
        _isLoading     = true;
        _isEditing     = false;
        _isExpired     = false;
        _submitWarning = null;
        ClearFile();

        try
        {
            _assignment = await AssignmentAppService.GetStudentAsync(AssignmentId);

            // Evaluate expiry once at load time so the UI is stable.
            // (The runtime Submit check uses DateTime.UtcNow independently.)
            if (_assignment?.Deadline.HasValue == true)
                _isExpired = DateTime.UtcNow > EnsureUtc(_assignment.Deadline.Value);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }

        try
        {
            _submission = await SubmissionAppService.GetMySubmissionByAssignmentAsync(AssignmentId);
        }
        catch
        {
            // No submission yet — expected for students who haven't submitted.
            _submission = null;
        }
        finally
        {
            _isLoading = false;
        }
    }

    // ── File selection ────────────────────────────────────────────────────────

    private static readonly string[] AllowedSubmissionExtensions = { ".pptx", ".docx", ".zip" };

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        _fileError    = null;
        _selectedFile = e.File;

        var ext = System.IO.Path.GetExtension(_selectedFile.Name).ToLowerInvariant();
        if (!Array.Exists(AllowedSubmissionExtensions, x => x == ext))
        {
            _fileError    = "Invalid file format. Only .pptx, .docx, and .zip files are allowed.";
            _selectedFile = null;
            return;
        }

        if (_selectedFile.Size > MaxFileSizeBytes)
        {
            _fileError    = $"File is too large. Maximum size is {MaxFileSizeBytes / 1024 / 1024} MB.";
            _selectedFile = null;
        }
    }

    private void ClearFile()
    {
        _selectedFile = null;
        _fileError    = null;
    }

    // ── Submit ────────────────────────────────────────────────────────────────

    private async Task SubmitAsync()
    {
        if (_selectedFile is null || _isSubmitting) return;

        // Runtime re-check: deadline may have passed since the page was loaded
        // (stale page scenario). Show a warning instead of attempting the upload.
        if (_assignment?.Status == AssignmentStatus.Closed)
        {
            _submitWarning = "This assignment has been closed. No more submissions are accepted.";
            return;
        }
        if (_assignment?.Deadline.HasValue == true &&
            DateTime.UtcNow > EnsureUtc(_assignment.Deadline.Value))
        {
            _submitWarning = "The submission deadline has passed. Please reload the page.";
            return;
        }

        _submitWarning = null;
        _fileError     = null;
        _isSubmitting  = true;

        try
        {
            var tokenResult = await AccessTokenProvider.RequestAccessToken();
            if (!tokenResult.TryGetToken(out var token))
                throw new Exception("Could not obtain access token.");

            var baseUrl = (Configuration["RemoteServices:AssessmentService:BaseUrl"]
                           ?? Configuration["RemoteServices:Default:BaseUrl"]!)
                          .TrimEnd('/');

            var httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Value);

            // Build multipart: AssignmentId + file
            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(AssignmentId.ToString()), "AssignmentId");

            await using var stream      = _selectedFile.OpenReadStream(MaxFileSizeBytes);
            var             fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(
                string.IsNullOrWhiteSpace(_selectedFile.ContentType)
                    ? "application/octet-stream"
                    : _selectedFile.ContentType);
            form.Add(fileContent, "File", _selectedFile.Name);

            var response = await httpClient.PostAsync(
                $"{baseUrl}/api/assessment/submission/submit-submission-file", form);

            response.EnsureSuccessStatusCode();

            var json    = await response.Content.ReadAsStringAsync();
            _submission = JsonSerializer.Deserialize<SubmissionDto>(
                json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            _isEditing = false;
            ClearFile();

            if (OnDone.HasDelegate)
                await OnDone.InvokeAsync();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    // ── Static helpers ────────────────────────────────────────────────────────

    private static string FormatFileSize(long bytes)
    {
        if (bytes < 1024)       return $"{bytes} B";
        if (bytes < 1048576)    return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1073741824) return $"{bytes / 1048576.0:F1} MB";
        return $"{bytes / 1073741824.0:F1} GB";
    }

    /// <summary>
    /// Guarantees a DateTime is treated as UTC before any local-time conversion.
    /// ABP/JSON deserialization may leave Kind as Unspecified for UTC values,
    /// which would make ToLocalTime() return the wrong result for GMT+7 users.
    /// </summary>
    private static DateTime EnsureUtc(DateTime dt)
        => dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    /// <summary>Converts a UTC server timestamp to the browser's local time (e.g. GMT+7).</summary>
    private static string ToLocalDisplay(DateTime dt, string format = "MMM dd, yyyy HH:mm")
        => EnsureUtc(dt).ToLocalTime().ToString(format);

    // ── Download my submission ────────────────────────────────────────────────

    private async Task DownloadMySubmissionAsync()
    {
        if (_submission is null || _isDownloading) return;
        _isDownloading = true;

        try
        {
            var tokenResult = await AccessTokenProvider.RequestAccessToken();
            if (!tokenResult.TryGetToken(out var token))
                throw new Exception("Could not obtain access token.");

            var baseUrl = (Configuration["RemoteServices:AssessmentService:BaseUrl"]
                           ?? Configuration["RemoteServices:Default:BaseUrl"]!)
                          .TrimEnd('/');

            var httpClient = HttpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Value);

            using var body = new StringContent(
                JsonSerializer.Serialize(new { submissionId = _submission.Id }),
                Encoding.UTF8,
                "application/json");
            var response = await httpClient.PostAsync(
                $"{baseUrl}/api/assessment/submission/download-my-submission-file", body);

            response.EnsureSuccessStatusCode();

            var bytes    = await response.Content.ReadAsByteArrayAsync();
            var base64   = Convert.ToBase64String(bytes);
            var mimeType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var fileName = _submission.FileName ?? "submission";

            await JS.InvokeVoidAsync("downloadFileFromBytes", fileName, base64, mimeType);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isDownloading = false;
        }
    }
}
