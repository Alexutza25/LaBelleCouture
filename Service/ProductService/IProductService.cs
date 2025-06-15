using Try.Domain;
using Try.DTO;

namespace Try.Service.ProductService;

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProducts();
    Task<ProductDetailsDto?> GetProductById(int id);
    Task<Product> CreateProduct(Product product);
    Task<Product> UpdateProduct(Product product);
    Task<bool> DeleteProduct(int id);
    Task<List<Product>> GetProductsByCategoryId(int categoryId);
    Task<IEnumerable<ProductDto>> SearchProducts(string? name);
    List<Product> GetFilteredProducts(ProductFiltersDto filters);
    List<string> GetAllCategories();
    List<string> GetAllColors();
    List<string> GetAllMaterials();
    (decimal MinPrice, decimal MaxPrice) GetPriceRange();
    Task<List<Product>> GetRecommendationsAsync(int userId);
    Task<List<Product>> GetAssociatedProduct(int productId);
    Task<Product?> GetProductEntityById(int id);



}