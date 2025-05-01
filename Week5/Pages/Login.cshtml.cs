using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Week5.Models;

namespace Week5.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","data","users.json");

            if (!System.IO.File.Exists(filePath))
            {
                ErrorMessage = "User database not found.";
                return Page();
            }

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var users = JsonSerializer.Deserialize<List<User>>(json);

            var matchedUser = users?.FirstOrDefault(u =>
                u.Username == Username &&
                u.Password == Password &&
                u.IsActive);

            if (matchedUser == null)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            // Token  
            var token = Guid.NewGuid().ToString();

            // Session 
            HttpContext.Session.SetString("username", matchedUser.Username);
            HttpContext.Session.SetString("token", token);
            HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

            // Cookie 
            var cookieOptions = new CookieOptions
            {
                Expires = DateTimeOffset.Now.AddMinutes(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            Response.Cookies.Append("username", matchedUser.Username, cookieOptions);
            Response.Cookies.Append("token", token, cookieOptions);
            Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

            
            return RedirectToPage("Index");
        }
    }
}
