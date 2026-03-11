using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazorise;
using saasLMS.ProductService.Localization;
using saasLMS.ProductService.Permissions;
using saasLMS.ProductService.Products;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.EntityActions;
using Volo.Abp.AspNetCore.Components.Web.Extensibility.TableColumns;
using Volo.Abp.AspNetCore.Components.Web.Theming.PageToolbars;

namespace saasLMS.ProductService.Blazor.Pages.ProductService;

public partial class Products
{
    protected PageToolbar Toolbar { get; } = new PageToolbar();
    protected List<TableColumn> ProductTableColumns => TableColumns.Get<Index>();

    public Products()
    {
        LocalizationResource = typeof(ProductServiceResource);
        ObjectMapperContext = typeof(ProductServiceBlazorModule);

        CreatePolicyName = ProductServicePermissions.Products.Create;
        UpdatePolicyName = ProductServicePermissions.Products.Edit;
        DeletePolicyName = ProductServicePermissions.Products.Delete;
    }

    protected override ValueTask SetBreadcrumbItemsAsync()
    {
        BreadcrumbItems.Add(new Volo.Abp.BlazoriseUI.BreadcrumbItem(L["Menu:Products"]));
        return ValueTask.CompletedTask;
    }

    protected override string GetDeleteConfirmationMessage(ProductDto entity)
    {
        return string.Format(L["DeleteConfirmationMessage"], entity.Name);
    }

    protected override ValueTask SetToolbarItemsAsync()
    {
        Toolbar.AddButton(L["NewProduct"],
            OpenCreateModalAsync,
            IconName.Add,
            requiredPolicyName: CreatePolicyName);

        return base.SetToolbarItemsAsync();
    }

    protected override ValueTask SetEntityActionsAsync()
    {
        EntityActions
            .Get<Index>()
            .AddRange(new[]
            {
                    new EntityAction
                    {
                        Text = L["Edit"],
                        Visible = (data) => HasUpdatePermission,
                        Clicked = async (data) => { await OpenEditModalAsync(data.As<ProductDto>()); }
                    },
                    new EntityAction
                    {
                        Text = L["Delete"],
                        Visible = (data) => HasDeletePermission,
                        Clicked = async (data) => await DeleteEntityAsync(data.As<ProductDto>()),
                        ConfirmationMessage = (data) => GetDeleteConfirmationMessage(data.As<ProductDto>())
                    }
            });

        return base.SetEntityActionsAsync();
    }

    protected override ValueTask SetTableColumnsAsync()
    {
        ProductTableColumns
            .AddRange(new[]
            {
                    new TableColumn
                    {
                        Title = L["Actions"],
                        Actions = EntityActions.Get<Index>()
                    },
                    new TableColumn
                    {
                        Title = L["Name"],
                        Data = nameof(ProductDto.Name)
                    },
                    new TableColumn
                    {
                        Title = L["Price"],
                        Data = nameof(ProductDto.Price)
                    }
            });

        return base.SetTableColumnsAsync();
    }
}

