using Craft.DataStructures.Graph;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.DD;

public class QuestManager : IQuestManager
{
    // Deprecated
    private GraphAdjacencyList<QuestVertex, EmptyEdge> _graph;

    public QuestManager()
    {
        var vertices = new List<QuestVertex>
        {
            new()
            {
                Quest = new NPCRequest()
                {
                    QuestId = 0,
                    SiteIdForQuestAcquisition = "Village",
                    SiteIdForQuestExecution = "Village",
                    Title = "Clear cellar",
                    Description = "Clear cellar of The Yawning Portal of rats.",
                    Status = QuestStatus.Available,
                    ModelId = "human female",
                    NPCName = "Innkeeper Cynthia",
                    Position = new Craft.Math.Point2D(11.5, 5.5),
                    Orientation = 90,
                    Height = 0
                }
            },
            new()
            {
                Quest = new NPCRequest()
                {
                    QuestId = 1,
                    SiteIdForQuestAcquisition = "Village",
                    SiteIdForQuestExecution = "Village",
                    Title = "Report back to the innkeeper Cynthia",
                    Description = "Report to the innkeeper Cynthia that the cellar was cleared successfully of rats.",
                    Status = QuestStatus.Unavailable,
                    ModelId = "human female",
                    NPCName = "Innkeeper Cynthia",
                    Position = new Craft.Math.Point2D(11.5, 5.5),
                    Orientation = 90,
                    Height = 0
                }
            }
        };

        _graph = new GraphAdjacencyList<QuestVertex, EmptyEdge>(vertices, directed:true);

        _graph.AddEdge(0, 1);
    }

    // Deprecated
    public IEnumerable<Quest> GetAllQuests()
    {
        return _graph.Vertices
            .Select(v => v.Quest);
    }

    // Deprecated
    public IEnumerable<Quest> GetAvailableAndStartedQuests()
    {
        return _graph.Vertices
            .Select(_ => _.Quest)
            .Where(_ => _.Status is QuestStatus.Available or QuestStatus.Started);
    }

    // Deprecated
    public Quest GetQuestById(
        int questId)
    {
        return ((QuestVertex)_graph.GetVertex(questId)).Quest;
    }

    // Deprecated
    public IEnumerable<Quest> GetSubsequentQuests(
        Quest quest)
    {
        var questVertex = _graph.Vertices.FirstOrDefault(_ => _.Quest.Equals(quest));

        if (questVertex == null)
        {
            throw new InvalidOperationException("unknown quest");
        }

        return _graph.NeighborIds(questVertex.Id)
            .Select(neighborId => ((QuestVertex)_graph.GetVertex(neighborId)).Quest);
    }
}