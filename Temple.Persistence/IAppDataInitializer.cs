namespace Temple.Persistence;

// (Made by Codex)

/// <summary>
/// Prepares application data storage without exposing the persistence provider.
/// </summary>
public interface IAppDataInitializer
{
    Task MigrateAsync();

    Task SeedAsync();
}
