using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class SatisfyOnDialogueRule : IQuestRule
{
    public string NPCId { get; set; }

    public SatisfyOnDialogueRule(string npcId)
    {
        NPCId = npcId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            e is DialogueEvent @event &&
            @event.NpcId == NPCId)
        {
            quest.MarkObjectivesCompleted();
        }
    }
}