using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Try.Domain;

[Table("Product")]
public class Product
{
    [Key]
    public int ProductId { get; set; }
    [Column("name")]
    public string Name { get; set; }
    [Column("description")]
    public string Description { get; set; }
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    [JsonIgnore]
    public Category Category { get; set; } 

    [Column("price")]
    public decimal Price { get; set; }
    [Column("colour")]
    public string Colour { get; set; }
    
    [Column("material")]
    public string Material { get; set; }
    
    [Column("imageUrl")]
    public string ImageURL { get; set; }
    
}