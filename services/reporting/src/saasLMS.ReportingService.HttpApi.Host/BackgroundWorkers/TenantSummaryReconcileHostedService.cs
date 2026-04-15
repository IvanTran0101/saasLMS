using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using saasLMS.ReportingService.Reports;

namespace saasLMS.ReportingService.BackgroundWorkers;

public sealed class TenantSummaryReconcileHostedService : BackgroundService
{
    private static readonly TimeZoneInfo VietnamTimeZone = ResolveVietnamTimeZone();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenantSummaryReconcileHostedService> _logger;

    public TenantSummaryReconcileHostedService(
        IServiceProvider serviceProvider,
        ILogger<TenantSummaryReconcileHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = GetDelayToNextRun();
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stoppingToken);
            }

            await RunOnceAsync(stoppingToken);
        }
    }

    private async Task RunOnceAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var rebuilder = scope.ServiceProvider.GetRequiredService<TenantSummaryRebuilder>();
            await rebuilder.RebuildAllAsync();
            _logger.LogInformation("Tenant summary reconcile completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tenant summary reconcile failed.");
        }
    }

    private static TimeSpan GetDelayToNextRun()
    {
        var nowLocal = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, VietnamTimeZone);
        var next = new DateTimeOffset(nowLocal.Year, nowLocal.Month, nowLocal.Day, 0, 0, 0, nowLocal.Offset);
        if (nowLocal >= next)
        {
            next = next.AddDays(1);
        }
        return next - nowLocal;
    }

    private static TimeZoneInfo ResolveVietnamTimeZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
        }
        catch
        {
            return TimeZoneInfo.Local;
        }
    }
}
