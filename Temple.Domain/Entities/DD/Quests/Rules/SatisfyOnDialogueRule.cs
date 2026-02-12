using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class SatisfyOnDialogueRule : IQuestRule
{
    private readonly string _npcId;

    public SatisfyOnDialogueRule(string npcId)
    {
        _npcId = npcId;
    }

    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Active &&
            e is DialogueEvent @event &&
            @event.NpcId == _npcId)
        {
            quest.MarkObjectivesCompleted();
        }
    }
}