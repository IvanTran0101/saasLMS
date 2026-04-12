using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using saasLMS.AssessmentService.Assignments;
using saasLMS.Blazor.Client.Components.Shared;
using saasLMS.CourseCatalogService.Chapters.Dtos.Inputs;
using saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Inputs;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Inputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Inputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Instructor.CourseEditor;

[Authorize]
public partial class CourseEditorPage : AbpComponentBase
{
    // ── Route Parameter ───────────────────────────────────────────────────────────

    [Parameter]
    public Guid CourseId { get; set; }

    // ── Dependencies ──────────────────────────────────────────────────────────────

    [Inject]
    private ICourseCatalogAppService CourseCatalogAppService { get; set; } = default!;

    [Inject]
    private IAssignmentAppService AssignmentAppService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    // ── Child Component References ────────────────────────────────────────────────

    private AddResourcesToLessonModal _addResourceModal = default!;

    // ── Page State ────────────────────────────────────────────────────────────────

    private bool _isLoading;
    private bool _isSavingDraft;
    private bool _isPublishing;

    private CourseDetailDto? _course;

    /// <summary>Key = LessonId, Value = assignments thuộc lesson đó (từ AssessmentService).</summary>
    private Dictionary<Guid, List<AssignmentListItemDto>> _assignmentsByLesson = new();

    // ── Inline Edit: Course Info ──────────────────────────────────────────────────

    private bool _isEditingCourseInfo;
    private string _editTitle       = string.Empty;
    private string _editDescription = string.Empty;
    private string? _titleError;

    // ── Inline Edit: Chapter ──────────────────────────────────────────────────────

    private Guid? _editingChapterId;
    private string _editingChapterTitle = string.Empty;

    // ── Inline Edit: Lesson ───────────────────────────────────────────────────────

    private Guid? _editingLessonId;
    private string _editingLessonTitle = string.Empty;

    // ── Collapse State ────────────────────────────────────────────────────────────

    /// <summary>Set ChapterId đang collapse. Mặc định tất cả mở.</summary>
    private readonly HashSet<Guid> _collapsedChapters = new();

    // ── Lifecycle ─────────────────────────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        await LoadCourseAsync();
    }

    // ── Load ──────────────────────────────────────────────────────────────────────

    private async Task LoadCourseAsync()
    {
        try
        {
            _isLoading = true;

            _course          = await CourseCatalogAppService.GetCourseDetailAsync(CourseId);
            _editTitle       = _course.Title;
            _editDescription = _course.Description ?? string.Empty;

            // Assignment thuộc AssessmentService riêng — gọi độc lập, lỗi server-side
            // route thì chỉ silent empty, không ảnh hưởng việc load course.
            try
            {
                var assignments = await AssignmentAppService.GetListByCourseAsync(CourseId);
                _assignmentsByLesson = assignments
                    .GroupBy(a => a.LessonId)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (Exception ex)
            {
                _assignmentsByLesson = new Dictionary<Guid, List<AssignmentListItemDto>>();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isLoading = false;
        }
    }

    // ── Course Info ───────────────────────────────────────────────────────────────

    private void BeginEditCourseInfo()
    {
        _titleError          = null;
        _editTitle           = _course!.Title;
        _editDescription     = _course.Description ?? string.Empty;
        _isEditingCourseInfo = true;
    }

    private void CancelEditCourseInfo()
    {
        _isEditingCourseInfo = false;
        _titleError          = null;
    }

    private async Task SaveCourseInfoAsync()
    {
        _titleError = null;

        if (string.IsNullOrWhiteSpace(_editTitle))
        {
            _titleError = "Course title is required.";
            return;
        }

        try
        {
            var input = new UpdateCourseInput
            {
                CourseId    = CourseId,
                Title       = _editTitle.Trim(),
                Description = string.IsNullOrWhiteSpace(_editDescription)
                    ? null
                    : _editDescription.Trim()
            };

            await CourseCatalogAppService.UpdateCourseAsync(input);

            _course!.Title      = input.Title;
            _course.Description = input.Description;
            _isEditingCourseInfo = false;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    // ── Save Draft / Publish ──────────────────────────────────────────────────────

    private async Task SaveDraftAsync()
    {
        if (_isEditingCourseInfo)
        {
            await SaveCourseInfoAsync();
        }
    }

    private async Task PublishCourseAsync()
    {
        try
        {
            _isPublishing = true;
            await CourseCatalogAppService.PublishCourseAsync(
                new PublishCourseInput { CourseId = CourseId });
            NavigationManager.NavigateTo("/instructor/dashboard");
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isPublishing = false;
        }
    }

    // ── Chapter ───────────────────────────────────────────────────────────────────

    private void ToggleChapter(Guid chapterId)
    {
        if (_collapsedChapters.Contains(chapterId))
            _collapsedChapters.Remove(chapterId);
        else
            _collapsedChapters.Add(chapterId);
    }

    private bool IsChapterCollapsed(Guid chapterId) => _collapsedChapters.Contains(chapterId);

    private async Task AddChapterAsync()
    {
        try
        {
            var input = new CreateChapterInput
            {
                CourseId = CourseId,
                Title    = $"Chapter {(_course!.Chapters.Count + 1):D2}"
            };

            var chapter = await CourseCatalogAppService.CreateChapterAsync(input);

            // ChapterDto không có CourseId — chỉ map các field thực sự tồn tại trên DTO
            _course!.Chapters.Add(new ChapterDto
            {
                Id      = chapter.Id,
                Title   = chapter.Title,
                OrderNo = chapter.OrderNo,
                Lessons = new List<LessonInChapterDto>()
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void BeginEditChapter(ChapterDto chapter)
    {
        _editingChapterId    = chapter.Id;
        _editingChapterTitle = chapter.Title;
    }

    private async Task SaveChapterTitleAsync(ChapterDto chapter)
    {
        if (string.IsNullOrWhiteSpace(_editingChapterTitle))
        {
            return;
        }

        try
        {
            var input = new RenameChapterInput
            {
                CourseId  = CourseId,
                ChapterId = chapter.Id,
                NewTitle  = _editingChapterTitle.Trim()   // ← đúng: NewTitle
            };

            await CourseCatalogAppService.RenameChapterAsync(input);
            chapter.Title     = input.NewTitle;
            _editingChapterId = null;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void CancelEditChapter() => _editingChapterId = null;

    private async Task RemoveChapterAsync(ChapterDto chapter)
    {
        try
        {
            await CourseCatalogAppService.RemoveChapterAsync(new RemoveChapterInput
            {
                CourseId  = CourseId,
                ChapterId = chapter.Id
            });

            _course!.Chapters.Remove(chapter);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    // ── Lesson ────────────────────────────────────────────────────────────────────

    private async Task AddLessonAsync(ChapterDto chapter)
    {
        try
        {
            var input = new CreateLessonInput
            {
                CourseId  = CourseId,
                ChapterId = chapter.Id,
                Title     = $"Lesson {(chapter.Lessons.Count + 1)}"
            };

            var lesson = await CourseCatalogAppService.CreateLessonAsync(input);

            chapter.Lessons.Add(new LessonInChapterDto
            {
                Id           = lesson.Id,
                ChapterId    = chapter.Id,
                Title        = lesson.Title,
                SortOrder    = lesson.SortOrder,
                ContentState = lesson.ContentState,
                Materials    = new List<MaterialInLessonDto>()
            });
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void BeginEditLesson(LessonInChapterDto lesson)
    {
        _editingLessonId    = lesson.Id;
        _editingLessonTitle = lesson.Title;
    }

    private async Task SaveLessonTitleAsync(LessonInChapterDto lesson, ChapterDto chapter)
    {
        if (string.IsNullOrWhiteSpace(_editingLessonTitle))
        {
            return;
        }

        try
        {
            var input = new RenameLessonInput
            {
                CourseId  = CourseId,
                ChapterId = chapter.Id,
                LessonId  = lesson.Id,
                NewTitle  = _editingLessonTitle.Trim()   // ← đúng: NewTitle
            };

            await CourseCatalogAppService.RenameLessonAsync(input);
            lesson.Title     = input.NewTitle;
            _editingLessonId = null;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private void CancelEditLesson() => _editingLessonId = null;

    private async Task RemoveLessonAsync(LessonInChapterDto lesson, ChapterDto chapter)
    {
        try
        {
            await CourseCatalogAppService.RemoveLessonAsync(new RemoveLessonInput
            {
                CourseId  = CourseId,
                ChapterId = chapter.Id,
                LessonId  = lesson.Id
            });

            chapter.Lessons.Remove(lesson);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    // ── Material ──────────────────────────────────────────────────────────────────

    private void OpenAddResourceModal(LessonInChapterDto lesson, ChapterDto chapter)
    {
        _addResourceModal.Show(_course!.CourseId, chapter.Id, lesson.Id, lesson.Title);
    }

    private async Task OnMaterialAddedAsync(MaterialDto material)
    {
        await LoadCourseAsync();
    }
    
    private Task OnAssignmentAddedAsync(AssignmentDto assignment)
    {
        // Assignment không thuộc CourseDetail nên không cần reload toàn bộ.
        // Chỉ thêm vào local dict để render ngay.
        var item = new AssignmentListItemDto
        {
            Id        = assignment.Id,
            CourseId  = assignment.CourseId,
            LessonId  = assignment.LessonId,
            Title     = assignment.Title,
            Deadline  = assignment.Deadline,
            MaxScore  = assignment.MaxScore,
            Status    = assignment.Status
        };

        if (!_assignmentsByLesson.TryGetValue(item.LessonId, out var list))
        {
            list = new List<AssignmentListItemDto>();
            _assignmentsByLesson[item.LessonId] = list;
        }

        list.Add(item);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task RemoveMaterialAsync(
        MaterialInLessonDto material,
        LessonInChapterDto lesson,
        ChapterDto chapter)
    {
        try
        {
            await CourseCatalogAppService.RemoveMaterialAsync(new RemoveMaterialInput
            {
                CourseId   = _course!.CourseId,
                ChapterId  = chapter.Id,
                LessonId   = lesson.Id,
                MaterialId = material.Id
            });

            // Cập nhật local state, tránh reload toàn trang
            lesson.Materials.RemoveAll(m => m.Id == material.Id);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    // ── Assignment Helpers ────────────────────────────────────────────────────────

    private IReadOnlyList<AssignmentListItemDto> GetLessonAssignments(Guid lessonId) =>
        _assignmentsByLesson.TryGetValue(lessonId, out var list) ? list : Array.Empty<AssignmentListItemDto>();

    private static string GetAssignmentStatusLabel(AssignmentStatus status) => status switch
    {
        AssignmentStatus.Published => "Published",
        AssignmentStatus.Closed    => "Closed",
        _                          => "Draft"
    };

    // ── Helpers ───────────────────────────────────────────────────────────────────

    private static string GetMaterialIcon(MaterialType type) => type switch
    {
        MaterialType.VideoLink => "fa fa-play-circle",
        MaterialType.File      => "fa fa-file-alt",
        MaterialType.Text      => "fa fa-align-left",
        _                      => "fa fa-paperclip"
    };

    private static string GetMaterialTypeLabel(MaterialType type) => type switch
    {
        MaterialType.VideoLink => "Video",
        MaterialType.File      => "File",
        MaterialType.Text      => "Text",
        _                      => "Resource"
    };

    private static string GetStatusBadgeCss(CourseStatus status) => status switch
    {
        CourseStatus.Published => "badge-published",
        CourseStatus.Hidden    => "badge-hidden",
        _                      => "badge-draft"
    };

    private static string GetStatusLabel(CourseStatus status) => status switch
    {
        CourseStatus.Published => "Published",
        CourseStatus.Hidden    => "Hidden",
        _                      => "Draft"
    };

    /// <summary>
    /// Theo Publish rule (CourseReadyToPublish):
    /// cần Title + Description + ít nhất 1 Chapter chứa ít nhất 1 Lesson.
    /// </summary>
    private bool CanPublish =>
        _course is not null
        && _course.Status != CourseStatus.Published
        && !string.IsNullOrWhiteSpace(_course.Title)
        && !string.IsNullOrWhiteSpace(_course.Description)
        && _course.Chapters.Any(c => c.Lessons.Count > 0);
}