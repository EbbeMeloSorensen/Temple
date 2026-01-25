using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        QuestEventBus eventBus,
        string npcId);
}