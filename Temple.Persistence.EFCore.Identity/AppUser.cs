using Microsoft.AspNetCore.Identity;

namespace Temple.Persistence.EFCore.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
    }
}
