using System.ComponentModel.DataAnnotations;

namespace Try.Domain;

public class Review
{
    public int ReviewId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string Comment { get; set; }

    public DateTime DatePosted { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public User User { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }
}
