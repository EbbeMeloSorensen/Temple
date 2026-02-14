using Craft.DataStructures.Graph;
using Temple.Infrastructure.Dialogues.DialogueGraphConditions;

namespace Temple.Infrastructure.Dialogues;

public class DialogueGraph
{
    public double Priority { get; set; }
    public IDialogueGraphCondition? Condition { get; set; }
    public GraphAdjacencyList<DialogueVertex, DialogueEdge> Graph { get; set; }
}