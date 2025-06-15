using Try.Domain;
using Try.DTO;
using Try.Repository.ReviewRepository;

namespace Try.Service.IReviewService;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _repository;

    public ReviewService(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> CanUserReviewAsync(int userId, int productId)
    {
        return await _repository.HasUserPurchasedProduct(userId, productId);
    }

    public async Task AddReviewAsync(AddReviewDto dto)
    {
        var review = new Review
        {
            UserId = dto.UserId,
            ProductId = dto.ProductId,
            Rating = dto.Rating,
            Comment = dto.Comment,
            DatePosted = DateTime.UtcNow
        };

        await _repository.AddReviewAsync(review);
    }

    public Task<List<ReviewDto>> GetReviewsForProductAsync(int productId)
    {
        return _repository.GetReviewsByProductIdAsync(productId);
    }
}