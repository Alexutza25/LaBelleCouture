using Try.Domain;
using Try.Repository.ProductVariantRepository;

namespace Try.Service.ProductVariantService;

public class ProductVariantService : IProductVariantService
{
    private readonly IProductVariantRepository _productVariantRepository;

    public ProductVariantService(IProductVariantRepository productVariantRepository)
    {
        _productVariantRepository = productVariantRepository;
    }

    public async Task<IEnumerable<ProductVariant>> GetAllProductVariants()
    {
        return await _productVariantRepository.GetAll();
    }

    public async Task<ProductVariant?> GetProductVariantById(int id)
    {
        return await _productVariantRepository.GetById(id);
    }

    public async Task<ProductVariant?> CreateProductVariant(ProductVariant productVariant)
    {
        await _productVariantRepository.Add(productVariant);
        return productVariant;
    }

    public async Task<ProductVariant> UpdateProductVariant(ProductVariant productVariant)
    {
        await _productVariantRepository.Update(productVariant);
        return productVariant;
    }

    public async Task<bool> DeleteProductVariant(int id)
    {
        var productVariant = await _productVariantRepository.GetById(id);
        if(productVariant == null)
            return false;
        await _productVariantRepository.Delete(id);
        return true;
    }
}