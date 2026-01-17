using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class AcceptQuestRule : IQuestRule
{
    public void Apply(Quest quest, IGameEvent e)
    {
        if (quest.State == QuestState.Available &&
            e is QuestAcceptedEvent accepted &&
            accepted.QuestId == quest.Id)
        {
            quest.TransitionTo(QuestState.Active);
        }
    }
}