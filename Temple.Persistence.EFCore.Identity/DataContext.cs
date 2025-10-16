using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace PR.Web.Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(
            DbContextOptions<DataContext> options) : base(options)
        {
        }
    }
}