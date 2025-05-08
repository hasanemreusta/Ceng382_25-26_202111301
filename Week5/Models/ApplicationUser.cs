using Microsoft.AspNetCore.Identity;

namespace Week5.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Özel kullanıcı özellikleri eklemek isterseniz, burada yapabilirsiniz.
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
