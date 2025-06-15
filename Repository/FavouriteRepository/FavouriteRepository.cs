using Microsoft.EntityFrameworkCore;
using Try.Domain;

namespace Try.Repository.FavouriteRepository;

public class FavouriteRepository : GenericRepository<Favourite>, IFavouriteRepository
{
    private readonly ApplicationDbContext _context;

    public FavouriteRepository(ApplicationDbContext _context) : base(_context)
    {
        this._context = _context;
    }
    
    public async Task<IEnumerable<Favourite>> GetFavouriteByUserId(int userId)
    {
        try
        {
            var rawFavourites = await _context.Favourites
                .Include(f => f.ProductVariant)
                .ThenInclude(pv => pv.Product) 
                .Where(f => f.UserId == userId)
                .ToListAsync();

            var cleanFavourites = rawFavourites
                .Where(f => f.ProductVariant != null)
                .ToList();

            Console.WriteLine($"clean favourites count: {cleanFavourites.Count}");
            return cleanFavourites;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Repo ERROR: " + ex.Message);
            return new List<Favourite>();
        }
    }



}