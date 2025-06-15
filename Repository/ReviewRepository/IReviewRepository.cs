using Try.Domain;
using Try.DTO;

namespace Try.Repository.ReviewRepository;

public interface IReviewRepository
{
    Task<bool> HasUserPurchasedProduct(int userId, int productId);
    Task AddReviewAsync(Review review);
    Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId);
}
