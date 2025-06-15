using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Try.DTO;

public class SignUpDto
{
    [Required(ErrorMessage = "Please enter your name")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Please enter your email")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Please enter your password correctly")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    public string Password { get; set; }
    
    
    [Required(ErrorMessage = "Please enter your password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

   [Phone(ErrorMessage = "Please enter your phone number")]
    public string Phone { get; set; }

    public string TypeUser { get; set; } = "Client";

    [ValidateNever]
    public AddressDto? Address { get; set; }

}