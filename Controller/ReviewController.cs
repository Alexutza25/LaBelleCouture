using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Try.DTO;
using Try.Service.IReviewService;


namespace Try.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetReviewsForProduct(int productId)
        {
            var reviews = await _reviewService.GetReviewsForProductAsync(productId);
            return Ok(reviews);
        }

    
        [HttpGet("can-review")]
        public async Task<IActionResult> CanReview(int userId, int productId)
        {
            var canReview = await _reviewService.CanUserReviewAsync(userId, productId);
            return Ok(canReview);
        }

        [HttpPost]
        public async Task<IActionResult> PostReview([FromBody] AddReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var canReview = await _reviewService.CanUserReviewAsync(dto.UserId, dto.ProductId);
                if (!canReview)
                    return Forbid("User hasn't purchased this product.");

                await _reviewService.AddReviewAsync(dto);
                return Ok("Review posted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

    }
}