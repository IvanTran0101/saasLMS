using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using saasLMS.ReportingService.ReadModels;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;

namespace saasLMS.ReportingService.Reports;

public class ReportingAppService : ApplicationService, IReportingAppService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly IRepository<StudentCourseProgressView, Guid> _studentCourseRepo;
    private readonly IRepository<ClassProgressView, Guid> _classProgressRepo;
    private readonly IRepository<CourseOutcomeReportView, Guid> _courseOutcomeRepo;
    private readonly IRepository<TenantSummaryReportView, Guid> _tenantSummaryRepo;

    private readonly IDistributedCache<StudentCourseProgressViewDto> _studentCache;
    private readonly IDistributedCache<ClassProgressViewDto> _classCache;
    private readonly IDistributedCache<CourseOutcomeReportViewDto> _courseOutcomeCache;
    private readonly IDistributedCache<TenantSummaryReportViewDto> _tenantSummaryCache;

    public ReportingAppService(
        IRepository<StudentCourseProgressView, Guid> studentCourseRepo,
        IRepository<ClassProgressView, Guid> classProgressRepo,
        IRepository<CourseOutcomeReportView, Guid> courseOutcomeRepo,
        IRepository<TenantSummaryReportView, Guid> tenantSummaryRepo,
        IDistributedCache<StudentCourseProgressViewDto> studentCache,
        IDistributedCache<ClassProgressViewDto> classCache,
        IDistributedCache<CourseOutcomeReportViewDto> courseOutcomeCache,
        IDistributedCache<TenantSummaryReportViewDto> tenantSummaryCache)
    {
        _studentCourseRepo = studentCourseRepo;
        _classProgressRepo = classProgressRepo;
        _courseOutcomeRepo = courseOutcomeRepo;
        _tenantSummaryRepo = tenantSummaryRepo;
        _studentCache = studentCache;
        _classCache = classCache;
        _courseOutcomeCache = courseOutcomeCache;
        _tenantSummaryCache = tenantSummaryCache;
    }

    public async Task<StudentCourseProgressViewDto?> GetStudentCourseProgressAsync(Guid courseId)
    {
        var tenantId = GetRequiredTenantId();
        var studentId = GetRequiredUserId();
        var cacheKey = BuildStudentKey(tenantId, courseId, studentId);
        var cached = await _studentCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _studentCourseRepo.FirstOrDefaultAsync(
            x => x.TenantId == tenantId
              && x.CourseId == courseId
              && x.StudentId == studentId);
        if (entity == null)
        {
            return null;
        }

        var dto = MapStudentCourseProgress(entity);
        await _studentCache.SetAsync(cacheKey, dto, BuildCacheOptions());
        return dto;
    }

    public async Task<ClassProgressViewDto?> GetClassProgressAsync(Guid courseId)
    {
        var tenantId = GetRequiredTenantId();
        var cacheKey = BuildClassKey(tenantId, courseId);
        var cached = await _classCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _classProgressRepo.FirstOrDefaultAsync(
            x => x.TenantId == tenantId
              && x.CourseId == courseId);
        if (entity == null)
        {
            return null;
        }

        var dto = MapClassProgress(entity);
        await _classCache.SetAsync(cacheKey, dto, BuildCacheOptions());
        return dto;
    }

    public async Task<CourseOutcomeReportViewDto?> GetCourseOutcomeReportAsync(Guid courseId)
    {
        var tenantId = GetRequiredTenantId();
        var cacheKey = BuildCourseOutcomeKey(tenantId, courseId);
        var cached = await _courseOutcomeCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _courseOutcomeRepo.FirstOrDefaultAsync(
            x => x.TenantId == tenantId
              && x.CourseId == courseId);
        if (entity == null)
        {
            return null;
        }

        var dto = MapCourseOutcome(entity);
        await _courseOutcomeCache.SetAsync(cacheKey, dto, BuildCacheOptions());
        return dto;
    }

    public async Task<TenantSummaryReportViewDto?> GetTenantSummaryAsync()
    {
        var tenantId = GetRequiredTenantId();
        var cacheKey = BuildTenantKey(tenantId);
        var cached = await _tenantSummaryCache.GetAsync(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var entity = await _tenantSummaryRepo.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        if (entity == null)
        {
            return null;
        }

        var dto = MapTenantSummary(entity);
        await _tenantSummaryCache.SetAsync(cacheKey, dto, BuildCacheOptions());
        return dto;
    }

    private static DistributedCacheEntryOptions BuildCacheOptions()
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = CacheDuration
        };
    }

    private static string BuildStudentKey(Guid tenantId, Guid courseId, Guid studentId)
        => $"report:student:{tenantId}:{courseId}:{studentId}";

    private static string BuildClassKey(Guid tenantId, Guid courseId)
        => $"report:class:{tenantId}:{courseId}";

    private static string BuildCourseOutcomeKey(Guid tenantId, Guid courseId)
        => $"report:course:{tenantId}:{courseId}";

    private static string BuildTenantKey(Guid tenantId)
        => $"report:tenant:{tenantId}";

    private Guid GetRequiredTenantId()
    {
        if (!CurrentTenant.Id.HasValue)
        {
            throw new AbpException("Tenant context is required for reporting.");
        }
        return CurrentTenant.Id.Value;
    }

    private Guid GetRequiredUserId()
    {
        if (!CurrentUser.Id.HasValue)
        {
            throw new AbpException("User context is required for reporting.");
        }
        return CurrentUser.Id.Value;
    }

    private static StudentCourseProgressViewDto MapStudentCourseProgress(StudentCourseProgressView e)
    {
        return new StudentCourseProgressViewDto
        {
            TenantId = e.TenantId,
            CourseId = e.CourseId,
            StudentId = e.StudentId,
            CompletedLessonsCount = e.CompletedLessonsCount,
            TotalLessonsCount = e.TotalLessonsCount,
            LessonCompletionPercent = e.LessonCompletionPercent,
            AssignmentGradedCount = e.AssignmentGradedCount,
            TotalAssignmentsCount = e.TotalAssignmentsCount,
            AssignmentScoreSum = e.AssignmentScoreSum,
            AssignmentCompletionPercent = e.AssignmentCompletionPercent,
            AvgAssignmentScore = e.AvgAssignmentScore,
            QuizCompletedCount = e.QuizCompletedCount,
            TotalQuizzesCount = e.TotalQuizzesCount,
            QuizScoreSum = e.QuizScoreSum,
            QuizCompletionPercent = e.QuizCompletionPercent,
            AvgQuizScore = e.AvgQuizScore,
            OverallProgress = e.OverallProgress,
            LastAccessedLessonId = e.LastAccessedLessonId,
            LastAccessedAt = e.LastAccessedAt,
            LastUpdatedAt = e.LastUpdatedAt
        };
    }

    private static ClassProgressViewDto MapClassProgress(ClassProgressView e)
    {
        return new ClassProgressViewDto
        {
            TenantId = e.TenantId,
            CourseId = e.CourseId,
            ActiveEnrollmentCount = e.ActiveEnrollmentCount,
            TotalStudents = e.TotalStudents,
            CompletedCount = e.CompletedCount,
            InProgressCount = e.InProgressCount,
            Bucket_0_25 = e.Bucket_0_25,
            Bucket_26_50 = e.Bucket_26_50,
            Bucket_51_75 = e.Bucket_51_75,
            Bucket_76_99 = e.Bucket_76_99,
            Bucket_100 = e.Bucket_100,
            LastRecalculatedAt = e.LastRecalculatedAt,
            LastUpdatedAt = e.LastUpdatedAt
        };
    }

    private static CourseOutcomeReportViewDto MapCourseOutcome(CourseOutcomeReportView e)
    {
        return new CourseOutcomeReportViewDto
        {
            TenantId = e.TenantId,
            CourseId = e.CourseId,
            AssignmentGradedCount = e.AssignmentGradedCount,
            AssignmentScoreSum = e.AssignmentScoreSum,
            AvgAssignmentScore = e.AvgAssignmentScore,
            QuizCompletedCount = e.QuizCompletedCount,
            QuizScoreSum = e.QuizScoreSum,
            AvgQuizScore = e.AvgQuizScore,
            FinalScoreCount = e.FinalScoreCount,
            FinalScoreSum = e.FinalScoreSum,
            FinalScoreAvg = e.FinalScoreAvg,
            CompletionRate = e.CompletionRate,
            PassRate = e.PassRate,
            ScoreDistributionJson = e.ScoreDistributionJson,
            LastUpdatedAt = e.LastUpdatedAt
        };
    }

    private static TenantSummaryReportViewDto MapTenantSummary(TenantSummaryReportView e)
    {
        return new TenantSummaryReportViewDto
        {
            TenantId = e.TenantId,
            TotalStudents = e.TotalStudents,
            ActiveStudents = e.ActiveStudents,
            TotalInstructors = e.TotalInstructors,
            TotalCourses = e.TotalCourses,
            ActiveCourses = e.ActiveCourses,
            LastUpdatedAt = e.LastUpdatedAt
        };
    }
}
