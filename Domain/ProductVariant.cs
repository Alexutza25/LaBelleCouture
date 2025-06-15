using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Try.Domain;
[Table("ProductVariant")]
public class ProductVariant
{
    [Key]
    [Column("variantId")]
    public int VariantId { get; set; }
    
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    public virtual Product? Product { get; set; }

    [Column("stock")]
    public int Stock { get; set; }
    [Column("size")]
    public string Size { get; set; }
    
   
    [JsonIgnore]
    public  ICollection<Favourite> Favourites { get; set; }


}