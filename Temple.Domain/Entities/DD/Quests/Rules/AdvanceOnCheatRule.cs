using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public sealed class AdvanceOnCheatRule : IQuestRule
{
    public void Apply(Quest quest, IGameEvent e)
    {
        if (e is not AdvanceQuestByCheatingEvent b ||
            b.QuestId != quest.Id)
        {
            return;
        }

        if (quest.State == QuestState.Hidden)
        {
            quest.TransitionTo(QuestState.Available);
            return;
        }

        if (quest.State == QuestState.Available)
        {
            quest.TransitionTo(QuestState.Active);
            return;
        }

        if (quest.State == QuestState.Active)
        {
            if (quest.AreCompletionCriteriaSatisfied)
            {
                quest.TransitionTo(QuestState.Completed);
            }
            else
            {
                quest.MarkObjectivesCompleted();
            }
        }
    }
}