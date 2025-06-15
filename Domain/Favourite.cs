using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Try.Domain;
[Table("Favourite")]
public class Favourite
{
    [Key]
    public int FavouriteId { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; } 
    [ForeignKey("ProductVariant")]
    public int VariantId { get; set; }  

    [InverseProperty("Favourites")]
    public  ProductVariant ProductVariant { get; set; }

    
}