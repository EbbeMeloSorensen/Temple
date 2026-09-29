using Microsoft.EntityFrameworkCore;

namespace Temple.Persistence.EFCore.AppData;

// (Made by Codex)
public class AppDataInitializer<TContext> : IAppDataInitializer
    where TContext : PRDbContextBase
{
    private readonly TContext _context;

    public AppDataInitializer(TContext context)
    {
        _context = context;
    }

    public Task MigrateAsync() => _context.Database.MigrateAsync();

    public Task SeedAsync() => Seeding.SeedDatabase(_context);
}
