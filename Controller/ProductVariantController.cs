using Microsoft.AspNetCore.Mvc;
using Try.Domain;
using Try.DTO;
using Try.Service.ProductVariantService;

namespace Try.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductVariantController : ControllerBase
{
    private readonly IProductVariantService _productVariantService;

    public ProductVariantController(IProductVariantService productVariantService)
    {
        _productVariantService = productVariantService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllVariants()
    {
        var variants = await _productVariantService.GetAllProductVariants();
        return Ok(variants);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVariantById(int id)
    {
        var variant = await _productVariantService.GetProductVariantById(id);
        if (variant == null) return NotFound();
        return Ok(variant);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVariant([FromBody] ProductVariantDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var variant = new ProductVariant
        {
            ProductId = dto.ProductId,
            Size = dto.Size,
            Stock = dto.Stock
        };

        var created = await _productVariantService.CreateProductVariant(variant);

        return CreatedAtAction(nameof(GetVariantById), new { id = created.VariantId }, created);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductVariantDto dto)
    {
        var variant = new ProductVariant
        {
            VariantId = id, 
            ProductId = dto.ProductId,
            Size = dto.Size,
            Stock = dto.Stock
        };

        var updated = await _productVariantService.UpdateProductVariant(variant);
        return Ok(updated);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteVariant(int id)
    {
        var deleted = await _productVariantService.DeleteProductVariant(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
    
    [HttpPut("{id}/decreaseStock")]
    public async Task<IActionResult> DecreaseStock(int id, [FromQuery] int amount)
    {
        var variant = await _productVariantService.GetProductVariantById(id);
        if (variant == null) return NotFound();

        variant.Stock = Math.Max(0, variant.Stock - amount);
        await _productVariantService.UpdateProductVariant(variant);

        return Ok(variant);
    }

}