using Microsoft.AspNetCore.Identity;

namespace PR.Web.Persistence
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;
    }
}
