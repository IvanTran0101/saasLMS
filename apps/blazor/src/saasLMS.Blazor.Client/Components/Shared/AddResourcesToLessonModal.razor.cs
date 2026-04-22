using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Configuration;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Quizzes;
using saasLMS.AssessmentService.Shared;
using Volo.Abp.Content;

namespace saasLMS.Blazor.Client.Components.Shared;

public enum MaterialTabType
{
    FileUpload,
    VideoLink,
    Text,
    Assignment,
    Quiz
}

public partial class AddResourcesToLessonModal : AbpComponentBase
{
    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IAssignmentAppService AssignmentAppService { get; set; } = default!;

    [Inject]
    private IHttpClientFactory HttpClientFactory { get; set; } = default!;

    [Inject]
    private IAccessTokenProvider AccessTokenProvider { get; set; } = default!;

    [Inject]
    private IConfiguration Configuration { get; set; } = default!;

    // ── Parameters ────────────────────────────────────────────────────────────────

    [Parameter]
    public EventCallback<MaterialDto> OnMaterialAdded { get; set; }

    [Parameter]
    public EventCallback<MaterialDto> OnMaterialUpdated { get; set; }

    [Parameter]
    public EventCallback<AssignmentDto> OnAssignmentAdded { get; set; }

    [Parameter]
    public EventCallback<AssignmentDto> OnAssignmentUpdated { get; set; }

    [Parameter]
    public EventCallback<QuizDto> OnQuizAdded { get; set; }

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

    // ── Edit Mode: Material ───────────────────────────────────────────────────────

    private bool _isEditMode;
    private Guid _editingMaterialId;
    private MaterialType _editingMaterialType;
    private string _originalTitle = string.Empty;
    /// <summary>Filename hiển thị trong edit mode File (chỉ để UI reference).</summary>
    private string _existingFileName = string.Empty;

    // ── Edit Mode: Assignment ─────────────────────────────────────────────────────

    private bool _isEditAssignmentMode;
    private Guid _editingAssignmentId;

    // ── Fields chung ──────────────────────────────────────────────────────────────

    private string _resourceTitle = string.Empty;
    private string? _titleError;

    // ── Fields: File Upload ───────────────────────────────────────────────────────

    private IBrowserFile? _selectedFile;
    private string? _fileError;
    private const long MaxFileSizeBytes = 50 * 1024 * 1024; // 50 MB

    // ── Fields: Video Link ────────────────────────────────────────────────────────

    private string _videoUrl      = string.Empty;
    private string? _videoUrlError;

    // ── Fields: Text ──────────────────────────────────────────────────────────────

    private string _textContent = string.Empty;
    private string? _textError;
    
    // ── Fields: Assignment ────────────────────────────────────────────────────────

    private string?   _assignmentDescription;
    private DateTime? _assignmentDeadline;
    private decimal   _assignmentMaxScore;
    private string?   _assignmentDeadlineError;
    private string?   _assignmentMaxScoreError;

    // ── Fields: Quiz CSV Upload ───────────────────────────────────────────────────

    private IBrowserFile?              _quizCsvFile;
    private string?                    _quizCsvFileError;
    private string                     _quizTitle            = string.Empty;
    private string?                    _quizTitleError;
    private int?                       _quizTimeLimitMinutes;
    private string?                    _quizTimeLimitError;
    private bool                       _isQuizPreviewVisible;
    private QuizDto?                   _uploadedQuizDto;
    private List<QuizQuestionPreview>? _quizPreviewQuestions;

    private sealed class QuizQuestionPreview
    {
        public string Text    { get; set; } = string.Empty;
        public List<QuizAnswerPreview> Answers { get; set; } = new();
    }

    private sealed class QuizAnswerPreview
    {
        public string Text      { get; set; } = string.Empty;
        public bool   IsCorrect { get; set; }
    }

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

    // ── Public API ────────────────────────────────────────────────────────────────

    /// <summary>Mở modal ở chế độ Edit — load sẵn dữ liệu của material hiện có.</summary>
    public void ShowEdit(Guid courseId, Guid chapterId, Guid lessonId, string lessonTitle, MaterialInLessonDto material)
    {
        _courseId    = courseId;
        _chapterId   = chapterId;
        _lessonId    = lessonId;
        _lessonTitle = lessonTitle;

        ResetForm();

        _isEditMode          = true;
        _editingMaterialId   = material.Id;
        _editingMaterialType = material.Type;
        _originalTitle       = material.Title;
        _resourceTitle       = material.Title;

        _activeTab = material.Type switch
        {
            MaterialType.VideoLink => MaterialTabType.VideoLink,
            MaterialType.Text      => MaterialTabType.Text,
            _                      => MaterialTabType.FileUpload
        };

        switch (material.Type)
        {
            case MaterialType.VideoLink:
                _videoUrl = material.ExternalUrl ?? string.Empty;
                break;
            case MaterialType.Text:
                _textContent = material.Content ?? string.Empty;
                break;
            case MaterialType.File:
                _existingFileName = material.FileName ?? string.Empty;
                break;
        }

        _isVisible = true;
        StateHasChanged();
    }

    /// <summary>Mở modal ở chế độ Edit Assignment — load sẵn dữ liệu assignment.</summary>
    public void ShowEditAssignment(Guid courseId, Guid chapterId, Guid lessonId, string lessonTitle, AssignmentDto assignment)
    {
        _courseId    = courseId;
        _chapterId   = chapterId;
        _lessonId    = lessonId;
        _lessonTitle = lessonTitle;

        ResetForm();

        _isEditAssignmentMode    = true;
        _editingAssignmentId     = assignment.Id;
        _activeTab               = MaterialTabType.Assignment;
        _resourceTitle           = assignment.Title;
        _assignmentDescription   = assignment.Description;
        // Convert UTC → local so the datetime-local input displays the correct local time
        _assignmentDeadline      = assignment.Deadline.HasValue
            ? DateTime.SpecifyKind(assignment.Deadline.Value, DateTimeKind.Utc).ToLocalTime()
            : (DateTime?)null;
        _assignmentMaxScore      = assignment.MaxScore;

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

        _selectedFile     = null;
        _fileError        = null;
        _existingFileName = string.Empty;

        _videoUrl      = string.Empty;
        _videoUrlError = null;

        _textContent = string.Empty;
        _textError   = null;

        _assignmentDescription   = null;
        _assignmentDeadline      = null;
        _assignmentMaxScore      = 100m;
        _assignmentDeadlineError = null;
        _assignmentMaxScoreError = null;

        _quizCsvFile           = null;
        _quizCsvFileError      = null;
        _quizTitle             = string.Empty;
        _quizTitleError        = null;
        _quizTimeLimitMinutes  = null;
        _quizTimeLimitError    = null;
        _isQuizPreviewVisible  = false;
        _uploadedQuizDto       = null;
        _quizPreviewQuestions  = null;

        _isEditMode          = false;
        _editingMaterialId   = Guid.Empty;
        _editingMaterialType = default;
        _originalTitle       = string.Empty;

        _isEditAssignmentMode = false;
        _editingAssignmentId  = Guid.Empty;
    }

    private void SelectTab(MaterialTabType tab)
    {
        _activeTab     = tab;
        _titleError    = null;
        _fileError     = null;
        _videoUrlError = null;
        _textError     = null;
        
        _assignmentDeadlineError = null;
        _assignmentMaxScoreError = null;

        _quizCsvFileError = null;
    }

    private void Close() => _isVisible = false;

    private static readonly string[] AllowedMaterialExtensions = { ".pptx", ".docx", ".zip" };

    private void OnFileSelected(InputFileChangeEventArgs e)
    {
        _fileError    = null;
        _selectedFile = e.File;

        var ext = System.IO.Path.GetExtension(_selectedFile.Name).ToLowerInvariant();
        if (!Array.Exists(AllowedMaterialExtensions, x => x == ext))
        {
            _fileError    = "Invalid file format. Only .pptx, .docx, and .zip files are allowed.";
            _selectedFile = null;
            return;
        }

        if (_selectedFile.Size > MaxFileSizeBytes)
        {
            _fileError    = "File size exceeds the 50 MB limit.";
            _selectedFile = null;
        }
    }

    private bool ValidateCommonFields()
    {
        // Quiz tab derives title from the CSV filename — no UI title field needed.
        if (_activeTab == MaterialTabType.Quiz) return true;

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
        if (!ValidateCommonFields()) return;

        _isSaving = true;
        try
        {
            if (_isEditAssignmentMode)
            {
                await SaveEditAssignmentAsync();
            }
            else if (_isEditMode)
            {
                await SaveEditAsync();
            }
            else if (_activeTab == MaterialTabType.Assignment)
            {
                var assignment = await AddAssignmentAsync();
                _isVisible = false;
                await OnAssignmentAdded.InvokeAsync(assignment);
            }
            else if (_activeTab == MaterialTabType.Quiz)
            {
                await UploadQuizCsvAsync();
                // Modal visibility is managed inside UploadQuizCsvAsync.
            }
            else
            {
                MaterialDto material = _activeTab switch
                {
                    MaterialTabType.FileUpload => await AddFileMaterialAsync(),
                    MaterialTabType.VideoLink  => await AddVideoLinkMaterialAsync(),
                    MaterialTabType.Text       => await AddTextMaterialAsync(),
                    _                          => throw new InvalidOperationException("Unknown tab type.")
                };

                _isVisible = false;
                await OnMaterialAdded.InvokeAsync(material);
            }
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

    private async Task SaveEditAsync()
    {
        bool titleChanged = _resourceTitle.Trim() != _originalTitle;
        MaterialDto? result = null;

        // 1. Rename nếu title thay đổi
        if (titleChanged)
        {
            result = await CourseCatalogAppService.RenameMaterialAsync(new RenameMaterialInput
            {
                CourseId   = _courseId,
                ChapterId  = _chapterId,
                LessonId   = _lessonId,
                MaterialId = _editingMaterialId,
                NewTitle   = _resourceTitle.Trim()
            });
        }

        // 2. Cập nhật nội dung theo type
        switch (_editingMaterialType)
        {
            case MaterialType.VideoLink:
                _videoUrlError = null;
                if (string.IsNullOrWhiteSpace(_videoUrl))
                {
                    _videoUrlError = "Video URL is required.";
                    throw new InvalidOperationException(_videoUrlError);
                }
                result = await CourseCatalogAppService.UpdateVideoLinkMaterialAsync(
                    new UpdateVideoLinkMaterialInput
                    {
                        CourseId    = _courseId,
                        ChapterId   = _chapterId,
                        LessonId    = _lessonId,
                        MaterialId  = _editingMaterialId,
                        ExternalUrl = _videoUrl.Trim()
                    });
                break;

            case MaterialType.Text:
                _textError = null;
                if (string.IsNullOrWhiteSpace(_textContent))
                {
                    _textError = "Content cannot be empty.";
                    throw new InvalidOperationException(_textError);
                }
                result = await CourseCatalogAppService.UpdateTextMaterialAsync(
                    new UpdateTextMaterialInput
                    {
                        CourseId   = _courseId,
                        ChapterId  = _chapterId,
                        LessonId   = _lessonId,
                        MaterialId = _editingMaterialId,
                        Content    = _textContent.Trim(),
                        Format     = TextFormat.Plain
                    });
                break;

            case MaterialType.File:
                // Nếu user chọn file mới → re-upload; không thì chỉ rename (đã xử lý ở trên)
                if (_selectedFile != null)
                    result = await ReplaceFileMaterialAsync();
                break;
        }

        _isVisible = false;
        if (result != null)
            await OnMaterialUpdated.InvokeAsync(result);
    }

    /// <summary>Re-upload file cho material đã có (dùng existing MaterialId).</summary>
    private async Task<MaterialDto> ReplaceFileMaterialAsync()
    {
        var tokenResult = await AccessTokenProvider.RequestAccessToken();
        if (!tokenResult.TryGetToken(out var token))
            throw new InvalidOperationException("Could not obtain access token for file upload.");

        var baseUrl = (Configuration["RemoteServices:course-catalog:BaseUrl"]
                       ?? Configuration["RemoteServices:Default:BaseUrl"]!)
                      .TrimEnd('/');

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(_courseId.ToString()),            "CourseId");
        form.Add(new StringContent(_chapterId.ToString()),           "ChapterId");
        form.Add(new StringContent(_lessonId.ToString()),            "LessonId");
        form.Add(new StringContent(_editingMaterialId.ToString()),   "MaterialId");
        form.Add(new StringContent(_resourceTitle.Trim()),           "Title");

        await using var stream = _selectedFile!.OpenReadStream(MaxFileSizeBytes);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(
            string.IsNullOrEmpty(_selectedFile.ContentType) ? "application/octet-stream" : _selectedFile.ContentType);
        form.Add(fileContent, "File", _selectedFile.Name);

        var httpClient = HttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Value);

        var response = await httpClient.PostAsync(
            $"{baseUrl}/api/course-catalog/course-catalog/upload-material-file", form);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<MaterialDto>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
    }

    private async Task SaveEditAssignmentAsync()
    {
        _assignmentDeadlineError = null;
        _assignmentMaxScoreError = null;

        if (_assignmentMaxScore <= 0)
        {
            _assignmentMaxScoreError = "Max score must be greater than 0.";
            throw new InvalidOperationException(_assignmentMaxScoreError);
        }

        var input = new UpdateAssignmentDto
        {
            Title       = _resourceTitle.Trim(),
            Description = string.IsNullOrWhiteSpace(_assignmentDescription) ? null : _assignmentDescription.Trim(),
            // datetime-local gives Kind=Unspecified (local time) → convert to UTC before sending to server
            Deadline    = _assignmentDeadline.HasValue
                ? DateTime.SpecifyKind(_assignmentDeadline.Value, DateTimeKind.Local).ToUniversalTime()
                : (DateTime?)null,
            MaxScore    = _assignmentMaxScore
        };

        var result = await AssignmentAppService.UpdateAsync(_editingAssignmentId, input);
        _isVisible = false;
        await OnAssignmentUpdated.InvokeAsync(result);
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
    
    private async Task<AssignmentDto> AddAssignmentAsync()
    {
        _assignmentDeadlineError = null;
        _assignmentMaxScoreError = null;

        if (_assignmentMaxScore <= 0)
        {
            _assignmentMaxScoreError = "Max score must be greater than 0.";
            throw new InvalidOperationException(_assignmentMaxScoreError);
        }

        if (_assignmentDeadline.HasValue && _assignmentDeadline.Value <= DateTime.Now)
        {
            _assignmentDeadlineError = "Deadline must be a future date and time.";
            throw new InvalidOperationException(_assignmentDeadlineError);
        }

        var input = new CreateAssignmentDto
        {
            CourseId    = _courseId,
            LessonId    = _lessonId,
            Title       = _resourceTitle.Trim(),
            Description = string.IsNullOrWhiteSpace(_assignmentDescription)
                ? null
                : _assignmentDescription.Trim(),
            // datetime-local gives Kind=Unspecified (local time) → convert to UTC before sending to server
            Deadline    = _assignmentDeadline.HasValue
                ? DateTime.SpecifyKind(_assignmentDeadline.Value, DateTimeKind.Local).ToUniversalTime()
                : (DateTime?)null,
            MaxScore    = _assignmentMaxScore
        };

        return await AssignmentAppService.CreateAsync(input);
    }

    // ── Quiz CSV Upload ───────────────────────────────────────────────────────────

    private void OnQuizCsvFileSelected(InputFileChangeEventArgs e)
    {
        _quizCsvFileError = null;
        _quizCsvFile      = e.File;

        var ext = System.IO.Path.GetExtension(_quizCsvFile.Name).ToLowerInvariant();
        if (ext != ".csv")
        {
            _quizCsvFileError = "Invalid file format. Only .csv files are accepted for quiz upload.";
            _quizCsvFile      = null;
            return;
        }

        if (_quizCsvFile.Size > MaxFileSizeBytes)
        {
            _quizCsvFileError = "File size exceeds the 50 MB limit.";
            _quizCsvFile      = null;
        }
    }

    private async Task UploadQuizCsvAsync()
    {
        _quizCsvFileError   = null;
        _quizTitleError     = null;
        _quizTimeLimitError = null;

        if (_quizCsvFile is null)
        {
            _quizCsvFileError = "Please select a CSV file.";
            return;
        }

        if (_quizTimeLimitMinutes.HasValue && _quizTimeLimitMinutes.Value <= 0)
        {
            _quizTimeLimitError = "Time limit must be a positive number.";
            return;
        }

        var tokenResult = await AccessTokenProvider.RequestAccessToken();
        if (!tokenResult.TryGetToken(out var token))
            throw new InvalidOperationException("Could not obtain access token for quiz upload.");

        var baseUrl = (Configuration["RemoteServices:AssessmentService:BaseUrl"]
                       ?? Configuration["RemoteServices:Default:BaseUrl"]!)
                      .TrimEnd('/');

        // Use the instructor-provided title; fall back to the CSV filename
        var title = string.IsNullOrWhiteSpace(_quizTitle)
            ? System.IO.Path.GetFileNameWithoutExtension(_quizCsvFile.Name)
            : _quizTitle.Trim();
        if (string.IsNullOrWhiteSpace(title)) title = "Quiz";

        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(_courseId.ToString()), "CourseId");
        form.Add(new StringContent(_lessonId.ToString()), "LessonId");
        form.Add(new StringContent(title),                "Title");
        form.Add(new StringContent("100"),                "MaxScore");
        form.Add(new StringContent("0"),                  "AttemptPolicy"); // OneTime
        if (_quizTimeLimitMinutes.HasValue)
            form.Add(new StringContent(_quizTimeLimitMinutes.Value.ToString()), "TimeLimitMinutes");

        await using var stream = _quizCsvFile.OpenReadStream(MaxFileSizeBytes);
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        form.Add(fileContent, "File", _quizCsvFile.Name);

        var httpClient = HttpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Value);

        var response = await httpClient.PostAsync(
            $"{baseUrl}/api/assessment/quiz/upload-csv", form);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _quizCsvFileError = ExtractAbpErrorMessage(errorBody)
                ?? "Failed to upload quiz CSV. Please verify the file format and try again.";
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        _uploadedQuizDto = JsonSerializer.Deserialize<QuizDto>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        // Parse QuestionsJson for preview display (quiz is Draft, GetFormSchema requires Published)
        _quizPreviewQuestions = JsonSerializer.Deserialize<List<QuizQuestionPreview>>(
            _uploadedQuizDto.QuestionsJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

        _isVisible            = false;
        _isQuizPreviewVisible = true;
    }

    /// <summary>Cancel từ preview → quay lại modal chính để upload CSV khác.</summary>
    private void CloseQuizPreview()
    {
        _isQuizPreviewVisible  = false;
        _quizCsvFile           = null;
        _quizCsvFileError      = null;
        _uploadedQuizDto       = null;
        _quizPreviewQuestions  = null;
        _isVisible             = true;
    }

    /// <summary>Confirm từ preview → đóng hoàn toàn, bắn callback.</summary>
    private async Task ConfirmQuizAsync()
    {
        var dto = _uploadedQuizDto;
        _isQuizPreviewVisible = false;
        _isVisible            = false;
        ResetForm();
        if (dto != null)
            await OnQuizAdded.InvokeAsync(dto);
    }

    /// <summary>
    /// Parse ABP HTTP error response body and extract the error code.
    /// ABP format: { "error": { "code": "...", "message": "...", "details": "..." } }
    /// </summary>
    private static string? ExtractAbpErrorMessage(string responseBody)
    {
        try
        {
            var node = JsonNode.Parse(responseBody);
            var error = node?["error"];
            if (error is null) return null;

            var code = error["code"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(code))
                return code;
        }
        catch { /* ignore parse errors */ }
        return null;
    }
}