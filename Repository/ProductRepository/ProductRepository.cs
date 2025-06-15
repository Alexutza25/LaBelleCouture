using Microsoft.EntityFrameworkCore;
using Try.Domain;

namespace Try.Repository.ProductRepository;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetProductsByCategoryId(int categoryId)
    {
        if (_context == null || _context.Products == null)
            throw new Exception("Contextul sau Products e null!");

        var produse = await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();

        if (produse == null)
            return new List<Product>();

        return produse;
    }
    
    public async Task<Product?> GetProductWithCategoryById(int id)
    {
        if (_context == null || _context.Products == null)
            throw new Exception("Contextul sau Products e null!");

        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.ProductId == id);
    }
    
    public async Task<IEnumerable<Product>> GetAllWithCategory()
    {
        return await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
    }
    
    public IQueryable<Product> GetAllProductsQueryable()
    {
        return _context.Products.AsQueryable();
    }

    public void MarkCategoryAsUnchanged(Category category)
    {
        _context.Entry(category).State = EntityState.Unchanged;
    }
    
    public async Task<List<Product>> GetRecommendationsForUserAsync(int userId)
    {
        var favCategoryIds = await _context.Favourites
            .Where(f => f.UserId == userId)
            .Include(f => f.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Select(f => f.ProductVariant.Product.CategoryId)
            .ToListAsync();

        var cartCategoryIds = await _context.Carts
            .Where(c => c.UserId == userId)
            .Include(c => c.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Select(c => c.ProductVariant.Product.CategoryId)
            .ToListAsync();

        var orderCategoryIds = await _context.OrderDetails
            .Where(od => od.Order.UserId == userId)
            .Include(od => od.Order)
            .Include(od => od.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Select(od => od.ProductVariant.Product.CategoryId)
            .ToListAsync();

        var allCategoryIds = favCategoryIds
            .Concat(cartCategoryIds)
            .Concat(orderCategoryIds)
            .Distinct()
            .ToList();

        var usedProductIds = await _context.OrderDetails
            .Where(od => od.Order.UserId == userId)
            .Include(od => od.ProductVariant)
            .Select(od => od.ProductVariant.ProductId)
            .ToListAsync();

        var recommendations = await _context.Products
            .Where(p => allCategoryIds.Contains(p.CategoryId) && !usedProductIds.Contains(p.ProductId))
            .Take(10)
            .ToListAsync();

        return recommendations;
    }
    
    public async Task<List<Product>> GetAssociatedProduct(int productId)
    {
        // se obtin toate comenzile in care apare produsul dat
        var associatedOrderIds = await _context.OrderDetails
            .Where(od => od.ProductVariant.ProductId == productId)
            .Select(od => od.OrderId)
            .Distinct()
            .ToListAsync();

        // se grupeaza produsele cumparate impreuna, se numara si se sorteaza descrescator
        var productCounts = await _context.OrderDetails
            .Where(od => associatedOrderIds.Contains(od.OrderId) && od.ProductVariant.ProductId != productId)
            .GroupBy(od => od.ProductVariant.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                Count = group.Count()
            })
            .Where(g => g.Count >= 3)
            .OrderByDescending(g => g.Count) // sortate descrescator
            .ToListAsync();

        var sortedIds = productCounts.Select(g => g.ProductId).ToList();

        // se obtin produsele in ordinea frecventei
        var products = await _context.Products
            .Where(p => sortedIds.Contains(p.ProductId))
            .ToListAsync();

        // se sorteaza lista finala după ordinea din sortedIds
        var associatedProducts = sortedIds
            .Select(id => products.First(p => p.ProductId == id))
            .ToList();

        return associatedProducts;
    }




}