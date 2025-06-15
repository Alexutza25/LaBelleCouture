using Try.Domain;

namespace Try.Repository.ProductRepository;

public interface IProductRepository : IRepository<Product>
{
    Task<List<Product>> GetProductsByCategoryId(int categoryId);
    Task<Product?> GetProductWithCategoryById(int id);
    
    Task<IEnumerable<Product>> GetAllWithCategory();
    IQueryable<Product> GetAllProductsQueryable();
    
    Task<List<Product>> GetRecommendationsForUserAsync(int userId);

    Task<List<Product>> GetAssociatedProduct(int productId);





}