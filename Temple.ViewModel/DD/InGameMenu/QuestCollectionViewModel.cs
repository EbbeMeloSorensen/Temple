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
        private readonly Brush _brush = new SolidColorBrush(Colors.CornflowerBlue);

        public GraphViewModel GraphViewModel { get; }

        public QuestCollectionViewModel(
            IQuestTree questTree)
        {
            var graph = GenerateGraph(questTree);

            GraphViewModel = new GraphViewModel(graph, 1200, 900);
            StyleGraph(graph);
        }

        private GraphAdjacencyList<LabelledVertex, EmptyEdge> GenerateGraph(
            IQuestTree questTree)
        {
            var vertices = questTree.GetAllQuests().Select(quest => new LabelledVertex(quest.Title));

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, directed:true);

            return graph;
        }

        private void StyleGraph(
            IGraph<LabelledVertex, EmptyEdge> graph)
        {
            GraphViewModel.PlacePoint(0, new PointD(200, 50));
            GraphViewModel.PlacePoint(1, new PointD(200, 100));
            GraphViewModel.PlacePoint(2, new PointD(400, 50));
            GraphViewModel.PlacePoint(3, new PointD(400, 100));
            GraphViewModel.PlacePoint(4, new PointD(400, 150));
            GraphViewModel.PlacePoint(5, new PointD(400, 200));

            graph.Vertices.ForEach(v => GraphViewModel.StylePoint(v.Id, _brush, v.Label));
        }
    }
}
