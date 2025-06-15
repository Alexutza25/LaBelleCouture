using System.ComponentModel.DataAnnotations;

namespace Try.DTO
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }

        [Required]
        public AddressDto Address { get; set; }
    }
}