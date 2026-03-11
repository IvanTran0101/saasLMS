using saasLMS.ProductService.Products;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace saasLMS.ProductService.Web;

[Mapper]
[MapExtraProperties]
public partial class ProductDtoToProductUpdateDtoMapper : MapperBase<ProductDto, ProductUpdateDto>
{
    public override partial ProductUpdateDto Map(ProductDto source);

    public override partial void Map(ProductDto source, ProductUpdateDto destination);
}
