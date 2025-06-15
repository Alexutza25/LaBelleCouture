using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Try.Pages;

using System.ComponentModel.DataAnnotations;
using Try.DTO;

public class Signup1Model : PageModel
{
    [BindProperty]
    [ValidateNever]
    public SignUpDto SignUp { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return Redirect($"/Signup2?Name={SignUp.Name}&Email={SignUp.Email}&Phone={SignUp.Phone}&Password={SignUp.Password}&ConfirmPassword={SignUp.ConfirmPassword}&TypeUser={SignUp.TypeUser}");
    }
}
