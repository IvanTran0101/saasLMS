using Localization.Resources.AbpUi;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using saasLMS.ProductService.Localization;
using saasLMS.ProductService.Permissions;
using saasLMS.ProductService.Web.Menus;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.PageToolbars;
using Volo.Abp.Mapperly;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace saasLMS.ProductService.Web;

[DependsOn(
    typeof(ProductServiceApplicationContractsModule),
    typeof(AbpAspNetCoreMvcUiThemeSharedModule),
    typeof(AbpMapperlyModule)
    )]
public class ProductServiceWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(typeof(ProductServiceResource), typeof(ProductServiceWebModule).Assembly);
        });

        PreConfigure<IMvcBuilder>(mvcBuilder =>
        {
            mvcBuilder.AddApplicationPartIfNotExists(typeof(ProductServiceWebModule).Assembly);
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new ProductServiceMenuContributor());
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<ProductServiceWebModule>();
        });

        context.Services.AddMapperlyObjectMapper<ProductServiceWebModule>();

        Configure<RazorPagesOptions>(options =>
        {
            options.Conventions.AuthorizePage("/Products/Index", ProductServicePermissions.Products.Default);
        });
        
        Configure<AbpPageToolbarOptions>(options =>
        {
            options.Configure<saasLMS.ProductService.Web.Pages.Products.IndexModel>(
                toolbar =>
                {
                    toolbar.AddButton(
                        LocalizableString.Create<ProductServiceResource>("NewProduct"),
                        icon: "plus",
                        name: "CreateProduct",
                        requiredPolicyName: ProductServicePermissions.Products.Create
                    );
                }
            );
        });

        
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<ProductServiceResource>()
                .AddBaseTypes(
                    typeof(AbpUiResource));
        });
    }
}
