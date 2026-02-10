using Craft.DataStructures.Graph;

namespace Temple.Infrastructure.Dialogues;

public class DialogueGraph
{
    public double Priority { get; set; }
    public IEnumerable<IDialogueGraphCondition>? Conditions { get; set; }
    public GraphAdjacencyList<DialogueVertex, DialogueEdge> Graph { get; set; }
}