using Craft.DataStructures.Graph;
using Temple.Application.DD;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSession : IDialogueSession
{
    private GraphAdjacencyList<LabelledVertex, LabelledEdge> _graph;
    private int _activeVertexId;

    public string CurrentNPCText => ((LabelledVertex) _graph.GetVertex(_activeVertexId)).Label;

    public IReadOnlyList<DialogueChoice> AvailableChoices { get; }

    public DialogueSession()
    {
        var vertices = new List<LabelledVertex>
        {
            new("Beautiful song, isn't it? I've heard her sing a hundred times, and each time, it still moves me."),
            new("It's the spirit of an elven woman; she haunts this tavern, singing once every couple of nights."),
            new("No one truly knows. Her spirit was here when I first bought this tavern. Some say she sings for a lost love, a soldier who died defending Baldur's Gate. They say she sings in the hope he will her her voice and return home. Still, that's nothing but hearsay and tales - welcome to the Elfsong Tavern. What can I get you?"),
            new("Hmmm. Sounds like members of that new thieves' guild I've been hearing about. You're lucky to be alive. Word is, they're responsible for the murder of two city watchmen and the \"disappearance\" of several thieves from the old guild.")
        };

        _graph = new GraphAdjacencyList<LabelledVertex, LabelledEdge>(vertices, true);

        _graph.AddEdge(new LabelledEdge(0, 1, "It is beautiful... but where's the voice coming from?"));
        _graph.AddEdge(new LabelledEdge(1, 2, "Why does she sing?"));
        _graph.AddEdge(new LabelledEdge(2, 3, "I was hoping you could help me. I was robbed on the streets by a band of thieves, and I'm looking to find them."));

        _activeVertexId = 0;
    }

    public void SelectChoice(
        int choiceId)
    {
        throw new NotImplementedException();
    }

    public bool IsFinished { get; }
}