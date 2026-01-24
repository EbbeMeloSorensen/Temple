namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        string npcId);
}