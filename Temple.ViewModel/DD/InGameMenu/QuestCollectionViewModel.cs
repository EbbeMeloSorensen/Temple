using Craft.DataStructures.Graph;
using Craft.Utils;
using Craft.ViewModels.Graph;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Infrastructure.Dialogues;

namespace Temple.ViewModel.DD.InGameMenu
{
    public class QuestCollectionViewModel : ViewModelBase
    {
        private IQuestStatusReadModel _questStatusReadModel;

        private readonly Brush _unavailableQuestBrush = new SolidColorBrush(Colors.IndianRed);
        private readonly Brush _availableQuestBrush = new SolidColorBrush(Colors.Orange);
        private readonly Brush _startedQuestBrush = new SolidColorBrush(Colors.DarkGreen);
        private readonly Brush _completedQuestBrush = new SolidColorBrush(Colors.White);
        private readonly Brush _failedQuestBrush = new SolidColorBrush(Colors.Black);

        public GraphViewModel GraphViewModel { get; }

        public ObservableCollection<QuestViewModel> Quests { get; }

        public RelayCommand<string> CheatCompleteQuest_Command { get; }

        public QuestCollectionViewModel(
            IQuestStatusReadModel questStatusReadModel,
            QuestEventBus eventBus)
        {
            _questStatusReadModel = questStatusReadModel ?? throw new ArgumentNullException(nameof(questStatusReadModel));

            var graph = GenerateGraph();

            GraphViewModel = new GraphViewModel(graph, 1200, 900);
            StyleGraph(graph);

            Quests = new ObservableCollection<QuestViewModel>();

            _questStatusReadModel.Quests.ToList().ForEach(quest =>
            {
                Quests.Add(new QuestViewModel
                {
                    Title = quest
                });
            });

            _questStatusReadModel.QuestStatusChanged += HandleQuestStatusChanged;

            CheatCompleteQuest_Command = new RelayCommand<string>(_ =>
            {
                throw new NotImplementedException();
            });
        }

        private void HandleQuestStatusChanged(
            object? sender,
            QuestStatusChangedEventArgs e)
        {
            throw new NotImplementedException();
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

        public override void Cleanup()
        {
            // Unsubscribe to prevent memory leaks
            _questStatusReadModel.QuestStatusChanged -= HandleQuestStatusChanged;
            base.Cleanup();
        }
    }
}
