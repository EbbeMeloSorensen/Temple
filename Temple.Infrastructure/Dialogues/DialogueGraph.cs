using Craft.DataStructures.Graph;

namespace Temple.Infrastructure.Dialogues;

// Den her skal kunne serialiseres til/fra json
public class DialogueGraph
{
    public double Priority { get; set; }
    public IEnumerable<DialogueGraphCondition>? Conditions { get; set; }
    public GraphAdjacencyList<DialogueVertex, LabelledEdge> Graph { get; set; }
}