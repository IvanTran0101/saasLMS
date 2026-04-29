using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using saasLMS.CourseCatalogService.DbMigrations;
using saasLMS.CourseCatalogService.EntityFrameworkCore;
using saasLMS.Shared.Hosting.Microservices;
using saasLMS.Shared.Hosting.AspNetCore;
using Prometheus;
using Volo.Abp;
using Volo.Abp.BlobStoring;
using saasLMS.CourseCatalogService.BlobStoring;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.BlobStoring.Aws;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.AspNetCore.Mvc.Conventions;
using Volo.Abp.Modularity;
using Volo.Abp.Security.Claims;
using saasLMS.EnrollmentService.Enrollments;

namespace saasLMS.CourseCatalogService;

[DependsOn(
    typeof(saasLMSSharedHostingMicroservicesModule),
    typeof(CourseCatalogServiceApplicationModule),
    typeof(CourseCatalogServiceHttpApiModule),
    typeof(CourseCatalogServiceEntityFrameworkCoreModule),
    typeof(AbpHttpClientIdentityModelWebModule)
    )]
public class CourseCatalogServiceHttpApiHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //You can disable this setting in production to avoid any potential security risks.
        Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

        // Enable if you need these
        // var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        JwtBearerConfigurationHelper.Configure(context, "CourseCatalogService");
        context.Services.AddHttpClientProxies(
            typeof(CourseCatalogServiceApplicationContractsModule).Assembly);
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(
                typeof(CourseCatalogServiceApplicationModule).Assembly,
                opts =>
                {
                    opts.RootPath = "course-catalog";
                    opts.RemoteServiceName = CourseCatalogServiceRemoteServiceConsts.RemoteServiceName;
                });
        });
        SwaggerConfigurationHelper.ConfigureWithOidc(
            context: context,
            authority: configuration["AuthServer:Authority"]!,
            scopes: new[] { "CourseCatalogService", "EnrollmentService" },
            flows: new[] { "authorization_code" },
            apiTitle: "CourseCatalogService Service API"
        );
        context.Services.Configure<AbpBlobStoringOptions>(options =>
        {
            options.Containers.Configure<CourseMaterialContainer>(container =>
            {
                container.UseAws(aws =>
                {
                    var awsSection = configuration.GetSection("CourseCatalogService:Aws");
                    aws.UseCredentials = awsSection.GetValue<bool>("UseCredentials");
                    aws.Region = awsSection.GetValue<string>("Region") ?? "ap-southeast-1";
                    aws.ContainerName = awsSection.GetValue<string>("ContainerName") ?? "your-coursecatalog-bucket";
                    aws.CreateContainerIfNotExists = awsSection.GetValue<bool>("CreateContainerIfNotExists");

                    var profileName = awsSection.GetValue<string>("ProfileName");
                    var profilesLocation = awsSection.GetValue<string>("ProfilesLocation");

                    if (!string.IsNullOrWhiteSpace(profileName))
                    {
                        aws.ProfileName = profileName;
                    }

                    if (!string.IsNullOrWhiteSpace(profilesLocation))
                    {
                        aws.ProfilesLocation = profilesLocation;
                    }
                });
            });
        });


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

        context.Services.AddHttpClientProxy<IEnrollmentAppService>("EnrollmentService");
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
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "CourseCatalogService API");
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
                .GetRequiredService<CourseCatalogServiceDatabaseMigrationChecker>()
                .CheckAndApplyDatabaseMigrationsAsync();
            await scope.ServiceProvider
                .GetRequiredService<CourseCatalogServicePermissionSeeder>()
                .SeedAsync();
        }
    }
}
