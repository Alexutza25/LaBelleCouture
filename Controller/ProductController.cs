using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try.Domain;
using Try.DTO;
using Try.Service.ProductService;

namespace Try.Controller;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [EndpointSummary("afiseaza toate produsele")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _productService.GetAllProducts();
        return Ok(result);
    }

    [HttpGet("{id}")]
    [EndpointSummary("afiseaza un produs după ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _productService.GetProductById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
    
    [HttpPost]
    [RequestSizeLimit(10_000_000)] //limita la dim imaginii
    public async Task<IActionResult> Create([FromForm] CreateProductDto dto)
    {
        string? imagePath = null;

        if (dto.Image != null && dto.Image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var savePath = Path.Combine("wwwroot", "images", "products", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            imagePath = $"/images/products/{fileName}";
        }

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Colour = dto.Colour,
            Material = dto.Material,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            ImageURL = imagePath
        };

        await _productService.CreateProduct(product);

        return Ok(product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromForm] CreateProductDto dto)
    {
        var existingProduct = await _productService.GetProductEntityById(id);
        if (existingProduct == null)
            return NotFound("Product not found.");

        if (dto.Image != null && dto.Image.Length > 0)
        {
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
            var savePath = Path.Combine("wwwroot", "images", "products", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            existingProduct.ImageURL = $"/images/products/{fileName}";
        }

        existingProduct.Name = dto.Name;
        existingProduct.Description = dto.Description;
        existingProduct.Colour = dto.Colour;
        existingProduct.Material = dto.Material;
        existingProduct.Price = dto.Price;
        existingProduct.CategoryId = dto.CategoryId;

        var result = await _productService.UpdateProduct(existingProduct);
        return Ok(result);
    }



    [HttpDelete("{id}")]
    [EndpointSummary("sterge un produs după ID")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _productService.DeleteProduct(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
    
    [HttpGet("category/{categoryId}")]
    [EndpointSummary("Afișează produsele dintr-o categorie")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        var result = await _productService.GetProductsByCategoryId(categoryId);
        return Ok(result);
    }
    
    [HttpGet("search-by-name")]
    public async Task<IActionResult> SearchByName([FromQuery] string? name)
    {
        var results = await _productService.SearchProducts(name);
        return Ok(results);
    }
    
    [HttpPost("filter")]
    public IActionResult GetFilteredProducts([FromBody] ProductFiltersDto filters)
    {
        var filteredProducts = _productService.GetFilteredProducts(filters);
        return Ok(filteredProducts);
    }

    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        var categories = _productService.GetAllCategories();
        return Ok(categories);
    }

    [HttpGet("colors")]
    public IActionResult GetColors()
    {
        var colors = _productService.GetAllColors();
        return Ok(colors);
    }

    [HttpGet("materials")]
    public IActionResult GetMaterials()
    {
        var materials = _productService.GetAllMaterials();
        return Ok(materials);
    }

    [HttpGet("price-range")]
    public IActionResult GetPriceRange()
    {
        var (min, max) = _productService.GetPriceRange();
        return Ok(new { min, max });
    }
    
    [HttpGet("recommendations/{userId}")]
    public async Task<IActionResult> GetRecommendations(int userId)
    {
        var recommendations = await _productService.GetRecommendationsAsync(userId);
        return Ok(recommendations);
    }
    
    [HttpGet("{id}/associated")]
    public async Task<IActionResult> GetAssociatedProduct(int id)
    {
        var products = await _productService.GetAssociatedProduct(id);
        if (products == null || products.Count == 0)
            return NoContent();

        var result = products.Select(p => new AssociatedProductDto()
        {
            ProductId = p.ProductId,
            Name = p.Name,
            Price = p.Price,
            ImageUrl = p.ImageURL
        });

        return Ok(result);
    }



}