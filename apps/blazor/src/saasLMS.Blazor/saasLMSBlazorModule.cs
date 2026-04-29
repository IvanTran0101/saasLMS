using saasLMS.Blazor.Client;
using Volo.Abp;
using Volo.Abp.Account.Pro.Public.Blazor.Shared.Pages.Account.Idle;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonXTheme.Bundling;
using Volo.Abp.AspNetCore.Components.WebAssembly.WebApp;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.AuditLogging.Blazor.WebAssembly.Bundling;
using Volo.Saas.Host.Blazor.WebAssembly.Bundling;
using Volo.Abp.Account.Pro.Public.Blazor.WebAssembly.Bundling;
using Volo.Abp.AspNetCore.Mvc.Libs;
using Volo.Abp.Ui.LayoutHooks;
using Prometheus;

namespace saasLMS.Blazor;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonXThemeBundlingModule),
    typeof(AbpAccountPublicBlazorWebAssemblyBundlingModule),
    typeof(AbpAuditLoggingBlazorWebAssemblyBundlingModule),
    typeof(SaasHostBlazorWebAssemblyBundlingModule),
    typeof(AbpAspNetCoreMvcUiBundlingModule)
)]
public class saasLMSBlazorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //https://github.com/dotnet/aspnetcore/issues/52530
        Configure<RouteOptions>(options =>
        {
            options.SuppressCheckForUnhandledSecurityMetadata = true;
        });

        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        // Production images bake `/app/wwwroot/libs` during the Docker build.
        // Keep the startup-time check disabled to avoid false negatives during cold start.
        Configure<AbpMvcLibsOptions>(options =>
        {
            options.CheckLibs = false;
        });

        Configure<AbpLayoutHookOptions>(hookOptions =>
        {
            hookOptions.Add(
                LayoutHooks.Body.Last,
                typeof(AccountIdleComponent)
            );
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var env = context.GetEnvironment();
        var app = context.GetApplicationBuilder();

        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseEndpoints(endpoints => endpoints.MapMetrics());
        app.MapAbpStaticAssets();
        app.UseAntiforgery();

        app.UseConfiguredEndpoints(builder =>
        {
            builder.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(
                    WebAppAdditionalAssembliesHelper.GetAssemblies<saasLMSBlazorClientModule>());
        });
    }
}
