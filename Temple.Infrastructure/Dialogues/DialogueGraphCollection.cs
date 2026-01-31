namespace Temple.Infrastructure.Dialogues;

public class DialogueGraphCollection
{
    public List<DialogueGraph> DialogueGraphs { get; }

    public DialogueGraphCollection(
        List<DialogueGraph> dialogueGraphs)
    {
        DialogueGraphs = dialogueGraphs;
    }
}