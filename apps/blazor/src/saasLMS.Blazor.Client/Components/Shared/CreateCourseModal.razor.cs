using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Inputs;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;
using Volo.Abp.Users;

namespace saasLMS.Blazor.Client.Components.Shared;

public partial class CreateCourseModal : AbpComponentBase
{
    // ── Dependencies ─────────────────────────────────────────────────────────────

    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    // ── Parameters ────────────────────────────────────────────────────────────────

    /// <summary>Callback sau khi tạo course thành công — trả CourseDto về parent.</summary>
    [Parameter]
    public EventCallback<CourseDto> OnCourseCreated { get; set; }

    // ── State ─────────────────────────────────────────────────────────────────────

    private bool _isVisible;
    private bool _isSaving;

    private string _title       = string.Empty;
    private string _description = string.Empty;
    private string? _titleError;
    private string? _descriptionError;

    // ── Public API ────────────────────────────────────────────────────────────────

    /// <summary>Mở modal — được gọi từ parent bằng @ref.</summary>
    public void Show()
    {
        ResetForm();
        _isVisible = true;
        StateHasChanged();
    }

    // ── Private Methods ───────────────────────────────────────────────────────────

    private void ResetForm()
    {
        _title            = string.Empty;
        _description      = string.Empty;
        _titleError       = null;
        _descriptionError = null;
        _isSaving         = false;
    }

    private void Close() => _isVisible = false;

    private bool ValidateForm()
    {
        _titleError       = null;
        _descriptionError = null;
        var isValid       = true;

        if (string.IsNullOrWhiteSpace(_title))
        {
            _titleError = "Course title is required.";
            isValid     = false;
        }
        else if (_title.Trim().Length < 3)
        {
            _titleError = "Course title must be at least 3 characters.";
            isValid     = false;
        }

        if (string.IsNullOrWhiteSpace(_description))
        {
            _descriptionError = "Course description is required. A description is needed before publishing.";
            isValid           = false;
        }

        return isValid;
    }

    private async Task SaveDraftAsync()
    {
        if (!ValidateForm())
        {
            return;
        }

        try
        {
            _isSaving = true;

            var input = new CreateCourseInput
            {
                Title       = _title.Trim(),
                Description = _description.Trim()
            };

            var course = await CourseCatalogAppService.CreateCourseAsync(input);

            _isVisible = false;
            await OnCourseCreated.InvokeAsync(course);
            NavigationManager.NavigateTo("/instructor/dashboard");
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
}