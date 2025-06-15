using Try.DTO;

namespace Try.Service.IReviewService;

public interface IReviewService
{
    Task<bool> CanUserReviewAsync(int userId, int productId);
    Task AddReviewAsync(AddReviewDto dto);
    Task<List<ReviewDto>> GetReviewsForProductAsync(int productId);
}
