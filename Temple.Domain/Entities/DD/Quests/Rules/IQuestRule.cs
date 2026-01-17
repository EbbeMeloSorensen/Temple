using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Domain.Entities.DD.Quests.Rules;

public interface IQuestRule
{
    void Apply(Quest quest, IGameEvent e);
}