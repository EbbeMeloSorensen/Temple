using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Temple.Persistence.EFCore.Dummies;
using Temple.Persistence.EFCore.Identity;
using Temple.Persistence.EFCore.AppData;

namespace Temple.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Original from project template
            //CreateHostBuilder(args).Build().Run();

            // Changes made by instructor (for creating the database from the migration)
            var host = CreateHostBuilder(args).Build();
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<DataContext>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                await context.Database.MigrateAsync();
                await Temple.Persistence.EFCore.Identity.Seed.SeedData(context, userManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration (1)");
            }

            try
            {
                var context = services.GetRequiredService<DataContext2>();
                await context.Database.MigrateAsync();
                await Temple.Persistence.EFCore.Dummies.Seed.SeedData(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration (2)");
            }

            try
            {
                var context = services.GetRequiredService<PRDbContextBase>();
                await context.Database.MigrateAsync();
                await Seeding.SeedDatabase(context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured during migration (3)");
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}