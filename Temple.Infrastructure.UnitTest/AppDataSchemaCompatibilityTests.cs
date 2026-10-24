using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Temple.Persistence.EFCore.AppData;

namespace Temple.Infrastructure.UnitTest;

public class AppDataSchemaCompatibilityTests
{
    [Fact]
    public void CurrentModelHasTheSameRelationalSchemaAsTheLatestMigration()
    {
        using var context = new PRDbContextBase(new DbContextOptionsBuilder<PRDbContextBase>()
            .UseSqlite("Data Source=:memory:").Options);
        var migrations = context.GetService<IMigrationsAssembly>();
        var latest = migrations.CreateMigration(migrations.Migrations.Last().Value, context.Database.ProviderName!);
        var initializer = context.GetService<IModelRuntimeInitializer>();
        var previous = initializer.Initialize(latest.TargetModel, designTime: true);
        var current = context.GetService<IDesignTimeModel>().Model;
        var differences = context.GetService<IMigrationsModelDiffer>()
            .GetDifferences(previous.GetRelationalModel(), current.GetRelationalModel());

        Assert.Empty(differences);
        Assert.False(context.Database.HasPendingModelChanges());
    }
}
