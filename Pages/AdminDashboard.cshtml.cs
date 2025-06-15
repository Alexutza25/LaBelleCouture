using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Try.Pages;

public class AdminDashboardModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();

    }
}
