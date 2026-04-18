using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using saasLMS.AssessmentService.Assignments;
using saasLMS.Blazor.Client.Authorization;
using saasLMS.CourseCatalogService.Courses;
using saasLMS.CourseCatalogService.Courses.Dtos.Outputs;
using saasLMS.CourseCatalogService.Chapters.Dtos.Outputs;
using saasLMS.CourseCatalogService.Lessons.Dtos.Outputs;
using saasLMS.CourseCatalogService.Materials.Dtos.Outputs;
using saasLMS.LearningProgressService.LessonProgresses;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Inputs;
using saasLMS.LearningProgressService.LessonProgresses.Dtos.Outputs;
using saasLMS.LearningProgressService.CourseProgresses.Dtos.Outputs;
using Volo.Abp.AspNetCore.Components;

namespace saasLMS.Blazor.Client.Pages.Student.Learn;

[Authorize]
public partial class LessonViewerPage : AbpComponentBase
{
    // ── Route parameters ──────────────────────────────────────────────────────

    [Parameter] public Guid  CourseId { get; set; }
    [Parameter] public Guid? LessonId { get; set; }

    // ── Injected services ─────────────────────────────────────────────────────

    [Inject] private ICourseCatalogAppService     CourseCatalogAppService     { get; set; } = default!;
    [Inject] private ILearningProgressAppService  LearningProgressAppService  { get; set; } = default!;
    [Inject] private IAssignmentAppService        AssignmentAppService        { get; set; } = default!;
    [Inject] private NavigationManager            NavigationManager           { get; set; } = default!;

    // ── Raw data ──────────────────────────────────────────────────────────────

    private CourseDetailDto?          _courseDetail;
    private List<LessonProgressDto>   _lessonProgresses   = new();
    private ResumeResultDto?          _resumeResult;
    private CourseProgressDto?        _courseProgress;

    // ── Derived state ─────────────────────────────────────────────────────────

    private HashSet<Guid>             _completedLessonIds = new();
    private HashSet<Guid>             _expandedChapters   = new();
    private HashSet<Guid>             _expandedLessons    = new();
    private List<LessonInChapterDto>  _flatLessons        = new();

    private Dictionary<Guid, List<AssignmentListItemDto>> _assignmentsByLesson = new();

    private ChapterDto?              _selectedChapter;
    private LessonInChapterDto?      _selectedLesson;
    private LessonInChapterDto?      _prevLesson;
    private LessonInChapterDto?      _nextLesson;

    // ── Resource selection state ──────────────────────────────────────────────

    /// Currently viewed individual material (null = show lesson overview).
    private MaterialInLessonDto?      _selectedMaterial;

    /// Currently viewed assignment (null = show lesson overview).
    private AssignmentListItemDto?    _selectedAssignment;

    // ── Computed lesson status flags ──────────────────────────────────────────

    private bool _hasCourseStarted => _completedLessonIds.Count > 0 || _resumeResult?.LessonStatus == LessonProgressStatus.InProgress;
    private bool _isResumeLesson   => _selectedLesson is not null && _resumeResult?.LessonId == _selectedLesson.Id;
    private bool _isLessonCompleted => _selectedLesson is not null && _completedLessonIds.Contains(_selectedLesson.Id);

    // ── UI state ──────────────────────────────────────────────────────────────

    private bool _isLoading = true;
    private bool _isActing  = false;

    private int _progressPct => _courseProgress is null ? 0 : (int)Math.Round(_courseProgress.ProgressPercent);

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    protected override async Task OnInitializedAsync()
    {
        if (!CurrentUser.IsInRole(LmsRoles.Student))
        {
            NavigationManager.NavigateTo("/");
            return;
        }

        await LoadAllAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (!_isLoading)
            await SelectLessonFromRouteParamAsync();
    }

    // ── Data loading ──────────────────────────────────────────────────────────

    private async Task LoadAllAsync()
    {
        _isLoading = true;
        try
        {
            var courseTask    = CourseCatalogAppService.GetCourseDetailStudentAsync(CourseId);
            var progressTask  = LearningProgressAppService.GetMyProgressAsync(CourseId);
            var resumeTask    = LearningProgressAppService.GetResumePositionAsync(CourseId);
            var cpTask        = LearningProgressAppService.GetMyCourseProgressAsync(CourseId);

            await Task.WhenAll(courseTask, progressTask, resumeTask, cpTask);

            _courseDetail      = courseTask.Result;
            _lessonProgresses  = progressTask.Result;
            _resumeResult      = resumeTask.Result;
            _courseProgress    = cpTask.Result;

            // Load assignments (non-fatal if the endpoint is unavailable)
            try
            {
                var assignments = await AssignmentAppService.GetListByCourseAsync(CourseId);
                _assignmentsByLesson = assignments
                    .GroupBy(a => a.LessonId)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }
            catch
            {
                _assignmentsByLesson = new();
            }

            BuildDerivedState();
            await SelectLessonFromRouteParamAsync();
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

    private void BuildDerivedState()
    {
        if (_courseDetail is null) return;

        _flatLessons = _courseDetail.Chapters
            .OrderBy(c => c.OrderNo)
            .SelectMany(c => c.Lessons.OrderBy(l => l.SortOrder))
            .ToList();

        _completedLessonIds = _lessonProgresses
            .Where(p => p.Status == LessonProgressStatus.Completed)
            .Select(p => p.LessonId)
            .ToHashSet();
    }

    private async Task SelectLessonFromRouteParamAsync()
    {
        if (_courseDetail is null) return;

        LessonInChapterDto? target = null;

        if (LessonId.HasValue)
            target = _flatLessons.FirstOrDefault(l => l.Id == LessonId.Value);

        if (target is null && _resumeResult?.LessonId.HasValue == true)
            target = _flatLessons.FirstOrDefault(l => l.Id == _resumeResult.LessonId!.Value);

        if (target is null)
            target = _flatLessons.FirstOrDefault();

        if (target is not null)
            await SelectLessonAsync(target, updateUrl: false);
    }

    // ── Lesson & resource selection ───────────────────────────────────────────

    public async Task SelectLessonAsync(LessonInChapterDto lesson, bool updateUrl = true)
    {
        _selectedLesson  = lesson;
        _selectedMaterial   = null;
        _selectedAssignment = null;

        _selectedChapter = _courseDetail?.Chapters
            .FirstOrDefault(c => c.Lessons.Any(l => l.Id == lesson.Id));

        if (_selectedChapter is not null)
            _expandedChapters.Add(_selectedChapter.Id);

        // Auto-expand the selected lesson in the sidebar
        _expandedLessons.Add(lesson.Id);

        var idx = _flatLessons.IndexOf(lesson);
        _prevLesson = idx > 0 ? _flatLessons[idx - 1] : null;
        _nextLesson = idx >= 0 && idx < _flatLessons.Count - 1 ? _flatLessons[idx + 1] : null;

        if (updateUrl)
            NavigationManager.NavigateTo($"/student/learn/{CourseId}/{lesson.Id}", replace: true);

        await Task.CompletedTask;
    }

    private void ToggleChapter(Guid chapterId)
    {
        if (_expandedChapters.Contains(chapterId))
            _expandedChapters.Remove(chapterId);
        else
            _expandedChapters.Add(chapterId);
    }

    private void ToggleLesson(Guid lessonId)
    {
        if (_expandedLessons.Contains(lessonId))
            _expandedLessons.Remove(lessonId);
        else
            _expandedLessons.Add(lessonId);
    }

    private void SelectResourceAsync(MaterialInLessonDto material)
    {
        _selectedMaterial   = material;
        _selectedAssignment = null;
    }

    private void SelectAssignmentAsync(AssignmentListItemDto assignment)
    {
        _selectedAssignment = assignment;
        _selectedMaterial   = null;
    }

    private void BackToLessonOverview()
    {
        _selectedMaterial   = null;
        _selectedAssignment = null;
    }

    private IReadOnlyList<AssignmentListItemDto> GetLessonAssignments(Guid lessonId) =>
        _assignmentsByLesson.TryGetValue(lessonId, out var list) ? list : Array.Empty<AssignmentListItemDto>();

    // ── Learning actions ──────────────────────────────────────────────────────

    public async Task ResumeLessonAsync()
    {
        if (_selectedLesson is null || _isActing) return;

        _isActing = true;
        try
        {
            await LearningProgressAppService.StartLessonAsync(new StartLessonInput
            {
                CourseId           = CourseId,
                LessonId           = _selectedLesson.Id,
                TotalLessonsCount  = _flatLessons.Count
            });

            _resumeResult   = await LearningProgressAppService.GetResumePositionAsync(CourseId);
            _courseProgress = await LearningProgressAppService.GetMyCourseProgressAsync(CourseId);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isActing = false;
        }
    }

    public async Task CompleteLessonAsync()
    {
        if (_selectedLesson is null || _isActing) return;

        _isActing = true;
        try
        {
            await LearningProgressAppService.CompleteLessonAsync(new CompleteLessonInput
            {
                CourseId           = CourseId,
                LessonId           = _selectedLesson.Id,
                TotalLessonsCount  = _flatLessons.Count
            });

            _completedLessonIds.Add(_selectedLesson.Id);

            _lessonProgresses = await LearningProgressAppService.GetMyProgressAsync(CourseId);
            _courseProgress   = await LearningProgressAppService.GetMyCourseProgressAsync(CourseId);
            _resumeResult     = await LearningProgressAppService.GetResumePositionAsync(CourseId);

            BuildDerivedState();

            if (_nextLesson is not null)
                await SelectLessonAsync(_nextLesson);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
        finally
        {
            _isActing = false;
        }
    }

    private async Task GoToPrevLessonAsync()
    {
        if (_prevLesson is not null)
            await SelectLessonAsync(_prevLesson);
    }

    private async Task GoToNextLessonAsync()
    {
        if (_nextLesson is not null)
            await SelectLessonAsync(_nextLesson);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string GetMaterialIcon(MaterialType type) => type switch
    {
        MaterialType.VideoLink => "fa-play-circle",
        MaterialType.Text      => "fa-file-text-o",
        MaterialType.File      => "fa-file-o",
        _                      => "fa-cube"
    };

    private static string GetMaterialIconClass(MaterialType type) => type switch
    {
        MaterialType.VideoLink => "slp-material__icon-wrap--video",
        MaterialType.Text      => "slp-material__icon-wrap--text",
        MaterialType.File      => "slp-material__icon-wrap--file",
        _                      => "slp-material__icon-wrap--default"
    };

    private static string GetMaterialTypeLabel(MaterialType type) => type switch
    {
        MaterialType.VideoLink => "Video",
        MaterialType.Text      => "Reading",
        MaterialType.File      => "File",
        _                      => "Material"
    };

    private static string GetResourceIconCss(MaterialType type) => type switch
    {
        MaterialType.VideoLink => "text-primary",
        MaterialType.Text      => "text-danger",
        MaterialType.File      => "text-success",
        _                      => "text-secondary"
    };
}
