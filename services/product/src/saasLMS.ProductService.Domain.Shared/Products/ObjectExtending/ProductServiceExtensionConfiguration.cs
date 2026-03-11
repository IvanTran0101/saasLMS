using System;
using Volo.Abp.ObjectExtending.Modularity;

namespace saasLMS.ProductService.Products.ObjectExtending;

public class ProductServiceExtensionConfiguration: ModuleExtensionConfiguration
{
    public ProductServiceExtensionConfiguration ConfigureProduct(
        Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(
            ProductServiceExtensionConsts.EntityNames.Product,
            configureAction
        );
    }
    
}