using Craft.DataStructures.Graph;

namespace Temple.Infrastructure.Dialogues;

public class DialogueGraph
{
    public double Priority { get; set; }
    public IEnumerable<DialogueGraphCondition>? Conditions { get; set; }
    public GraphAdjacencyList<DialogueVertex, LabelledEdge> Graph { get; set; }
}