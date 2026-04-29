using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using saasLMS.EnrollmentService.DbMigrations;
using saasLMS.EnrollmentService.EntityFrameworkCore;
using saasLMS.Shared.Hosting.Microservices;
using saasLMS.Shared.Hosting.AspNetCore;
using Prometheus;
using saasLMS.CourseCatalogService;
using saasLMS.CourseCatalogService.Courses;
using Volo.Abp;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;

namespace saasLMS.EnrollmentService;

[DependsOn(
    typeof(saasLMSSharedHostingMicroservicesModule),
    typeof(EnrollmentServiceApplicationModule),
    typeof(EnrollmentServiceHttpApiModule),
    typeof(EnrollmentServiceEntityFrameworkCoreModule),
    typeof(CourseCatalogServiceApplicationContractsModule),
    typeof(AbpHttpClientIdentityModelWebModule)
    )]
public class EnrollmentServiceHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //You can disable this setting in production to avoid any potential security risks.
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        // Enable if you need these
        // var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        JwtBearerConfigurationHelper.Configure(context, "EnrollmentService");
        SwaggerConfigurationHelper.ConfigureWithOidc(
            context: context,
            authority: configuration["AuthServer:Authority"]!,
            scopes: new[] { "EnrollmentService", "CourseCatalogService" },
            flows: new[] { "authorization_code" },
            apiTitle: "EnrollmentService Service API"
        );
        context.Services.Configure<AbpClaimsPrincipalFactoryOptions>(options =>
        {
            options.IsDynamicClaimsEnabled = true;
        });
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.Trim().RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        context.Services.TransformAbpClaims();

        context.Services.AddHttpClientProxy<ICourseCatalogAppService>(
            CourseCatalogServiceRemoteServiceConsts.RemoteServiceName);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCorrelationId();
        app.UseAbpRequestLocalization();
        app.MapAbpStaticAssets();
        app.UseRouting();
        app.UseAbpSecurityHeaders();
        app.UseCors();
        app.UseHttpMetrics();
        app.UseAuthentication();
        app.UseMultiTenancy();
        app.UseUnitOfWork();
        app.UseDynamicClaims();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "EnrollmentService API");
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
        });
        app.UseAbpSerilogEnrichers();
        app.UseAuditing();
        app.UseConfiguredEndpoints(endpoints => endpoints.MapMetrics());
    }

    public async override Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        using (var scope = context.ServiceProvider.CreateScope())
        {
            await scope.ServiceProvider
                .GetRequiredService<EnrollmentServiceDatabaseMigrationChecker>()
                .CheckAndApplyDatabaseMigrationsAsync();
            await scope.ServiceProvider
                .GetRequiredService<EnrollmentServicePermissionSeeder>()
                .SeedAsync();
        }
    }
}
