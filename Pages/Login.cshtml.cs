using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Try.DTO;

namespace Try.Pages;

public class LoginModel : PageModel
{
    private readonly HttpClient _httpClient;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string RedirectHref { get; set; } = "#";
    public string ErrorMessage { get; set; }

    public async Task OnPostAsync()
    {
        Console.WriteLine("am ajuns în OnPostAsync");

        RedirectHref = "#";
        ErrorMessage = string.Empty;
        
        var dto = new LoginDto { Email = Email, Password = Password };

        var context = new ValidationContext(dto);
        var results = new List<ValidationResult>();

        var response1 = await _httpClient.PostAsJsonAsync("/api/user/login", dto);
        Console.WriteLine("Status code: " + response1.StatusCode);
        
        if (!Validator.TryValidateObject(dto, context, results, true))
        {
            ErrorMessage = string.Join(" ", results.Select(r => r.ErrorMessage));
            return;
        }

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/user/login", dto);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserResponse>();
                RedirectHref = result.TypeUser == "Administrator" ? "/AdminDashboard" : "/";
            }
            else
            {
                ErrorMessage = "Email or password is incorrect.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error: " + ex.Message;
        }
        
    }

    public class UserResponse
    {
        public string TypeUser { get; set; }
    }
}