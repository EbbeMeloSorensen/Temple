using Temple.Domain.Entities.DD.Quests;

namespace Temple.Application.Saves;

// Storage DTOs deliberately exclude runtime objects and event subscriptions.
public sealed class GameSave
{
    public const int CurrentFormatVersion = 1;
    // Increment when content identifiers or their interpretation become incompatible.
    public const string CurrentContentVersion = "1";

    public int FormatVersion { get; set; }
    public string ContentVersion { get; set; } = "";
    public DateTimeOffset SavedAtUtc { get; set; }
    public GameSaveLocation Location { get; set; }
    public string CurrentSiteId { get; set; } = "";
    public double? PositionX { get; set; }
    public double? PositionY { get; set; }
    public double? Orientation { get; set; }
    public List<PartyMemberSave> Party { get; set; } = null!;
    public List<QuestProgressSave> Quests { get; set; } = null!;
    public List<string> KnowledgeGained { get; set; } = null!;
    public List<string> FactsEstablished { get; set; } = null!;
    public List<string> BattlesWon { get; set; } = null!;
    public List<string> SitesUnlocked { get; set; } = null!;
}

public enum GameSaveLocation
{
    Exploration = 1,
    Wilderness = 2
}

public sealed class PartyMemberSave
{
    public Guid CreatureTypeId { get; set; }
    public int HitPoints { get; set; }
    public bool IsAutomatic { get; set; }
    public bool IsHostile { get; set; }
}

public sealed class QuestProgressSave
{
    public string QuestId { get; set; } = "";
    public QuestState State { get; set; }
    public bool AreCompletionCriteriaSatisfied { get; set; }
}
