using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        string npcId)
    {
        return new DialogueSession();
    }
}