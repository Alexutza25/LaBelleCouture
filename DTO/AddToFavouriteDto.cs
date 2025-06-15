using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class AddToFavouriteDto
{
    [Required(ErrorMessage = "user id is required.")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "variant id is required.")]
    public int VariantId { get; set; }
}