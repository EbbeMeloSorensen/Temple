using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class TurnInOnDialogueRule : IQuestRule
{
    public string NPCId { get; set; }

    public TurnInOnDialogueRule(string npcId)
    {
        NPCId = npcId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            quest.AreCompletionCriteriaSatisfied &&
            e is DialogueEvent d &&
            d.NpcId == NPCId)
        {
            quest.TransitionTo(QuestState.Completed);
        }
    }
}