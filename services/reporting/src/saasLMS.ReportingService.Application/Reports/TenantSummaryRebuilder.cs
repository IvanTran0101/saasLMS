using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using saasLMS.ReportingService.ReadModels;
using saasLMS.ReportingService.Reports.Dtos.Outputs;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace saasLMS.ReportingService.Reports;

public class TenantSummaryRebuilder : ITransientDependency
{
    private readonly IRepository<StudentCourseProgressView, Guid> _studentCourseRepo;
    private readonly IRepository<ClassProgressView, Guid> _classProgressRepo;
    private readonly IRepository<TenantSummaryReportView, Guid> _tenantSummaryRepo;
    private readonly IDistributedCache<TenantSummaryReportViewDto> _tenantSummaryCache;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public TenantSummaryRebuilder(
        IRepository<StudentCourseProgressView, Guid> studentCourseRepo,
        IRepository<ClassProgressView, Guid> classProgressRepo,
        IRepository<TenantSummaryReportView, Guid> tenantSummaryRepo,
        IDistributedCache<TenantSummaryReportViewDto> tenantSummaryCache,
        IUnitOfWorkManager unitOfWorkManager)
    {
        _studentCourseRepo = studentCourseRepo;
        _classProgressRepo = classProgressRepo;
        _tenantSummaryRepo = tenantSummaryRepo;
        _tenantSummaryCache = tenantSummaryCache;
        _unitOfWorkManager = unitOfWorkManager;
    }

    public async Task RebuildAllAsync()
    {
        using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: false);

        var studentQuery = await _studentCourseRepo.GetQueryableAsync();
        var classQuery = await _classProgressRepo.GetQueryableAsync();
        var summaryQuery = await _tenantSummaryRepo.GetQueryableAsync();

        var studentAgg = await studentQuery
            .GroupBy(x => x.TenantId)
            .Select(g => new
            {
                TenantId = g.Key,
                TotalStudents = g.Select(x => x.StudentId).Distinct().Count(),
                ActiveStudents = g.Where(x => x.IsActiveEnrollment).Select(x => x.StudentId).Distinct().Count()
            })
            .ToListAsync();

        var courseAgg = await classQuery
            .GroupBy(x => x.TenantId)
            .Select(g => new
            {
                TenantId = g.Key,
                TotalCourses = g.Select(x => x.CourseId).Distinct().Count(),
                ActiveCourses = g.Where(x => x.ActiveEnrollmentCount > 0).Select(x => x.CourseId).Distinct().Count()
            })
            .ToListAsync();

        var summaries = await summaryQuery.ToListAsync();
        var summaryByTenant = summaries.ToDictionary(x => x.TenantId, x => x);

        var tenantIds = new HashSet<Guid>(studentAgg.Select(x => x.TenantId));
        foreach (var tenantId in courseAgg.Select(x => x.TenantId))
        {
            tenantIds.Add(tenantId);
        }

        foreach (var tenantId in tenantIds)
        {
            if (!summaryByTenant.TryGetValue(tenantId, out var summary))
            {
                summary = new TenantSummaryReportView(Guid.NewGuid(), tenantId);
                await _tenantSummaryRepo.InsertAsync(summary, autoSave: true);
                summaryByTenant[tenantId] = summary;
            }

            var student = studentAgg.FirstOrDefault(x => x.TenantId == tenantId);
            var course = courseAgg.FirstOrDefault(x => x.TenantId == tenantId);

            summary.TotalStudents = student?.TotalStudents ?? 0;
            summary.ActiveStudents = student?.ActiveStudents ?? 0;
            summary.TotalCourses = course?.TotalCourses ?? 0;
            summary.ActiveCourses = course?.ActiveCourses ?? 0;
            summary.LastUpdatedAt = DateTime.UtcNow;

            await _tenantSummaryRepo.UpdateAsync(summary, autoSave: true);
            await _tenantSummaryCache.RemoveAsync(ReportingCacheKeys.Tenant(tenantId));
        }

        await uow.CompleteAsync();
    }
}
