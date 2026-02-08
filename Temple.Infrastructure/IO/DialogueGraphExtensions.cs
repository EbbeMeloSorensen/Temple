using System.Text;
using Temple.Infrastructure.Dialogues;

namespace Temple.Infrastructure.IO;

public static class DialogueGraphExtensions
{
    public static void WriteToDotFile(
        this DialogueGraph dialogueGraph,
        string fileName)
    {
        // We don't do this - we need more control
        //dialogueGraph.Graph.WriteToFile(fileName, Format.Dot);

        using var streamWriter = new StreamWriter(fileName);
        streamWriter.WriteLine("digraph {");
        streamWriter.WriteLine("  node [shape=ellipse];");
        var graph = dialogueGraph.Graph;

        int? vertexLimit = null;

        // Add vertices to graph
        for (var vertexId = 0; vertexId < graph.VertexCount; vertexId++)
        {
            var vertex = (DialogueVertex) graph.GetVertex(vertexId);

            var text = vertex.Text;

            text = text.Replace("\"", "\\\"");

            //text = WrapForEllipse(text, 40, 3); // Doesn't work
            text = WrapByMaxLength(text, 40);

            // Add Game event trigger
            if (vertex.GameEventTrigger != null)
            {
                text = $"{text}\\n\\n{vertex.GameEventTrigger}";
            }

            streamWriter.WriteLine($"  {vertexId} [label=\"{text}\"];");

            if (vertexLimit.HasValue && vertexLimit <= vertexId + 1)
            {
                break;
            }
        }

        // Add edges to graph
        graph.Edges.ForEach(edge =>
        {
            var text = edge.Text;
            text = text.Replace("\"", "\\\"");
            text = WrapByMaxLength(text, 30);

            if (edge.KnowledgeRequired != null)
            {
                text = $"{text}\\n\\nKnowledge required: {edge.KnowledgeRequired}";
            }

            streamWriter.WriteLine($"  {edge.VertexId1}->{edge.VertexId2} [label=\"{text}\"]");
        });


        streamWriter.WriteLine("}");
    }

    private static string WrapForEllipse(
        string text,
        int maxWidth,
        int minWidth = 10)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // First: rough estimate of line count
        var rough = WrapByMaxLength(text, maxWidth)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);

        var lineCount = rough.Length;

        var widths = ComputeEllipseWidths(lineCount, maxWidth, minWidth);
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var lines = new List<string>();
        var current = new StringBuilder();
        var lineIndex = 0;

        foreach (var word in words)
        {
            var targetWidth = widths[Math.Min(lineIndex, widths.Length - 1)];

            if (current.Length == 0)
            {
                current.Append(word);
            }
            else if (current.Length + 1 + word.Length <= targetWidth)
            {
                current.Append(' ').Append(word);
            }
            else
            {
                lines.Add(current.ToString());
                current.Clear();
                lineIndex++;
                current.Append(word);
            }
        }

        if (current.Length > 0)
            lines.Add(current.ToString());

        return string.Join("\n", lines);
    }

    private static int[] ComputeEllipseWidths(
        int lineCount,
        int maxWidth,
        int minWidth)
    {
        var widths = new int[lineCount];
        var mid = (lineCount - 1) / 2.0;

        for (var i = 0; i < lineCount; i++)
        {
            var x = Math.Abs(i - mid) / mid; // 0 in center, 1 at edges
            var factor = 1.0 - x * x;        // parabola (ellipse-like)
            widths[i] = (int)(minWidth + factor * (maxWidth - minWidth));
        }

        return widths;
    }

    private static string WrapByMaxLength(
        string text,
        int maxLineLength)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var currentLine = new StringBuilder();

        foreach (var word in words)
        {
            if (currentLine.Length == 0)
            {
                currentLine.Append(word);
            }
            else if (currentLine.Length + 1 + word.Length <= maxLineLength)
            {
                currentLine.Append(' ').Append(word);
            }
            else
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear();
                currentLine.Append(word);
            }
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine.ToString());

        return string.Join("\\n", lines);
    }
}