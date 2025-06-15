using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Try.Domain;
using Try.DTO;
using Try.Service.FavouriteService;

namespace Try.Controller;

[ApiController]
[Route("api/[controller]")]
public class FavouriteController : ControllerBase
{
    private readonly IFavouriteService _favouriteService;

    public FavouriteController(IFavouriteService favouriteService)
    {
        _favouriteService = favouriteService;
    }
    [Authorize]
    [HttpGet]
    [EndpointSummary("afiseaza toate produsele favorite")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _favouriteService.GetAllFavourites();
        return Ok(result);
    }
    [Authorize]
    [HttpGet("{id}")]
    [EndpointSummary("afiseaza un produs favorit după ID")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _favouriteService.GetFavouriteById(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
    [HttpPost]
    [EndpointSummary("adauga un produs la favorite")]
    public async Task<IActionResult> Create([FromBody] AddToFavouriteDto dto)
    {
        Console.WriteLine($"🔍 DTO primit: UserId={dto.UserId}, VariantId={dto.VariantId}");

        
        var favourite = new Favourite
        {
            UserId = dto.UserId,
            VariantId = dto.VariantId
        };

        var created = await _favouriteService.CreateFavourite(favourite);
        return CreatedAtAction(nameof(GetById), new { id = created.FavouriteId }, created);
    }


    [HttpPut("{id}")]
    [EndpointSummary("actualizeaza un produs favorit")]
    public async Task<IActionResult> Update(int id, [FromBody] Favourite item)
    {
        if (item.FavouriteId != id) return BadRequest();
        var updated = await _favouriteService.UpdateFavourite(item);
        return Ok(updated);
    }

    [Authorize]
    [HttpDelete("{id}")]
    [EndpointSummary("sterge un produs din favorite")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _favouriteService.DeleteFavourite(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
    
    [Authorize]
    [HttpGet("my")] 
    [EndpointSummary("afiseaza toate favourite-urile userului logat")]
    public async Task<IActionResult> GetByUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            Console.WriteLine("OK PANA AICI 1");

            var userId = int.Parse(userIdClaim.Value);
            Console.WriteLine(userId);

            Console.WriteLine("OK PANA AICI 3");

            var favourites = await _favouriteService.GetFavouriteById(userId);

            if (favourites == null || !favourites.Any())
            {
                Console.WriteLine("Niciun favourite gasit.");
                return Ok(new List<Favourite>());
            }

            Console.WriteLine("OK PANA AICI 2");
            foreach (var fav in favourites)
            {
                if (fav.ProductVariant == null)
                {
                    Console.WriteLine($"❌ FavouriteId {fav.FavouriteId} nu are ProductVariant.");
                }
                else if (fav.ProductVariant.Product == null)
                {
                    Console.WriteLine($"❌ VariantId {fav.ProductVariant.VariantId} nu are Product.");
                }
            }

            return Ok(favourites);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Eroare in /api/favourite/my: " + ex.Message);
            return StatusCode(500, "Server error");
        }
    }


}