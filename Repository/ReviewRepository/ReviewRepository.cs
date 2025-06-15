using Microsoft.EntityFrameworkCore;
using Try.Domain;
using Try.DTO;

namespace Try.Repository.ReviewRepository;

public class ReviewRepository : IReviewRepository
{
    private readonly ApplicationDbContext _context;

    public ReviewRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HasUserPurchasedProduct(int userId, int productId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .SelectMany(o => o.OrderDetails)
            .AnyAsync(od => od.ProductVariant.ProductId == productId);
    }

    public async Task AddReviewAsync(Review review)
    {
        _context.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId)
    {
        return await _context.Reviews
            .Where(r => r.ProductId == productId)
            .Include(r => r.User) 
            .Select(r => new ReviewDto
            {
                UserName = r.User != null ? r.User.Name : "N/A", 
                Rating = r.Rating,
                Comment = r.Comment,
                DatePosted = r.DatePosted
            })
            .OrderByDescending(r => r.DatePosted)
            .ToListAsync();

    }
}
