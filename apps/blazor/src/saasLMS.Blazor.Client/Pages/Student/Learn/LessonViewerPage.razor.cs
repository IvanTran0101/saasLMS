using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
    [Inject] private NavigationManager            NavigationManager           { get; set; } = default!;

    // ── Raw data ──────────────────────────────────────────────────────────────

    private CourseDetailDto?          _courseDetail;
    private List<LessonProgressDto>   _lessonProgresses   = new();
    private ResumeResultDto?          _resumeResult;
    private CourseProgressDto?        _courseProgress;

    // ── Derived state ─────────────────────────────────────────────────────────

    private HashSet<Guid>             _completedLessonIds = new();
    private HashSet<Guid>             _expandedChapters   = new();
    private List<LessonInChapterDto>  _flatLessons        = new(); // ordered flat list

    private ChapterDto?              _selectedChapter;
    private LessonInChapterDto?      _selectedLesson;
    private LessonInChapterDto?      _prevLesson;
    private LessonInChapterDto?      _nextLesson;

    // ── Computed lesson status flags ──────────────────────────────────────────

    /// True when there is no completed lesson yet (course not started at all).
    private bool _hasCourseStarted => _completedLessonIds.Count > 0 || _resumeResult?.LessonStatus == LessonProgressStatus.InProgress;

    /// True when the currently selected lesson is the "resume" lesson (InProgress or the next to start).
    private bool _isResumeLesson => _selectedLesson is not null && _resumeResult?.LessonId == _selectedLesson.Id;

    /// True when the currently selected lesson is fully completed.
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
        // Called when navigating between lessons (URL changes but component stays mounted)
        if (!_isLoading)
            await SelectLessonFromRouteParamAsync();
    }

    // ── Data loading ──────────────────────────────────────────────────────────

    private async Task LoadAllAsync()
    {
        _isLoading = true;
        try
        {
            // Load everything in parallel
            var courseTask    = CourseCatalogAppService.GetCourseDetailStudentAsync(CourseId);
            var progressTask  = LearningProgressAppService.GetMyProgressAsync(CourseId);
            var resumeTask    = LearningProgressAppService.GetResumePositionAsync(CourseId);
            var cpTask        = LearningProgressAppService.GetMyCourseProgressAsync(CourseId);

            await Task.WhenAll(courseTask, progressTask, resumeTask, cpTask);

            _courseDetail      = courseTask.Result;
            _lessonProgresses  = progressTask.Result;
            _resumeResult      = resumeTask.Result;
            _courseProgress    = cpTask.Result;

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

        // Build flat, ordered lesson list
        _flatLessons = _courseDetail.Chapters
            .OrderBy(c => c.OrderNo)
            .SelectMany(c => c.Lessons.OrderBy(l => l.SortOrder))
            .ToList();

        // Build set of completed lesson IDs
        _completedLessonIds = _lessonProgresses
            .Where(p => p.Status == LessonProgressStatus.Completed)
            .Select(p => p.LessonId)
            .ToHashSet();
    }

    /// Decides which lesson to show based on: URL param → resume → first lesson.
    private async Task SelectLessonFromRouteParamAsync()
    {
        if (_courseDetail is null) return;

        LessonInChapterDto? target = null;

        if (LessonId.HasValue)
        {
            // Explicitly requested via URL
            target = _flatLessons.FirstOrDefault(l => l.Id == LessonId.Value);
        }

        if (target is null && _resumeResult?.LessonId.HasValue == true)
        {
            // Fall back to resume position
            target = _flatLessons.FirstOrDefault(l => l.Id == _resumeResult.LessonId!.Value);
        }

        if (target is null)
        {
            // Fall back to first lesson
            target = _flatLessons.FirstOrDefault();
        }

        if (target is not null)
            await SelectLessonAsync(target, updateUrl: false);
    }

    // ── Lesson selection ──────────────────────────────────────────────────────

    public async Task SelectLessonAsync(LessonInChapterDto lesson, bool updateUrl = true)
    {
        _selectedLesson  = lesson;
        _selectedChapter = _courseDetail?.Chapters
            .FirstOrDefault(c => c.Lessons.Any(l => l.Id == lesson.Id));

        // Expand the containing chapter
        if (_selectedChapter is not null)
            _expandedChapters.Add(_selectedChapter.Id);

        // Compute prev/next
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

    // ── Learning actions ──────────────────────────────────────────────────────

    /// Called by the "Start Learning" / "Resume Learning" button.
    public async Task ResumeLessonAsync()
    {
        if (_selectedLesson is null || _isActing) return;

        _isActing = true;
        try
        {
            var totalCount = _flatLessons.Count;
            await LearningProgressAppService.StartLessonAsync(new StartLessonInput
            {
                CourseId           = CourseId,
                LessonId           = _selectedLesson.Id,
                TotalLessonsCount  = totalCount
            });

            // Refresh progress so the resume position updates
            _resumeResult     = await LearningProgressAppService.GetResumePositionAsync(CourseId);
            _courseProgress   = await LearningProgressAppService.GetMyCourseProgressAsync(CourseId);
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

    /// Called by the "Mark as Complete" button.
    public async Task CompleteLessonAsync()
    {
        if (_selectedLesson is null || _isActing) return;

        _isActing = true;
        try
        {
            var totalCount = _flatLessons.Count;
            await LearningProgressAppService.CompleteLessonAsync(new CompleteLessonInput
            {
                CourseId           = CourseId,
                LessonId           = _selectedLesson.Id,
                TotalLessonsCount  = totalCount
            });

            // Update local state immediately for responsiveness
            _completedLessonIds.Add(_selectedLesson.Id);

            // Refresh all progress
            _lessonProgresses = await LearningProgressAppService.GetMyProgressAsync(CourseId);
            _courseProgress   = await LearningProgressAppService.GetMyCourseProgressAsync(CourseId);
            _resumeResult     = await LearningProgressAppService.GetResumePositionAsync(CourseId);

            BuildDerivedState();

            // Auto-advance to next lesson if available
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
}
