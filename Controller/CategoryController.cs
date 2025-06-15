using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Try.Domain;
using Try.DTO;
using Try.Service.CategoryService;

namespace Try.Controller;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [EndpointSummary("afiseaza toate categoriile de produse")]
    public async Task<IActionResult> GetAll()
    {
       
        try
        {
            var result = await _categoryService.GetAllCategories();
            var sortedResult = result.OrderBy(c => c.Name).ToList(); // sortam dupa numele categoriei
            return Ok(sortedResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message }); // JSON valid, nu text
        }
    }

    [HttpGet("{id}")]
    [EndpointSummary("afiseaza o categorie după ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _categoryService.GetCategoryById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddCategory([FromBody] CategoryDto dto)
    {

        try
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            if (role != "Administrator")
            {
                return Forbid("You must be an administrator to add a category.");
            }

            var result = await _categoryService.CreateCategory(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }


    [HttpPut("{id}")]
    [EndpointSummary("actualizeaza o categorie existenta")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
    {
        var existing = await _categoryService.GetCategoryById(id);
        if (existing == null) return NotFound();

        var category = new Category
        {
            CategoryId = id,
            Name = dto.Name
        };

        var updated = await _categoryService.UpdateCategory(category);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [EndpointSummary("sterge o categorie după id")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _categoryService.DeleteCategory(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}