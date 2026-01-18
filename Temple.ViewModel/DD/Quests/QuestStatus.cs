using Temple.Domain.Entities.DD.Quests;

namespace Temple.ViewModel.DD.Quests;

// Dette er ikke det samme som QuestState. Det bruges til at notificere UI-laget om ændringer i quest-tilstande.
public sealed class QuestStatus
{
    public string QuestId { get; }
    public QuestState State { get; private set; }

    public QuestStatus(
        string questId,
        QuestState state)
    {
        QuestId = questId;
        State = state;
    }

    public void UpdateState(
        QuestState newState)
    {
        State = newState;
    }
}
