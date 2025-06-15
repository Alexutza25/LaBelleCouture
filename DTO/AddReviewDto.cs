namespace Try.DTO;

public class AddReviewDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
}
