using Craft.DataStructures.IO;
using Temple.Infrastructure.Dialogues;

namespace Temple.Infrastructure.IO;

public static class DialogueGraphExtensions
{
    public static void WriteToDotFile(
        this DialogueGraph dialogueGraph,
        string fileName)
    {
        dialogueGraph.Graph.WriteToFile(fileName, Format.Dot);
    }
}