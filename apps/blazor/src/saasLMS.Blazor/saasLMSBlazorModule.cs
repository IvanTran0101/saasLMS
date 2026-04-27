using System.IO.Compression;
using saasLMS.Blazor.Client;
using Microsoft.AspNetCore.ResponseCompression;
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

        // Brotli + Gzip compression for dynamic responses (HTML, JSON, etc.)
        // Note: Blazor's pre-compressed _framework/*.br files are served by static file
        // middleware with Content-Encoding already set, so they won't be double-compressed.
        context.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
                "application/wasm",
                "image/svg+xml"
            ]);
        });
        context.Services.Configure<BrotliCompressionProviderOptions>(o =>
            o.Level = CompressionLevel.Fastest);
        context.Services.Configure<GzipCompressionProviderOptions>(o =>
            o.Level = CompressionLevel.Fastest);

        // Add services to the container.
        context.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents();

        // In containerized deployments we may mount `/app/wwwroot/libs` from the host.
        // Disable the startup-time libs check to avoid 500s when the folder is empty/missing.
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

        // Compression must come first so it wraps all subsequent middleware responses
        app.UseResponseCompression();

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
