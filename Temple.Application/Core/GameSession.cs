using Temple.Application.Interfaces.Readers;
using Temple.Application.ReadModels;
using Temple.Application.State;
using Temple.Application.Saves;
using Craft.Math;
using Temple.Domain.Entities.DD.Battle;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Application.Core;

// All mutable progress and event subscriptions belong to this one game.
public sealed class GameSession : IDisposable
{
    private readonly QuestRuntime _questRuntime;

    public ApplicationData ApplicationData { get; } = new();
    public IReadOnlyCollection<string> SiteIds { get; }
    public IReadOnlyCollection<Quest> Quests { get; }
    public QuestEventBus EventBus { get; } = new();
    public IGameQueryService Queries { get; }
    public IQuestStatusReader QuestStatusReader { get; }
    public ISitesUnlockedReader SitesUnlockedReader { get; }

    internal GameSession(IReadOnlyCollection<string> siteIds, IReadOnlyCollection<Quest> quests, GameSave? save = null)
    {
        SiteIds = siteIds;
        Quests = quests;
        ApplicationData.CurrentSiteId = "village";
        var creatureTypes = PartyCatalog.CreateTypes();
        if (save is null)
        {
            ApplicationData.Party.AddRange(creatureTypes.Values.Select(type => new Creature(type, false)));
            if (siteIds.Contains("village")) ApplicationData.SitesUnlocked.Add("village");
        }
        else
        {
            RestoreProgress(save, creatureTypes);
        }

        var knowledge = new KnowledgeGainedReadModel(EventBus, ApplicationData.KnowledgeGained);
        var facts = new FactsEstablishedReadModel(EventBus, ApplicationData.FactsEstablished);
        var battles = new BattlesWonReadModel(EventBus, ApplicationData.BattlesWon);
        QuestStatusReader = new QuestStatusReadModel(EventBus);
        SitesUnlockedReader = new SitesUnlockedReadModel(EventBus, ApplicationData.SitesUnlocked);
        QuestStatusReader.Initialize(quests);
        Queries = new GameQueryService(knowledge, facts, QuestStatusReader, battles, SitesUnlockedReader);

        EventBus.Subscribe<KnowledgeGainedEvent>(e => ApplicationData.KnowledgeGained.Add(e.KnowledgeId));
        EventBus.Subscribe<FactEstablishedEvent>(e => ApplicationData.FactsEstablished.Add(e.FactId));
        EventBus.Subscribe<BattleWonEvent>(e => ApplicationData.BattlesWon.Add(e.BattleId));
        EventBus.Subscribe<SiteUnlockedEvent>(e => ApplicationData.SitesUnlocked.Add(e.SiteId));
        _questRuntime = new QuestRuntime(quests, EventBus);
    }

    private void RestoreProgress(GameSave save, IReadOnlyDictionary<Guid, CreatureType> creatureTypes)
    {
        if (!SiteIds.Contains(save.CurrentSiteId) || save.SitesUnlocked.Any(id => !SiteIds.Contains(id)))
            throw new InvalidDataException("The save references an unknown site.");
        var progress = save.Quests.ToDictionary(q => q.QuestId);
        if (progress.Count != Quests.Count || Quests.Any(q => !progress.ContainsKey(q.Id)))
            throw new InvalidDataException("The save's quests do not match the installed content.");
        foreach (var quest in Quests)
        {
            var saved = progress[quest.Id];
            quest.RestoreProgress(saved.State, saved.AreCompletionCriteriaSatisfied);
        }
        foreach (var member in save.Party)
        {
            if (!creatureTypes.TryGetValue(member.CreatureTypeId, out var type))
                throw new InvalidDataException("The save references an unknown creature definition.");
            var creature = new Creature(type, member.IsHostile);
            creature.RestoreProgress(member.HitPoints, member.IsAutomatic);
            ApplicationData.Party.Add(creature);
        }
        ApplicationData.CurrentSiteId = save.CurrentSiteId;
        ApplicationData.ExplorationPosition = save.PositionX is double x && save.PositionY is double y
            ? new Vector2D(x, y) : null;
        ApplicationData.ExplorationOrientation = save.Orientation;
        ApplicationData.KnowledgeGained.UnionWith(save.KnowledgeGained);
        ApplicationData.FactsEstablished.UnionWith(save.FactsEstablished);
        ApplicationData.BattlesWon.UnionWith(save.BattlesWon);
        ApplicationData.SitesUnlocked.UnionWith(save.SitesUnlocked);
    }

    // Called only at a paused application boundary; all collections are detached copies.
    public GameSave Capture(GameSaveLocation location) => new()
    {
        FormatVersion = GameSave.CurrentFormatVersion,
        ContentVersion = GameSave.CurrentContentVersion,
        SavedAtUtc = DateTimeOffset.UtcNow,
        Location = location,
        CurrentSiteId = ApplicationData.CurrentSiteId,
        PositionX = ApplicationData.ExplorationPosition?.X,
        PositionY = ApplicationData.ExplorationPosition?.Y,
        Orientation = ApplicationData.ExplorationOrientation,
        KnowledgeGained = ApplicationData.KnowledgeGained.ToList(),
        FactsEstablished = ApplicationData.FactsEstablished.ToList(),
        BattlesWon = ApplicationData.BattlesWon.ToList(),
        SitesUnlocked = ApplicationData.SitesUnlocked.ToList(),
        Party = ApplicationData.Party.Select(member => new PartyMemberSave
        {
            CreatureTypeId = member.CreatureType.ID,
            HitPoints = member.HitPoints,
            IsHostile = member.IsHostile,
            IsAutomatic = member.IsAutomatic
        }).ToList(),
        Quests = Quests.Select(quest => new QuestProgressSave
        {
            QuestId = quest.Id,
            State = quest.State,
            AreCompletionCriteriaSatisfied = quest.AreCompletionCriteriaSatisfied
        }).ToList()
    };

    public void Dispose() => EventBus.Dispose();

}
