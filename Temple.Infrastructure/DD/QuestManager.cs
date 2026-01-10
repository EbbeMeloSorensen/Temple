using Craft.DataStructures.Graph;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.DD
{
    public class QuestManager : IQuestManager
    {
        private GraphAdjacencyList<QuestVertex, EmptyEdge> _graph;

        public QuestManager()
        {
            var vertices = new List<QuestVertex>
            {
                new()
                {
                    Quest = new Quest
                    {
                        Id = 0,
                        Title = "Clear cellar",
                        Description = "Clear cellar of The Yawning Portal of rats.",
                        Status = QuestStatus.Available
                    }
                },
                new()
                {
                    Quest = new Quest
                    {
                        Id = 1,
                        Title = "Report back to the innkeeper Eve",
                        Description = "Report to the innkeeper Eve that the cellar was cleared successfully of rats.",
                        Status = QuestStatus.Unavailable
                    }
                },
                new()
                {
                    Quest = new Quest
                    {
                        Id = 2,
                        Title = "Investigate what happened to the town guard patrol",
                        Description = "A patrol of the town guard that was supposed to return two days ago is missing. The town guard Captain Boris wants you to investigate what happened to the patrol.",
                        Status = QuestStatus.Available
                    }
                },
                new()
                {
                    Quest = new Quest
                    {
                        Id = 3,
                        Title = "Report back to Captain Boris",
                        Description = "Report the findings of the investigation of the fate of the town guard patrol to Captain Boris.",
                        Status = QuestStatus.Unavailable
                    }
                },
                new()
                {
                    Quest = new Quest
                    {
                        Id = 4,
                        Title = "Talk with the wizard Cyrus",
                        Description = "Talk with the wizard Cyrus, as suggested by Captain Boris",
                        Status = QuestStatus.Unavailable
                    }
                },
                new()
                {
                    Quest = new Quest
                    {
                        Id = 5,
                        Title = "Investigate the tower ruins",
                        Description = "Investigate the tower ruins, as suggested by the wizard Cyrus",
                        Status = QuestStatus.Unavailable
                    }
                }
            };

            _graph = new GraphAdjacencyList<QuestVertex, EmptyEdge>(vertices, directed:true);

            _graph.AddEdge(0, 1);
            _graph.AddEdge(2, 3);
            _graph.AddEdge(3, 4);
            _graph.AddEdge(4, 5);
        }

        public IEnumerable<Quest> GetAllQuests()
        {
            return _graph.Vertices.Select(v => v.Quest);
        }

        public int GetQuestCount()
        {
            return _graph.Vertices.Count;
        }

        public Quest GetQuestById(
            int id)
        {
            var vertex = _graph.Vertices.FirstOrDefault(v => v.Quest.Id == id);

            if (vertex == null)
            {
                throw new ArgumentException($"Quest with id {id} not found.");
            }

            return vertex.Quest;
        }
    }
}
