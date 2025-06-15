using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Domain;
[Table("Category")]
public class Category
{
    [Key]
    public int CategoryId { get; set; }
    [Column("name")]
    public string Name { get; set; }
}