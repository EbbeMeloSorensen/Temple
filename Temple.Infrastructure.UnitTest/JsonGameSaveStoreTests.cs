using Temple.Application.Saves;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.UnitTest;

public sealed class JsonGameSaveStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "Temple-save-tests-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task SessionSnapshotCanBeRestoredAfterDiskRoundTrip()
    {
        var factory = new GameSessionFactory(new Content(), new GameSaveValidator());
        using var original = factory.CreateNew();
        original.EventBus.Publish(new KnowledgeGainedEvent("secret"));
        original.ApplicationData.Party[0].HitPoints = 3;
        var store = new JsonGameSaveStore(_directory);
        await store.WriteAsync(1, original.Capture(GameSaveLocation.Wilderness));

        using var restored = factory.Restore(await store.ReadAsync(1));

        Assert.True(restored.Queries.IsKnowledgeGained("secret"));
        Assert.Equal(3, restored.ApplicationData.Party[0].HitPoints);
        Assert.Equal(new[] { "village" }, restored.SitesUnlockedReader.SitesUnlocked);
    }

    [Fact]
    public async Task SlotsRoundTripAndOverwriteIndependently_WithoutSerializingRuntimeTypes()
    {
        var store = new JsonGameSaveStore(_directory);
        await store.WriteAsync(1, Save("first"));
        await store.WriteAsync(2, Save("second"));
        await store.WriteAsync(1, Save("replacement"));

        var loaded = await store.ReadAsync(1);
        Assert.Equal("replacement", loaded.CurrentSiteId);
        Assert.Equal("second", (await store.ReadAsync(2)).CurrentSiteId);
        Assert.Equal(2.5, loaded.PositionX);
        Assert.Equal(QuestId, loaded.Quests.Single().QuestId);
        Assert.Equal(CreatureId, loaded.Party.Single().CreatureTypeId);
        var json = await File.ReadAllTextAsync(Path.Combine(_directory, "slot-1.json"));
        Assert.DoesNotContain("$type", json);
        Assert.DoesNotContain("EventBus", json);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    [Fact]
    public async Task FailedReplacement_PreservesPreviousSave_AndCleansTemporaryFile()
    {
        var store = new JsonGameSaveStore(_directory);
        await store.WriteAsync(1, Save("original"));
        var path = Path.Combine(_directory, "slot-1.json");
        // Windows prevents replacing a file opened without delete sharing.
        await using (var locked = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None))
        {
            var failure = await Record.ExceptionAsync(() => store.WriteAsync(1, Save("replacement")));
            Assert.True(failure is IOException or UnauthorizedAccessException, failure?.ToString());
        }
        Assert.Equal("original", (await store.ReadAsync(1)).CurrentSiteId);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    [Fact]
    public async Task CanceledWrite_PreservesPreviousSave()
    {
        var store = new JsonGameSaveStore(_directory);
        await store.WriteAsync(1, Save("original"));
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => store.WriteAsync(1, Save("replacement"), cancellation.Token));
        Assert.Equal("original", (await store.ReadAsync(1)).CurrentSiteId);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("null")]
    [InlineData("{\"$type\":\"Some.Runtime.Type\"}")]
    public async Task CorruptFilesAreReportedAsInvalidData(string contents)
    {
        Directory.CreateDirectory(_directory);
        await File.WriteAllTextAsync(Path.Combine(_directory, "slot-1.json"), contents);
        await Assert.ThrowsAsync<InvalidDataException>(() => new JsonGameSaveStore(_directory).ReadAsync(1));
    }

    [Fact]
    public async Task MissingPartyFieldsCannotSilentlyRestoreDefaultHitPoints()
    {
        var store = new JsonGameSaveStore(_directory);
        await store.WriteAsync(1, Save("village"));
        var path = Path.Combine(_directory, "slot-1.json");
        var json = Newtonsoft.Json.Linq.JObject.Parse(await File.ReadAllTextAsync(path));
        ((Newtonsoft.Json.Linq.JObject)json["Party"]![0]!).Remove("HitPoints");
        await File.WriteAllTextAsync(path, json.ToString());

        await Assert.ThrowsAsync<InvalidDataException>(() => store.ReadAsync(1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task InvalidSlotsCannotAddressOtherFiles(int slot)
    {
        var store = new JsonGameSaveStore(_directory);
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => store.ReadAsync(slot));
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => store.WriteAsync(slot, Save("site")));
        Assert.False(Directory.Exists(_directory));
    }

    private const string QuestId = "quest";
    private sealed class Content : IGameIOHandler
    {
        public IEnumerable<string> ReadSiteIdsFromDirectory(string path) => new[] { "village" };
        public IEnumerable<Quest> ReadQuestListFromFile(string path) => Array.Empty<Quest>();
        public void WriteQuestsToFile(IEnumerable<Quest> quests, string path) => throw new NotSupportedException();
    }
    private static readonly Guid CreatureId = Guid.Parse("f3827714-2366-42ca-b053-c1f229e2d001");
    private static GameSave Save(string site) => new()
    {
        FormatVersion = GameSave.CurrentFormatVersion,
        ContentVersion = GameSave.CurrentContentVersion,
        SavedAtUtc = DateTimeOffset.UtcNow,
        CurrentSiteId = site,
        Location = GameSaveLocation.Exploration,
        PositionX = 2.5, PositionY = 7.25, Orientation = 37,
        Party = new() { new PartyMemberSave { CreatureTypeId = CreatureId, HitPoints = -2 } },
        Quests = new() { new QuestProgressSave { QuestId = QuestId } },
        KnowledgeGained = new() { "secret" }, FactsEstablished = new(), BattlesWon = new(), SitesUnlocked = new() { site }
    };

    public void Dispose()
    {
        // Only remove this test instance's explicitly created temporary files.
        if (!Directory.Exists(_directory)) return;
        foreach (var file in Directory.GetFiles(_directory)) File.Delete(file);
        Directory.Delete(_directory);
    }
}
