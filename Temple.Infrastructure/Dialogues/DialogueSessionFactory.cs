using Temple.Application.Core;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        QuestEventBus eventBus,
        string npcId)
    {
        return new DialogueSession(eventBus);
    }
}