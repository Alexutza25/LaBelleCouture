using Microsoft.EntityFrameworkCore;
using Try.Domain;

namespace Try.Repository.CartRepository;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    private readonly ApplicationDbContext _context;

    public CartRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cart>> GetCartByUserId(int userId)
    {
        return await _context.Carts
            .Include(c => c.ProductVariant)
            .ThenInclude(pv => pv.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<ProductVariant?> GetVariantById(int id)
    {
        return await _context.ProductVariants.FindAsync(id);
    }

    public async Task<Cart?> GetCartByUserAndVariant(int userId, int variantId)
    {
        return await _context.Carts.FirstOrDefaultAsync(c =>
            c.UserId == userId && c.VariantId == variantId);
    }
    
 

}