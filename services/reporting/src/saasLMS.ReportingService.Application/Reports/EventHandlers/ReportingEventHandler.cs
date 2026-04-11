using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using saasLMS.AssessmentService.Assignments.Etos;
using saasLMS.AssessmentService.QuizAttempts.Etos;
using saasLMS.AssessmentService.Quizzes.Etos;
using saasLMS.AssessmentService.Submissions.Etos;
using saasLMS.CourseCatalogService.Etos.Lessons;
using saasLMS.EnrollmentService.Etos.Enrollments;
using saasLMS.LearningProgressService.Etos.CourseProgresses;
using saasLMS.ReportingService.ReadModels;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace saasLMS.ReportingService.Reports.EventHandlers;

[UnitOfWork]
public class ReportingEventHandler :
    IDistributedEventHandler<StudentEnrolledEto>,
    IDistributedEventHandler<StudentUnenrolledEto>,
    IDistributedEventHandler<LessonCreatedEto>,
    IDistributedEventHandler<LessonDeletedEto>,
    IDistributedEventHandler<CourseProgressUpdatedEto>,
    IDistributedEventHandler<SubmissionGradedEto>,
    IDistributedEventHandler<QuizAttemptCompletedEto>,
    IDistributedEventHandler<AssignmentCreatedEto>,
    IDistributedEventHandler<QuizCreatedEto>,
    ITransientDependency
{
    private readonly IRepository<StudentCourseProgressView, Guid> _studentCourseRepo;
    private readonly IRepository<ClassProgressView, Guid> _classProgressRepo;
    private readonly IRepository<CourseOutcomeReportView, Guid> _courseOutcomeRepo;
    private readonly IRepository<TenantSummaryReportView, Guid> _tenantSummaryRepo;
    private readonly IDistributedCache<StudentCourseProgressViewDto> _studentCache;
    private readonly IDistributedCache<ClassProgressViewDto> _classCache;
    private readonly IDistributedCache<CourseOutcomeReportViewDto> _courseOutcomeCache;
    private readonly IDistributedCache<TenantSummaryReportViewDto> _tenantSummaryCache;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public ReportingEventHandler(
        IRepository<StudentCourseProgressView, Guid> studentCourseRepo,
        IRepository<ClassProgressView, Guid> classProgressRepo,
        IRepository<CourseOutcomeReportView, Guid> courseOutcomeRepo,
        IRepository<TenantSummaryReportView, Guid> tenantSummaryRepo,
        IDistributedCache<StudentCourseProgressViewDto> studentCache,
        IDistributedCache<ClassProgressViewDto> classCache,
        IDistributedCache<CourseOutcomeReportViewDto> courseOutcomeCache,
        IDistributedCache<TenantSummaryReportViewDto> tenantSummaryCache,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _studentCourseRepo = studentCourseRepo;
        _classProgressRepo = classProgressRepo;
        _courseOutcomeRepo = courseOutcomeRepo;
        _tenantSummaryRepo = tenantSummaryRepo;
        _studentCache = studentCache;
        _classCache = classCache;
        _courseOutcomeCache = courseOutcomeCache;
        _tenantSummaryCache = tenantSummaryCache;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task HandleEventAsync(StudentEnrolledEto eventData)
    {
        await ExecuteInUowAsync(async () =>
        {
            var studentView = await GetOrCreateStudentViewAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            if (!studentView.IsActiveEnrollment)
            {
                studentView.IsActiveEnrollment = true;
                studentView.LastUpdatedAt = DateTime.UtcNow;
                await _studentCourseRepo.UpdateAsync(studentView, autoSave: true);
            }

            var classView = await GetOrCreateClassViewAsync(eventData.TenantId, eventData.CourseId);
            classView.ActiveEnrollmentCount += 1;
            if (studentView.CompletedLessonsCount == 0 && studentView.TotalLessonsCount == 0)
            {
                classView.TotalStudents += 1;
            }
            classView.LastUpdatedAt = DateTime.UtcNow;
            await _classProgressRepo.UpdateAsync(classView, autoSave: true);

            await RecalculateCourseOutcomeMetricsAsync(eventData.TenantId, eventData.CourseId);

            await InvalidateStudentCacheAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            await InvalidateClassCacheAsync(eventData.TenantId, eventData.CourseId);
            await InvalidateTenantCacheAsync(eventData.TenantId);
            await InvalidateCourseOutcomeCacheAsync(eventData.TenantId, eventData.CourseId);
        });
    }

    public async Task HandleEventAsync(StudentUnenrolledEto eventData)
    {
        await ExecuteInUowAsync(async () =>
        {
            var studentView = await _studentCourseRepo.FirstOrDefaultAsync(
                x => x.TenantId == eventData.TenantId
                  && x.CourseId == eventData.CourseId
                  && x.StudentId == eventData.StudentId);
            if (studentView == null)
            {
                return;
            }

            if (studentView.IsActiveEnrollment)
            {
                studentView.IsActiveEnrollment = false;
                studentView.LastUpdatedAt = DateTime.UtcNow;
                await _studentCourseRepo.UpdateAsync(studentView, autoSave: true);
            }

            var classView = await GetOrCreateClassViewAsync(eventData.TenantId, eventData.CourseId);
            classView.ActiveEnrollmentCount = Math.Max(0, classView.ActiveEnrollmentCount - 1);
            classView.LastUpdatedAt = DateTime.UtcNow;
            await _classProgressRepo.UpdateAsync(classView, autoSave: true);

            await RecalculateCourseOutcomeMetricsAsync(eventData.TenantId, eventData.CourseId);

            await InvalidateStudentCacheAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            await InvalidateClassCacheAsync(eventData.TenantId, eventData.CourseId);
            await InvalidateTenantCacheAsync(eventData.TenantId);
            await InvalidateCourseOutcomeCacheAsync(eventData.TenantId, eventData.CourseId);
        });
    }

    public Task HandleEventAsync(LessonCreatedEto eventData)
        => ExecuteInUowAsync(() => UpdateTotalLessonsForCourseAsync(eventData.TenantId, eventData.CourseId, delta: 1));

    public Task HandleEventAsync(LessonDeletedEto eventData)
        => ExecuteInUowAsync(() => UpdateTotalLessonsForCourseAsync(eventData.TenantId, eventData.CourseId, delta: -1));

    public async Task HandleEventAsync(CourseProgressUpdatedEto eventData)
    {
        await ExecuteInUowAsync(async () =>
        {
            var studentView = await GetOrCreateStudentViewAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            var oldBucket = GetBucket(studentView.OverallProgress);
            var oldStatus = studentView.Status;

            studentView.Status = (int)eventData.Status;
            studentView.CompletedLessonsCount = eventData.CompletedLessonsCount;
            studentView.TotalLessonsCount = eventData.TotalLessonsCount;
            studentView.LessonCompletionPercent = eventData.TotalLessonsCount == 0
                ? 0
                : Math.Round((decimal)eventData.CompletedLessonsCount / eventData.TotalLessonsCount * 100, 2);
            studentView.LastAccessedLessonId = eventData.LastAccessedLessonId;
            studentView.LastAccessedAt = eventData.LastAccessedAt;
            studentView.OverallProgress = ComputeOverallProgress(studentView);
            studentView.LastUpdatedAt = DateTime.UtcNow;
            await _studentCourseRepo.UpdateAsync(studentView, autoSave: true);

            var classView = await GetOrCreateClassViewAsync(eventData.TenantId, eventData.CourseId);
            var newBucket = GetBucket(studentView.OverallProgress);
            if (oldBucket != newBucket)
            {
                ApplyBucketDelta(classView, oldBucket, -1);
                ApplyBucketDelta(classView, newBucket, 1);
            }

            if (oldStatus != studentView.Status)
            {
                UpdateStatusCounts(classView, oldStatus, studentView.Status);
            }

            classView.LastUpdatedAt = DateTime.UtcNow;
            await _classProgressRepo.UpdateAsync(classView, autoSave: true);

            await RecalculateCourseOutcomeMetricsAsync(eventData.TenantId, eventData.CourseId);

            await InvalidateStudentCacheAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            await InvalidateClassCacheAsync(eventData.TenantId, eventData.CourseId);
            await InvalidateCourseOutcomeCacheAsync(eventData.TenantId, eventData.CourseId);
        });
    }

    public async Task HandleEventAsync(AssignmentCreatedEto eventData)
    {
        await ExecuteInUowAsync(() => UpdateTotalAssignmentsForCourseAsync(eventData.TenantId, eventData.CourseId, delta: 1));
    }

    public async Task HandleEventAsync(QuizCreatedEto eventData)
    {
        await ExecuteInUowAsync(() => UpdateTotalQuizzesForCourseAsync(eventData.TenantId, eventData.CourseId, delta: 1));
    }

    public async Task HandleEventAsync(SubmissionGradedEto eventData)
    {
        await ExecuteInUowAsync(async () =>
        {
            var studentView = await GetOrCreateStudentViewAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            studentView.AssignmentGradedCount += 1;
            studentView.AssignmentScoreSum += eventData.Score;
            studentView.AvgAssignmentScore = studentView.AssignmentGradedCount == 0
                ? 0
                : Math.Round(studentView.AssignmentScoreSum / studentView.AssignmentGradedCount, 2);
            studentView.AssignmentCompletionPercent = studentView.TotalAssignmentsCount == 0
                ? 0
                : Math.Round((decimal)studentView.AssignmentGradedCount / studentView.TotalAssignmentsCount * 100, 2);
            studentView.OverallProgress = ComputeOverallProgress(studentView);
            studentView.LastUpdatedAt = DateTime.UtcNow;
            await _studentCourseRepo.UpdateAsync(studentView, autoSave: true);

            var courseOutcome = await GetOrCreateCourseOutcomeAsync(eventData.TenantId, eventData.CourseId);
            courseOutcome.AssignmentGradedCount += 1;
            courseOutcome.AssignmentScoreSum += eventData.Score;
            courseOutcome.AvgAssignmentScore = courseOutcome.AssignmentGradedCount == 0
                ? 0
                : Math.Round(courseOutcome.AssignmentScoreSum / courseOutcome.AssignmentGradedCount, 2);
            courseOutcome.LastUpdatedAt = DateTime.UtcNow;
            await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);

            await RecalculateCourseOutcomeMetricsAsync(eventData.TenantId, eventData.CourseId);

            await InvalidateStudentCacheAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            await InvalidateCourseOutcomeCacheAsync(eventData.TenantId, eventData.CourseId);
        });
    }

    public async Task HandleEventAsync(QuizAttemptCompletedEto eventData)
    {
        await ExecuteInUowAsync(async () =>
        {
            var studentView = await GetOrCreateStudentViewAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            studentView.QuizCompletedCount += 1;
            studentView.QuizScoreSum += eventData.Score;
            studentView.AvgQuizScore = studentView.QuizCompletedCount == 0
                ? 0
                : Math.Round(studentView.QuizScoreSum / studentView.QuizCompletedCount, 2);
            studentView.QuizCompletionPercent = studentView.TotalQuizzesCount == 0
                ? 0
                : Math.Round((decimal)studentView.QuizCompletedCount / studentView.TotalQuizzesCount * 100, 2);
            studentView.OverallProgress = ComputeOverallProgress(studentView);
            studentView.LastUpdatedAt = DateTime.UtcNow;
            await _studentCourseRepo.UpdateAsync(studentView, autoSave: true);

            var courseOutcome = await GetOrCreateCourseOutcomeAsync(eventData.TenantId, eventData.CourseId);
            courseOutcome.QuizCompletedCount += 1;
            courseOutcome.QuizScoreSum += eventData.Score;
            courseOutcome.AvgQuizScore = courseOutcome.QuizCompletedCount == 0
                ? 0
                : Math.Round(courseOutcome.QuizScoreSum / courseOutcome.QuizCompletedCount, 2);
            courseOutcome.LastUpdatedAt = DateTime.UtcNow;
            await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);

            await RecalculateCourseOutcomeMetricsAsync(eventData.TenantId, eventData.CourseId);

            await InvalidateStudentCacheAsync(eventData.TenantId, eventData.CourseId, eventData.StudentId);
            await InvalidateCourseOutcomeCacheAsync(eventData.TenantId, eventData.CourseId);
        });
    }

    private async Task<StudentCourseProgressView> GetOrCreateStudentViewAsync(Guid tenantId, Guid courseId, Guid studentId)
    {
        var entity = await _studentCourseRepo.FirstOrDefaultAsync(
            x => x.TenantId == tenantId
              && x.CourseId == courseId
              && x.StudentId == studentId);
        if (entity != null)
        {
            return entity;
        }

        entity = new StudentCourseProgressView(Guid.NewGuid(), tenantId, courseId, studentId);
        var courseOutcome = await GetOrCreateCourseOutcomeAsync(tenantId, courseId);
        entity.TotalLessonsCount = courseOutcome.TotalLessonsCount;
        entity.TotalAssignmentsCount = courseOutcome.TotalAssignmentsCount;
        entity.TotalQuizzesCount = courseOutcome.TotalQuizzesCount;
        await _studentCourseRepo.InsertAsync(entity, autoSave: true);
        return entity;
    }

    private async Task<ClassProgressView> GetOrCreateClassViewAsync(Guid tenantId, Guid courseId)
    {
        var entity = await _classProgressRepo.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.CourseId == courseId);
        if (entity != null)
        {
            return entity;
        }

        entity = new ClassProgressView(Guid.NewGuid(), tenantId, courseId);
        await _classProgressRepo.InsertAsync(entity, autoSave: true);
        return entity;
    }

    private async Task<CourseOutcomeReportView> GetOrCreateCourseOutcomeAsync(Guid tenantId, Guid courseId)
    {
        var entity = await _courseOutcomeRepo.FirstOrDefaultAsync(x => x.TenantId == tenantId && x.CourseId == courseId);
        if (entity != null)
        {
            return entity;
        }

        entity = new CourseOutcomeReportView(Guid.NewGuid(), tenantId, courseId);
        await _courseOutcomeRepo.InsertAsync(entity, autoSave: true);
        return entity;
    }

    private async Task ExecuteInUowAsync(Func<Task> action)
    {
        using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);
        await action();
        await uow.CompleteAsync();
    }

    private async Task UpdateTotalLessonsForCourseAsync(Guid tenantId, Guid courseId, int delta)
    {
        var students = await _studentCourseRepo.GetListAsync(x => x.TenantId == tenantId && x.CourseId == courseId);
        foreach (var student in students)
        {
            student.TotalLessonsCount = Math.Max(0, student.TotalLessonsCount + delta);
            student.LessonCompletionPercent = student.TotalLessonsCount == 0
                ? 0
                : Math.Round((decimal)student.CompletedLessonsCount / student.TotalLessonsCount * 100, 2);
            student.OverallProgress = ComputeOverallProgress(student);
            student.LastUpdatedAt = DateTime.UtcNow;
            await _studentCourseRepo.UpdateAsync(student, autoSave: true);
            await InvalidateStudentCacheAsync(tenantId, courseId, student.StudentId);
        }
        var courseOutcome = await GetOrCreateCourseOutcomeAsync(tenantId, courseId);
        courseOutcome.TotalLessonsCount = Math.Max(0, courseOutcome.TotalLessonsCount + delta);
        courseOutcome.LastUpdatedAt = DateTime.UtcNow;
        await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);
        await RecalculateCourseOutcomeMetricsAsync(tenantId, courseId);
        await InvalidateClassCacheAsync(tenantId, courseId);
    }

    private async Task UpdateTotalAssignmentsForCourseAsync(Guid tenantId, Guid courseId, int delta)
    {
        var students = await _studentCourseRepo.GetListAsync(x => x.TenantId == tenantId && x.CourseId == courseId);
        foreach (var student in students)
        {
            student.TotalAssignmentsCount = Math.Max(0, student.TotalAssignmentsCount + delta);
            student.AssignmentCompletionPercent = student.TotalAssignmentsCount == 0
                ? 0
                : Math.Round((decimal)student.AssignmentGradedCount / student.TotalAssignmentsCount * 100, 2);
            student.OverallProgress = ComputeOverallProgress(student);
            student.LastUpdatedAt = DateTime.UtcNow;
            await _studentCourseRepo.UpdateAsync(student, autoSave: true);
            await InvalidateStudentCacheAsync(tenantId, courseId, student.StudentId);
        }
        var courseOutcome = await GetOrCreateCourseOutcomeAsync(tenantId, courseId);
        courseOutcome.TotalAssignmentsCount = Math.Max(0, courseOutcome.TotalAssignmentsCount + delta);
        courseOutcome.LastUpdatedAt = DateTime.UtcNow;
        await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);
        await RecalculateCourseOutcomeMetricsAsync(tenantId, courseId);
        await InvalidateCourseOutcomeCacheAsync(tenantId, courseId);
    }

    private async Task UpdateTotalQuizzesForCourseAsync(Guid tenantId, Guid courseId, int delta)
    {
        var students = await _studentCourseRepo.GetListAsync(x => x.TenantId == tenantId && x.CourseId == courseId);
        foreach (var student in students)
        {
            student.TotalQuizzesCount = Math.Max(0, student.TotalQuizzesCount + delta);
            student.QuizCompletionPercent = student.TotalQuizzesCount == 0
                ? 0
                : Math.Round((decimal)student.QuizCompletedCount / student.TotalQuizzesCount * 100, 2);
            student.OverallProgress = ComputeOverallProgress(student);
            student.LastUpdatedAt = DateTime.UtcNow;
            await _studentCourseRepo.UpdateAsync(student, autoSave: true);
            await InvalidateStudentCacheAsync(tenantId, courseId, student.StudentId);
        }
        var courseOutcome = await GetOrCreateCourseOutcomeAsync(tenantId, courseId);
        courseOutcome.TotalQuizzesCount = Math.Max(0, courseOutcome.TotalQuizzesCount + delta);
        courseOutcome.LastUpdatedAt = DateTime.UtcNow;
        await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);
        await RecalculateCourseOutcomeMetricsAsync(tenantId, courseId);
        await InvalidateCourseOutcomeCacheAsync(tenantId, courseId);
    }

    private async Task RecalculateCourseOutcomeMetricsAsync(Guid tenantId, Guid courseId)
    {
        var students = await _studentCourseRepo.GetListAsync(
            x => x.TenantId == tenantId && x.CourseId == courseId && x.IsActiveEnrollment);

        var courseOutcome = await GetOrCreateCourseOutcomeAsync(tenantId, courseId);

        if (students.Count == 0)
        {
            courseOutcome.FinalScoreCount = 0;
            courseOutcome.FinalScoreSum = 0;
            courseOutcome.FinalScoreAvg = 0;
            courseOutcome.CompletionRate = 0;
            courseOutcome.PassRate = 0;
            courseOutcome.ScoreDistributionJson = JsonSerializer.Serialize(new ScoreDistribution());
            courseOutcome.LastUpdatedAt = DateTime.UtcNow;
            await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);
            return;
        }

        var finalScores = students
            .Select(s => Math.Round((s.AvgAssignmentScore + s.AvgQuizScore) / 2, 2))
            .ToList();

        var distribution = new ScoreDistribution();
        foreach (var score in finalScores)
        {
            ApplyScoreBucket(distribution, GetScoreBucket(score), 1);
        }

        courseOutcome.FinalScoreCount = students.Count;
        courseOutcome.FinalScoreSum = finalScores.Sum();
        courseOutcome.FinalScoreAvg = Math.Round(courseOutcome.FinalScoreSum / courseOutcome.FinalScoreCount, 2);
        courseOutcome.CompletionRate = Math.Round(students.Average(s => s.OverallProgress), 2);
        courseOutcome.PassRate = Math.Round((decimal)finalScores.Count(s => s >= 50) / courseOutcome.FinalScoreCount * 100, 2);
        courseOutcome.ScoreDistributionJson = JsonSerializer.Serialize(distribution);
        courseOutcome.LastUpdatedAt = DateTime.UtcNow;
        await _courseOutcomeRepo.UpdateAsync(courseOutcome, autoSave: true);
    }

    private static int GetScoreBucket(decimal score)
    {
        if (score >= 100) return 100;
        if (score >= 76) return 76;
        if (score >= 51) return 51;
        if (score >= 26) return 26;
        return 0;
    }

    private static void ApplyScoreBucket(ScoreDistribution dist, int bucket, int delta)
    {
        switch (bucket)
        {
            case 100:
                dist.Bucket_100 = Math.Max(0, dist.Bucket_100 + delta);
                break;
            case 76:
                dist.Bucket_76_99 = Math.Max(0, dist.Bucket_76_99 + delta);
                break;
            case 51:
                dist.Bucket_51_75 = Math.Max(0, dist.Bucket_51_75 + delta);
                break;
            case 26:
                dist.Bucket_26_50 = Math.Max(0, dist.Bucket_26_50 + delta);
                break;
            default:
                dist.Bucket_0_25 = Math.Max(0, dist.Bucket_0_25 + delta);
                break;
        }
    }

    private sealed class ScoreDistribution
    {
        public int Bucket_0_25 { get; set; }
        public int Bucket_26_50 { get; set; }
        public int Bucket_51_75 { get; set; }
        public int Bucket_76_99 { get; set; }
        public int Bucket_100 { get; set; }
    }

    private static decimal ComputeOverallProgress(StudentCourseProgressView student)
    {
        var parts = new List<decimal>();
        if (student.TotalLessonsCount > 0)
        {
            parts.Add(student.LessonCompletionPercent);
        }
        if (student.TotalAssignmentsCount > 0)
        {
            parts.Add(student.AssignmentCompletionPercent);
        }
        if (student.TotalQuizzesCount > 0)
        {
            parts.Add(student.QuizCompletionPercent);
        }
        if (parts.Count == 0)
        {
            return 0;
        }
        return Math.Round(parts.Average(), 2);
    }

    private static int GetBucket(decimal progress)
    {
        if (progress >= 100) return 100;
        if (progress >= 76) return 76;
        if (progress >= 51) return 51;
        if (progress >= 26) return 26;
        return 0;
    }

    private static void ApplyBucketDelta(ClassProgressView view, int bucket, int delta)
    {
        switch (bucket)
        {
            case 100:
                view.Bucket_100 = Math.Max(0, view.Bucket_100 + delta);
                break;
            case 76:
                view.Bucket_76_99 = Math.Max(0, view.Bucket_76_99 + delta);
                break;
            case 51:
                view.Bucket_51_75 = Math.Max(0, view.Bucket_51_75 + delta);
                break;
            case 26:
                view.Bucket_26_50 = Math.Max(0, view.Bucket_26_50 + delta);
                break;
            default:
                view.Bucket_0_25 = Math.Max(0, view.Bucket_0_25 + delta);
                break;
        }
    }

    private static void UpdateStatusCounts(ClassProgressView view, int oldStatus, int newStatus)
    {
        if (oldStatus == newStatus)
        {
            return;
        }
        if (oldStatus == 2) view.CompletedCount = Math.Max(0, view.CompletedCount - 1);
        if (oldStatus == 1) view.InProgressCount = Math.Max(0, view.InProgressCount - 1);
        if (newStatus == 2) view.CompletedCount += 1;
        if (newStatus == 1) view.InProgressCount += 1;
    }

    private Task InvalidateStudentCacheAsync(Guid tenantId, Guid courseId, Guid studentId)
        => _studentCache.RemoveAsync(ReportingCacheKeys.Student(tenantId, courseId, studentId));

    private Task InvalidateClassCacheAsync(Guid tenantId, Guid courseId)
        => _classCache.RemoveAsync(ReportingCacheKeys.Class(tenantId, courseId));

    private Task InvalidateCourseOutcomeCacheAsync(Guid tenantId, Guid courseId)
        => _courseOutcomeCache.RemoveAsync(ReportingCacheKeys.CourseOutcome(tenantId, courseId));

    private Task InvalidateTenantCacheAsync(Guid tenantId)
        => _tenantSummaryCache.RemoveAsync(ReportingCacheKeys.Tenant(tenantId));
}
