namespace Temple.Infrastructure.Dialogues;

public class DialogueGraphCollection
{
    public IEnumerable<DialogueGraph> DialogueGraphs { get; }

    public DialogueGraphCollection(
        IEnumerable<DialogueGraph> dialogueGraphs)
    {
        DialogueGraphs = dialogueGraphs;
    }
}