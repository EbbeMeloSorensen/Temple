using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class BecomeAvailableOnDialogueRule : IQuestRule
{
    public string NPCId { get; set; }

    public BecomeAvailableOnDialogueRule(
        string npcId)
    {
        NPCId = npcId;
    }

    public void Apply(
        Quest quest,
        IGameEvent e)
    {
        if (quest.State == QuestState.Hidden &&
            e is DialogueEvent d &&
            d.NpcId == NPCId)
        {
            quest.TransitionTo(QuestState.Available);
        }
    }
}