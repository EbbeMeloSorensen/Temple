using Craft.DataStructures.Graph;
using Temple.Infrastructure.GameConditions;

namespace Temple.Infrastructure.Dialogues;

public class DialogueGraph
{
    public double Priority { get; set; }
    public IGameCondition? Condition { get; set; }
    public GraphAdjacencyList<DialogueVertex, DialogueEdge> Graph { get; set; }
}