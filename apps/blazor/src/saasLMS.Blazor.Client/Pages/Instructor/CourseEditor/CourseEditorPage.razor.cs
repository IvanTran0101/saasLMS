using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.Blazor.Client.Authorization;
using Microsoft.Extensions.Logging;
using saasLMS.AssessmentService.Assignments;
using saasLMS.AssessmentService.Quizzes;
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
    private IQuizAppService QuizAppService { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    // ── Child Component References ────────────────────────────────────────────────

    private AddResourcesToLessonModal _addResourceModal  = default!;
    private GradingModal              _gradingModal      = default!;
    private QuizResultModal           _quizResultModal   = default!;

    // ── Page State ────────────────────────────────────────────────────────────────

    private bool _isLoading;
    private bool _isSavingDraft;
    private bool _isPublishing;
    private bool _isHiding;

    /// <summary>ID của assignment đang trong quá trình Publish/Close (tránh double-click).</summary>
    private Guid? _processingAssignmentId;
    /// <summary>ID của assignment đang load full data để mở edit modal.</summary>
    private Guid? _loadingEditAssignmentId;

    private CourseDetailDto? _course;

    /// <summary>Key = LessonId, Value = assignments thuộc lesson đó (từ AssessmentService).</summary>
    private Dictionary<Guid, List<AssignmentListItemDto>> _assignmentsByLesson = new();

    /// <summary>Key = LessonId, Value = quizzes thuộc lesson đó (từ AssessmentService).</summary>
    private Dictionary<Guid, List<QuizListItemDto>> _quizzesByLesson = new();

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

            // Assignment và Quiz thuộc AssessmentService riêng — gọi độc lập, lỗi server-side
            // route thì chỉ silent empty, không ảnh hưởng việc load course.
            try
            {
                var assignments = await AssignmentAppService.GetListByCourseAsync(CourseId);
                _assignmentsByLesson = assignments
                    .GroupBy(a => a.LessonId)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (Exception)
            {
                _assignmentsByLesson = new Dictionary<Guid, List<AssignmentListItemDto>>();
            }

            try
            {
                var quizzes = await QuizAppService.GetListByCourseAsync(CourseId);
                _quizzesByLesson = quizzes
                    .GroupBy(q => q.LessonId)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch (Exception)
            {
                _quizzesByLesson = new Dictionary<Guid, List<QuizListItemDto>>();
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

    private async Task HideCourseAsync()
    {
        try
        {
            _isHiding = true;
            await CourseCatalogAppService.HideCourseAsync(
                new HideCourseInput { CourseId = CourseId });
            _course!.Status = CourseStatus.Hidden;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isHiding = false;
        }
    }

    private async Task ReopenCourseAsync()
    {
        try
        {
            _isPublishing = true;
            await CourseCatalogAppService.ReopenCourseAsync(
                new ReopenCourseInput { CourseId = CourseId });
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

    private void OpenEditMaterialModal(MaterialInLessonDto material, LessonInChapterDto lesson, ChapterDto chapter)
    {
        _addResourceModal.ShowEdit(_course!.CourseId, chapter.Id, lesson.Id, lesson.Title, material);
    }

    private async Task OnMaterialAddedAsync(MaterialDto material)
    {
        await LoadCourseAsync();
    }

    private async Task OnMaterialUpdatedAsync(MaterialDto material)
    {
        await LoadCourseAsync();
    }
    
    private Task OnAssignmentAddedAsync(AssignmentDto assignment)
    {
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

    private Task OnQuizAddedAsync(QuizDto quiz)
    {
        var item = new QuizListItemDto
        {
            Id               = quiz.Id,
            CourseId         = quiz.CourseId,
            LessonId         = quiz.LessonId,
            Title            = quiz.Title,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            MaxScore         = quiz.MaxScore,
            AttemptPolicy    = quiz.AttemptPolicy,
            Status           = quiz.Status
        };

        if (!_quizzesByLesson.TryGetValue(item.LessonId, out var list))
        {
            list = new List<QuizListItemDto>();
            _quizzesByLesson[item.LessonId] = list;
        }

        list.Add(item);
        StateHasChanged();
        return Task.CompletedTask;
    }

    private Task OnAssignmentUpdatedAsync(AssignmentDto assignment)
    {
        if (_assignmentsByLesson.TryGetValue(assignment.LessonId, out var list))
        {
            var item = list.FirstOrDefault(a => a.Id == assignment.Id);
            if (item != null)
            {
                item.Title       = assignment.Title;
                item.Deadline    = assignment.Deadline;
                item.MaxScore    = assignment.MaxScore;
                item.Status      = assignment.Status;
                item.PublishedAt = assignment.PublishedAt;
                item.ClosedAt    = assignment.ClosedAt;
            }
        }
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task PublishAssignmentAsync(AssignmentListItemDto asgn)
    {
        try
        {
            _processingAssignmentId = asgn.Id;
            await AssignmentAppService.PublishAsync(asgn.Id);
            asgn.Status      = AssignmentStatus.Published;
            asgn.PublishedAt = DateTime.Now;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _processingAssignmentId = null;
        }
    }

    private async Task CloseAssignmentAsync(AssignmentListItemDto asgn)
    {
        try
        {
            _processingAssignmentId = asgn.Id;
            await AssignmentAppService.CloseAsync(asgn.Id);
            asgn.Status   = AssignmentStatus.Closed;
            asgn.ClosedAt = DateTime.Now;
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _processingAssignmentId = null;
        }
    }

    private async Task PublishQuizAsync(QuizListItemDto quiz)
    {
        try
        {
            await QuizAppService.PublishAsync(quiz.Id);
            quiz.Status      = QuizStatus.Published;
            quiz.PublishedAt = DateTime.Now;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task CloseQuizAsync(QuizListItemDto quiz)
    {
        try
        {
            await QuizAppService.CloseAsync(quiz.Id);
            quiz.Status   = QuizStatus.Closed;
            quiz.ClosedAt = DateTime.Now;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task OpenEditAssignmentModalAsync(AssignmentListItemDto asgn, LessonInChapterDto lesson, ChapterDto chapter)
    {
        try
        {
            _loadingEditAssignmentId = asgn.Id;
            StateHasChanged();
            var full = await AssignmentAppService.GetAsync(asgn.Id);
            _addResourceModal.ShowEditAssignment(_course!.CourseId, chapter.Id, lesson.Id, lesson.Title, full);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _loadingEditAssignmentId = null;
        }
    }

    private void OpenGradingModal(AssignmentListItemDto asgn)
    {
        _gradingModal.Show(asgn.Id, asgn.Title, asgn.MaxScore);
    }

    private void OpenQuizResultModal(QuizListItemDto quiz)
    {
        _quizResultModal.Show(quiz.Id, quiz.Title, quiz.MaxScore);
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

    private static string GetAssignmentBadgeCss(AssignmentStatus status) => status switch
    {
        AssignmentStatus.Published => "cep-asgn-badge cep-asgn-badge--published",
        AssignmentStatus.Closed    => "cep-asgn-badge cep-asgn-badge--closed",
        _                          => "cep-asgn-badge cep-asgn-badge--draft"
    };

    // ── Quiz Helpers ──────────────────────────────────────────────────────────

    private IReadOnlyList<QuizListItemDto> GetLessonQuizzes(Guid lessonId) =>
        _quizzesByLesson.TryGetValue(lessonId, out var list) ? list : Array.Empty<QuizListItemDto>();

    private static string GetQuizStatusLabel(QuizStatus status) => status switch
    {
        QuizStatus.Published => "Published",
        QuizStatus.Closed    => "Closed",
        _                    => "Draft"
    };

    private static string GetQuizBadgeCss(QuizStatus status) => status switch
    {
        QuizStatus.Published => "cep-asgn-badge cep-asgn-badge--published",
        QuizStatus.Closed    => "cep-asgn-badge cep-asgn-badge--closed",
        _                    => "cep-asgn-badge cep-asgn-badge--draft"
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
    /// Chỉ áp dụng khi Status == Draft.
    /// </summary>
    private bool CanPublish =>
        _course is not null
        && _course.Status == CourseStatus.Draft
        && !string.IsNullOrWhiteSpace(_course.Title)
        && !string.IsNullOrWhiteSpace(_course.Description)
        && _course.Chapters.Any(c => c.Lessons.Count > 0);

    /// <summary>Course đang Hidden → cho phép Reopen (gọi lại Published).</summary>
    private bool CanReopen =>
        _course is not null && _course.Status == CourseStatus.Hidden;
}