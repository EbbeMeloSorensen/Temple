using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Temple.Domain.Entities.Smurfs;
using Temple.Persistence.EFCore.AppData;

namespace Temple.Infrastructure.UnitTest;

public class SmurfRolePersistenceTests
{
    private const string InitialMigration = "20260813050542_InitialMigration";

    [Fact]
    public async Task UpgradePreservesExistingSmurfs_AndDefaultsTheirRoles()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateContext(connection);
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(InitialMigration);
        var existingId = Guid.NewGuid();
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO Smurfs (ID, Name) VALUES ({existingId}, {"Gammelsmolf"})");

        await migrator.MigrateAsync();

        var existing = await context.Smurfs.SingleAsync();
        Assert.Equal(existingId, existing.ID);
        Assert.Equal("Gammelsmolf", existing.Name);
        Assert.Equal(SmurfRole.Unspecified, existing.Role);

        // Inserts which do not use the domain constructor must receive the same default.
        var insertedId = Guid.NewGuid();
        await context.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO Smurfs (ID, Name) VALUES ({insertedId}, {"Smolfine"})");
        Assert.Equal(SmurfRole.Unspecified, (await context.Smurfs.SingleAsync(s => s.ID == insertedId)).Role);

        existing.Role = SmurfRole.Builder;
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        Assert.Equal(SmurfRole.Builder, (await context.Smurfs.SingleAsync(s => s.ID == existingId)).Role);
    }

    [Theory]
    [InlineData(SmurfRole.Unspecified)]
    [InlineData(SmurfRole.Builder)]
    [InlineData(SmurfRole.Farmer)]
    public async Task RolesRoundTripThroughPersistence(SmurfRole role)
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateContext(connection);
        await context.Database.MigrateAsync();
        var smurf = new Smurf { ID = Guid.NewGuid(), Name = "Smolfine", Role = role };
        context.Smurfs.Add(smurf);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var restored = await context.Smurfs.SingleAsync();

        Assert.Equal(smurf.ID, restored.ID);
        Assert.Equal("Smolfine", restored.Name);
        Assert.Equal(role, restored.Role);
        Assert.Equal("Smolfine", restored.ToString());
    }

    [Fact]
    public async Task DowngradePreservesSmurfIdentitiesAndNames()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        await using var context = CreateContext(connection);
        await context.Database.MigrateAsync();
        var id = Guid.NewGuid();
        context.Smurfs.Add(new Smurf { ID = id, Name = "Smolfine", Role = SmurfRole.Farmer });
        await context.SaveChangesAsync();

        await context.GetService<IMigrator>().MigrateAsync(InitialMigration);

        // Query the old schema directly because the current entity includes Role.
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT ID, Name FROM Smurfs";
        await using var reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.Equal(id, reader.GetGuid(0));
        Assert.Equal("Smolfine", reader.GetString(1));
    }

    private static PRDbContextBase CreateContext(SqliteConnection connection) => new(
        new DbContextOptionsBuilder<PRDbContextBase>().UseSqlite(connection).Options);
}
