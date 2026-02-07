using Craft.DataStructures.Graph;

namespace Temple.Infrastructure.Dialogues;

public class DialogueEdge : IEdge
{
    public int VertexId1 { get; }
    public int VertexId2 { get; }

    public string Text { get; set; }

    public DialogueEdge(
        int vertexId1,
        int vertexId2,
        string text)
    {
        VertexId1 = vertexId1;
        VertexId2 = vertexId2;
        Text = text;
    }
}