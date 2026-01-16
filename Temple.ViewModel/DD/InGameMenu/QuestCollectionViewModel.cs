using System.Windows.Media;
using GalaSoft.MvvmLight;
using Craft.DataStructures.Graph;
using Craft.Utils;
using Craft.ViewModels.Graph;
using Temple.Application.Interfaces;

namespace Temple.ViewModel.DD.InGameMenu
{
    public class QuestCollectionViewModel : ViewModelBase
    {
        private Dictionary<int, QuestOld> _questMap = new();

        private readonly Brush _unavailableQuestBrush = new SolidColorBrush(Colors.IndianRed);
        private readonly Brush _availableQuestBrush = new SolidColorBrush(Colors.Orange);
        private readonly Brush _startedQuestBrush = new SolidColorBrush(Colors.DarkGreen);
        private readonly Brush _completedQuestBrush = new SolidColorBrush(Colors.White);
        private readonly Brush _failedQuestBrush = new SolidColorBrush(Colors.Black);

        public GraphViewModel GraphViewModel { get; }

        public QuestCollectionViewModel(
            IQuestManager questManager)
        {
            foreach (var quest in questManager.GetAllQuests())
            {
                _questMap[quest.QuestId] = quest;
            }

            var graph = GenerateGraph(questManager);

            GraphViewModel = new GraphViewModel(graph, 1200, 900);
            StyleGraph(graph);
        }

        private GraphAdjacencyList<LabelledVertex, EmptyEdge> GenerateGraph(
            IQuestManager questManager)
        {
            var vertices = questManager.GetAllQuests().Select(quest => new LabelledVertex(string.Empty));

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, directed:true);

            questManager.GetAllQuests()
                .ToList()
                .ForEach(quest =>
                {
                    questManager.GetSubsequentQuests(quest)
                        .ToList().ForEach(subsequentQuest =>
                        {
                            graph.AddEdge(quest.QuestId, subsequentQuest.QuestId);
                        });
                });

            return graph;
        }

        private void StyleGraph(
            IGraph<LabelledVertex, EmptyEdge> graph)
        {
            GraphViewModel.PlacePoint(0, new PointD(200, 50));
            GraphViewModel.PlacePoint(1, new PointD(200, 100));

            graph.Vertices.ForEach(v => GraphViewModel.StylePoint(v.Id, GetBrush(v.Id), _questMap[v.Id].Title));
        }

        private Brush GetBrush(
            int questId)
        {
            var quest = _questMap[questId];

            return quest.Status switch
            {
                QuestStatus.Unavailable => _unavailableQuestBrush,
                QuestStatus.Available => _availableQuestBrush,
                QuestStatus.Started => _startedQuestBrush,
                QuestStatus.Completed => _completedQuestBrush,
                QuestStatus.Failed => _failedQuestBrush,
                _ => throw new NotSupportedException($"Unknown quest status '{quest.Status}'.")
            };
        }
    }
}
