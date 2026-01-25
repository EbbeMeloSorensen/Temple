using Craft.DataStructures.Graph;

namespace Temple.Infrastructure.Dialogues;

public class DialogueVertex : EmptyVertex
{
    public string Text { get; set; }
    public IGameEventTrigger? GameEventTrigger { get; set; }

    public DialogueVertex()
    {
    }

    public DialogueVertex(
        string text,
        IGameEventTrigger? gameEventTrigger = null)
    {
        Text = text;
        GameEventTrigger = gameEventTrigger;
    }
}