using Temple.Application.Interfaces.Readers;
using Temple.Application.ReadModels;
using Temple.Application.State;
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

    public GameSession(IReadOnlyCollection<string> siteIds, IReadOnlyCollection<Quest> quests)
    {
        SiteIds = siteIds;
        Quests = quests;
        ApplicationData.CurrentSiteId = "village";
        GeneratePartyData();

        var knowledge = new KnowledgeGainedReadModel(EventBus);
        var facts = new FactsEstablishedReadModel(EventBus);
        var battles = new BattlesWonReadModel(EventBus);
        QuestStatusReader = new QuestStatusReadModel(EventBus);
        SitesUnlockedReader = new SitesUnlockedReadModel(EventBus);
        QuestStatusReader.Initialize(quests);
        SitesUnlockedReader.Initialize(siteIds);
        ApplicationData.SitesUnlocked.UnionWith(SitesUnlockedReader.SitesUnlocked);
        Queries = new GameQueryService(knowledge, facts, QuestStatusReader, battles, SitesUnlockedReader);

        EventBus.Subscribe<KnowledgeGainedEvent>(e => ApplicationData.KnowledgeGained.Add(e.KnowledgeId));
        EventBus.Subscribe<FactEstablishedEvent>(e => ApplicationData.FactsEstablished.Add(e.FactId));
        EventBus.Subscribe<BattleWonEvent>(e => ApplicationData.BattlesWon.Add(e.BattleId));
        EventBus.Subscribe<SiteUnlockedEvent>(e => ApplicationData.SitesUnlocked.Add(e.SiteId));
        _questRuntime = new QuestRuntime(quests, EventBus);
    }

    public void Dispose() => EventBus.Dispose();

    private void GeneratePartyData()
    {
        // Attacks
        var longSword = new MeleeAttack(name: "Longsword", maxDamage: 10);
        var bowAndArrow = new RangedAttack(name: "Bow & Arrow", maxDamage: 4, range: 5);

        // Creature types
        var knight = new CreatureType("Knight",
            maxHitPoints: 8, //20,
            armorClass: 3,
            thaco: 5,
            initiativeModifier: 0,
            movement: 4,
            attacks: new List<Attack>
            {
                longSword,
                longSword,
                longSword
            });

        var archer = new CreatureType(
            name: "Archer",
            maxHitPoints: 12,
            armorClass: 6,
            thaco: 14,
            initiativeModifier: 10,
            movement: 6,
            attacks: new List<Attack>
            {
                bowAndArrow,
                bowAndArrow
            });

        var adventurer1 = new Creature(knight, false);
        var adventurer2 = new Creature(archer, false);

        ApplicationData.Party.Add(adventurer1);
        ApplicationData.Party.Add(adventurer2);
    }

}
