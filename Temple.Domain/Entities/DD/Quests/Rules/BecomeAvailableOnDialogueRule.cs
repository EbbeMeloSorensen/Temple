using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class BecomeAvailableOnDialogueRule : IQuestRule
{
    private readonly string _npcId;

    public BecomeAvailableOnDialogueRule(string npcId)
    {
        _npcId = npcId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Hidden &&
            e is DialogueCompletedEvent d &&
            d.NpcId == _npcId)
        {
            quest.TransitionTo(QuestState.Available);
        }
    }
}