using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State;
using Temple.Domain.Entities.DD.Quests;
using Temple.Persistence;

namespace Temple.Application.UnitTest;

// (Made by Codex)

public class ApplicationInitializationTests
{
    [Fact]
    public async Task InitializeAsync_MigratesBeforeSeeding_AndReportsReadiness()
    {
        var initializer = new RecordingInitializer();
        var scope = new InitializerScope(initializer);
        var controller = CreateController(scope);
        var progress = new List<string>();
        controller.ProgressChanged += (_, message) => progress.Add(message);

        await controller.InitializeAsync();

        Assert.Equal(new[] { "migrate", "seed" }, initializer.Calls);
        Assert.Equal(new[]
        {
            "Initializing application...",
            "Applying database migrations...",
            "Seeding database...",
            "Application is ready."
        }, progress);
        Assert.True(scope.Disposed);
    }

    [Theory]
    [InlineData("migrate")]
    [InlineData("seed")]
    public async Task InitializeAsync_OnPersistenceFailure_DoesNotReportReadiness(string failingStep)
    {
        var initializer = new RecordingInitializer { FailingStep = failingStep };
        var scope = new InitializerScope(initializer);
        var controller = CreateController(scope);
        var progress = new List<string>();
        controller.ProgressChanged += (_, message) => progress.Add(message);

        await controller.InitializeAsync();

        Assert.Equal(failingStep == "migrate" ? new[] { "migrate" } : new[] { "migrate", "seed" }, initializer.Calls);
        Assert.DoesNotContain("Application is ready.", progress);
        Assert.True(scope.Disposed);
    }

    private static ApplicationController CreateController(InitializerScope scope) =>
        new(new ApplicationStateMachine(), scope, new EmptyGameIOHandler(),
            NullLogger<ApplicationController>.Instance);

    private sealed class RecordingInitializer : IAppDataInitializer
    {
        public List<string> Calls { get; } = new();
        public string? FailingStep { get; init; }

        public Task MigrateAsync() => Record("migrate");
        public Task SeedAsync() => Record("seed");

        private Task Record(string step)
        {
            Calls.Add(step);
            return step == FailingStep
                ? Task.FromException(new InvalidOperationException("Storage unavailable"))
                : Task.CompletedTask;
        }
    }

    private sealed class InitializerScope(IAppDataInitializer initializer)
        : IServiceScopeFactory, IServiceScope, IServiceProvider
    {
        public bool Disposed { get; private set; }
        public IServiceProvider ServiceProvider => this;
        public IServiceScope CreateScope() => this;
        public object? GetService(Type serviceType) =>
            serviceType == typeof(IAppDataInitializer) ? initializer : null;
        public void Dispose() => Disposed = true;
    }

    private sealed class EmptyGameIOHandler : IGameIOHandler
    {
        public IEnumerable<string> ReadSiteIdsFromDirectory(string directoryPath) => Array.Empty<string>();
        public IEnumerable<Quest> ReadQuestListFromFile(string fileName) => Array.Empty<Quest>();
        public void WriteQuestsToFile(IEnumerable<Quest> quests, string fileName) => throw new NotSupportedException();
    }
}
