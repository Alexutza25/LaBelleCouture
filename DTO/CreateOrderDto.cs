using System.ComponentModel.DataAnnotations;

namespace Try.DTO;

public class CreateOrderDto
{
    [Required(ErrorMessage = "ID-ul utilizatorului este obligatoriu.")]
    public int UserId { get; set; }
    [Required(ErrorMessage = "Metoda de plată este obligatorie.")]
    
    public DateTime date { get; set; }

    public string PaymentMethod { get; set; }
    public List<CreateOrderDetailsDto> OrderDetails { get; set; }
}