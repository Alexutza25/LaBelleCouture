using Try.Domain;

namespace Try.Service.ProductVariantService;

public interface IProductVariantService
{
    Task<IEnumerable<ProductVariant>> GetAllProductVariants();
    Task<ProductVariant?> GetProductVariantById(int id);
    Task<ProductVariant?> CreateProductVariant(ProductVariant productVariant);
    Task<ProductVariant?> UpdateProductVariant(ProductVariant productVariant);
    Task<bool> DeleteProductVariant(int id);
}