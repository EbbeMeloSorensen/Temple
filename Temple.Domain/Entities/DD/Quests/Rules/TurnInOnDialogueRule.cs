using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class TurnInOnDialogueRule : IQuestRule
{
    private readonly string _npcId;

    public TurnInOnDialogueRule(string npcId)
    {
        _npcId = npcId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            quest.AreCompletionCriteriaSatisfied &&
            e is DialogueEvent d &&
            d.NpcId == _npcId)
        {
            quest.TransitionTo(QuestState.Completed);
        }
    }
}