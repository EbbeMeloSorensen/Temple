using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.DataStructures.Graph;
using Craft.Utils;
using Craft.ViewModels.Graph;
using Temple.Application.Core;
using Temple.ViewModel.DD.Quests;

namespace Temple.ViewModel.DD.InGameMenu
{
    public class QuestCollectionViewModel : ViewModelBase
    {
        private readonly Brush _unavailableQuestBrush = new SolidColorBrush(Colors.IndianRed);
        private readonly Brush _availableQuestBrush = new SolidColorBrush(Colors.Orange);
        private readonly Brush _startedQuestBrush = new SolidColorBrush(Colors.DarkGreen);
        private readonly Brush _completedQuestBrush = new SolidColorBrush(Colors.White);
        private readonly Brush _failedQuestBrush = new SolidColorBrush(Colors.Black);

        public GraphViewModel GraphViewModel { get; }

        public ObservableCollection<QuestViewModel> Quests { get; }

        public RelayCommand<string> CheatCompleteQuest_Command { get; }

        public QuestCollectionViewModel(
            QuestStatusReadModel questStatusReadModel,
            QuestEventBus eventBus)
        {
            var graph = GenerateGraph();

            GraphViewModel = new GraphViewModel(graph, 1200, 900);
            StyleGraph(graph);

            Quests = new ObservableCollection<QuestViewModel>();

            questStatusReadModel.Quests.ToList().ForEach(quest =>
            {
                Quests.Add(new QuestViewModel
                {
                    Title = quest
                });
            });

            CheatCompleteQuest_Command = new RelayCommand<string>(questId =>
            {
                //eventBus.Publish(new Quest);
                throw new NotImplementedException();
            });
        }

        private GraphAdjacencyList<LabelledVertex, EmptyEdge> GenerateGraph()
        {
            var vertices = new List<LabelledVertex>
            {
                new LabelledVertex("Quest 1"),
                new LabelledVertex("Quest 2"),
                new LabelledVertex("Quest 3")
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, directed:true);

            return graph;
        }

        private void StyleGraph(
            IGraph<LabelledVertex, EmptyEdge> graph)
        {
            GraphViewModel.PlacePoint(0, new PointD(200, 50));
            GraphViewModel.PlacePoint(1, new PointD(200, 100));
        }
    }
}
