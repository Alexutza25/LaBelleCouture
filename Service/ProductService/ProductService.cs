using Try.Domain;
using Try.DTO;
using Try.Repository.ProductRepository;

namespace Try.Service.ProductService;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;

    }

    public async Task<IEnumerable<Product>> GetAllProducts()
    {
        return await _productRepository.GetAll();
    }

    public async Task<Product?> GetProductEntityById(int id)
    {
        return await _productRepository.GetProductWithCategoryById(id);
    }

    public async Task<ProductDetailsDto?> GetProductById(int id)
    {
        var product = await _productRepository.GetProductWithCategoryById(id);


        if (product == null) return null;

        return new ProductDetailsDto
        {
            ProductId = product.ProductId,
            Name = product.Name,
            Description = product.Description,
            Colour = product.Colour,
            Material = product.Material,
            Price = product.Price,
            ImageURL = product.ImageURL,
            Category = product.Category?.Name,
            CategoryId = product.CategoryId 

        };
    }


    public async Task<Product> CreateProduct(Product product)
    {
        await _productRepository.Add(product);
        return product;
    }

    
    public async Task<Product> UpdateProduct(Product product)
    {
        await _productRepository.Update(product);
        return product;
    }


    public async Task<bool> DeleteProduct(int id)
    {
        var existing = await _productRepository.GetById(id);
        if (existing == null) return false;
        await _productRepository.Delete(id);
        return true;
    }
    public async Task<List<Product>> GetProductsByCategoryId(int categoryId)
    {
        return await _productRepository.GetProductsByCategoryId(categoryId);
    }
    
    public async Task<IEnumerable<ProductDto>> SearchProducts(string? name)
    {
        var products = await _productRepository.GetAllWithCategory(); 

        if (!string.IsNullOrWhiteSpace(name))
        {
            var terms = name.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            products = products.Where(p =>
                terms.All(term =>
                    (p.Name != null && p.Name.ToLower().Contains(term)) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)) ||
                    (p.Colour != null && p.Colour.ToLower().Contains(term)) ||
                    (p.Material != null && p.Material.ToLower().Contains(term)) ||
                    (p.Category != null && p.Category.Name.ToLower().Contains(term))
                )
            );
        }


        return products.Select(p => new ProductDto
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            ImageURL = p.ImageURL,
            CategoryId = p.CategoryId,

        });
    }
    public List<Product> GetFilteredProducts(ProductFiltersDto filters)
    {
        var query = _productRepository.GetAllProductsQueryable();

        if (filters.Categories != null && filters.Categories.Any())
        {
            query = query.Where(p => filters.Categories.Contains(p.Category.Name));        }

        if (filters.Colors != null && filters.Colors.Any())
        {
            query = query.Where(p => filters.Colors.Contains(p.Colour));
        }

        if (filters.Materials != null && filters.Materials.Any())
        {
            query = query.Where(p => filters.Materials.Contains(p.Material));
        }

        if (filters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= filters.MaxPrice.Value);
        }

        return query.ToList();
    }
    
    public List<string> GetAllCategories()
    {
        return _productRepository
            .GetAllProductsQueryable()
            .Select(p => p.Category.Name)
            .Distinct()
            .ToList();
    }

    public List<string> GetAllColors()
    {
        return _productRepository
            .GetAllProductsQueryable()
            .Select(p => p.Colour)
            .Distinct()
            .ToList();
    }

    public List<string> GetAllMaterials()
    {
        return _productRepository
            .GetAllProductsQueryable()
            .Select(p => p.Material)
            .Distinct()
            .ToList();
    }

    public (decimal MinPrice, decimal MaxPrice) GetPriceRange()
    {
        var products = _productRepository.GetAllProductsQueryable();
        var min = products.Min(p => p.Price);
        var max = products.Max(p => p.Price);
        return (min, max);
    }
    public async Task<List<Product>> GetRecommendationsAsync(int userId)
    {
        return await _productRepository.GetRecommendationsForUserAsync(userId);
    }
    
    public async Task<List<Product>> GetAssociatedProduct(int productId)
    {
        return await _productRepository.GetAssociatedProduct(productId);
    }


}