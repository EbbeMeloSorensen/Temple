using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        IQuestStateReadModel questStateReadModel,
        QuestEventBus eventBus,
        string npcId);
}