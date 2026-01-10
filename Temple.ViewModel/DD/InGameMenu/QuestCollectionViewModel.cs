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
        private readonly Brush _northAmericaBrush = new SolidColorBrush(Colors.Yellow);
        private readonly Brush _southAmericaBrush = new SolidColorBrush(Colors.DarkOrange);
        private readonly Brush _europeBrush = new SolidColorBrush(Colors.CornflowerBlue);
        private readonly Brush _africaBrush = new SolidColorBrush(Colors.Red);
        private readonly Brush _asiaBrush = new SolidColorBrush(Colors.MediumSeaGreen);
        private readonly Brush _oceaniaBrush = new SolidColorBrush(Colors.MediumPurple);

        public GraphViewModel GraphViewModel { get; }

        public QuestCollectionViewModel(
            IQuestTree questTree)
        {
            var graph = GenerateGraph();

            GraphViewModel = new GraphViewModel(graph, 1200, 900);
            StyleGraph();
        }

        private GraphAdjacencyList<LabelledVertex, EmptyEdge> GenerateGraph()
        {
            var vertices = new List<LabelledVertex>
            {
                // North America
                new LabelledVertex("Alaska"),                //  0
                new LabelledVertex("Northwest Territory"),   //  1
                new LabelledVertex("Greenland"),             //  2
                new LabelledVertex("Alberta"),               //  3
                new LabelledVertex("Ontario"),               //  4
                new LabelledVertex("Quebec"),                //  5
                new LabelledVertex("Western United States"), //  6
                new LabelledVertex("Eastern United States"), //  7
                new LabelledVertex("Central America"),       //  8

                // South America
                new LabelledVertex("Venezuela"),   //  9
                new LabelledVertex("Peru"),        // 10
                new LabelledVertex("Argentina"),   // 11
                new LabelledVertex("Brazil"),      // 12

                // Europe
                new LabelledVertex("Iceland"),         // 13
                new LabelledVertex("Scandinavia"),     // 14
                new LabelledVertex("Great Britain"),   // 15
                new LabelledVertex("Northern Europe"), // 16
                new LabelledVertex("Ukraine"),         // 17
                new LabelledVertex("Western Europe"),  // 18
                new LabelledVertex("Southern Europe"), // 19

                // Africa
                new LabelledVertex("North Africa"), // 20
                new LabelledVertex("Egypt"),        // 21
                new LabelledVertex("East Africa"),  // 22
                new LabelledVertex("Congo"),        // 23
                new LabelledVertex("South Africa"), // 24
                new LabelledVertex("Madagascar"),   // 25

                // Asia
                new LabelledVertex("Siberia"),     // 26
                new LabelledVertex("Ural"),        // 27
                new LabelledVertex("Yakutsk"),     // 28
                new LabelledVertex("Kamchatka"),   // 29
                new LabelledVertex("Irkutsk"),     // 30
                new LabelledVertex("Afghanistan"), // 31
                new LabelledVertex("Mongolia"),    // 32
                new LabelledVertex("Japan"),       // 33
                new LabelledVertex("China"),       // 34
                new LabelledVertex("Middle East"), // 35
                new LabelledVertex("India"),       // 36
                new LabelledVertex("Siam"),        // 37

                // Oceania
                new LabelledVertex("Indonesia"),         // 38
                new LabelledVertex("New Guinea"),        // 39
                new LabelledVertex("Western Australia"), // 40
                new LabelledVertex("Eastern Australia"), // 41
            };

            var graph = new GraphAdjacencyList<LabelledVertex, EmptyEdge>(vertices, false);

            graph.AddEdge(0, 1);
            graph.AddEdge(0, 3);
            graph.AddEdge(0, 29);
            graph.AddEdge(1, 2);
            graph.AddEdge(1, 3);
            graph.AddEdge(1, 4);
            graph.AddEdge(2, 4);
            graph.AddEdge(2, 5);
            graph.AddEdge(2, 13);
            graph.AddEdge(3, 4);
            graph.AddEdge(3, 6);
            graph.AddEdge(4, 5);
            graph.AddEdge(4, 6);
            graph.AddEdge(4, 7);
            graph.AddEdge(5, 7);
            graph.AddEdge(6, 7);
            graph.AddEdge(6, 8);
            graph.AddEdge(7, 8);
            graph.AddEdge(8, 9);
            graph.AddEdge(9, 10);
            graph.AddEdge(9, 12);
            graph.AddEdge(10, 11);
            graph.AddEdge(10, 12);
            graph.AddEdge(11, 12);
            graph.AddEdge(12, 20);
            graph.AddEdge(13, 14);
            graph.AddEdge(13, 15);
            graph.AddEdge(14, 15);
            graph.AddEdge(14, 16);
            graph.AddEdge(14, 17);
            graph.AddEdge(15, 16);
            graph.AddEdge(15, 18);
            graph.AddEdge(16, 17);
            graph.AddEdge(16, 18);
            graph.AddEdge(16, 19);
            graph.AddEdge(17, 19);
            graph.AddEdge(18, 19);
            graph.AddEdge(18, 20);
            graph.AddEdge(19, 20);
            graph.AddEdge(19, 21);
            graph.AddEdge(19, 35);
            graph.AddEdge(17, 27);
            graph.AddEdge(17, 31);
            graph.AddEdge(17, 35);
            graph.AddEdge(20, 21);
            graph.AddEdge(20, 22);
            graph.AddEdge(20, 23);
            graph.AddEdge(21, 22);
            graph.AddEdge(21, 35);
            graph.AddEdge(22, 23);
            graph.AddEdge(22, 24);
            graph.AddEdge(22, 25);
            graph.AddEdge(22, 35);
            graph.AddEdge(23, 24);
            graph.AddEdge(24, 25);
            graph.AddEdge(26, 27);
            graph.AddEdge(26, 28);
            graph.AddEdge(26, 30);
            graph.AddEdge(26, 32);
            graph.AddEdge(26, 34);
            graph.AddEdge(27, 31);
            graph.AddEdge(27, 34);
            graph.AddEdge(28, 29);
            graph.AddEdge(28, 30);
            graph.AddEdge(29, 30);
            graph.AddEdge(29, 32);
            graph.AddEdge(29, 33);
            graph.AddEdge(30, 32);
            graph.AddEdge(31, 34);
            graph.AddEdge(31, 35);
            graph.AddEdge(31, 36);
            graph.AddEdge(32, 33);
            graph.AddEdge(32, 34);
            graph.AddEdge(34, 36);
            graph.AddEdge(34, 37);
            graph.AddEdge(35, 36);
            graph.AddEdge(36, 37);
            graph.AddEdge(37, 38);
            graph.AddEdge(38, 39);
            graph.AddEdge(38, 40);
            graph.AddEdge(39, 40);
            graph.AddEdge(39, 41);
            graph.AddEdge(40, 41);

            return graph;
        }

        private void StyleGraph()
        {
            // North America
            GraphViewModel.PlacePoint(0, new PointD(50, 50));
            GraphViewModel.PlacePoint(1, new PointD(150, 100));
            GraphViewModel.PlacePoint(2, new PointD(300, 100));
            GraphViewModel.PlacePoint(3, new PointD(100, 150));
            GraphViewModel.PlacePoint(4, new PointD(200, 150));
            GraphViewModel.PlacePoint(5, new PointD(300, 150));
            GraphViewModel.PlacePoint(6, new PointD(100, 200));
            GraphViewModel.PlacePoint(7, new PointD(250, 200));
            GraphViewModel.PlacePoint(8, new PointD(175, 250));

            // South America
            GraphViewModel.PlacePoint(9, new PointD(175, 350));
            GraphViewModel.PlacePoint(10, new PointD(125, 400));
            GraphViewModel.PlacePoint(11, new PointD(175, 450));
            GraphViewModel.PlacePoint(12, new PointD(225, 400));

            // Europe
            GraphViewModel.PlacePoint(13, new PointD(400, 100));
            GraphViewModel.PlacePoint(14, new PointD(500, 100));
            GraphViewModel.PlacePoint(15, new PointD(450, 150));
            GraphViewModel.PlacePoint(16, new PointD(500, 200));
            GraphViewModel.PlacePoint(17, new PointD(550, 150));
            GraphViewModel.PlacePoint(18, new PointD(450, 250));
            GraphViewModel.PlacePoint(19, new PointD(550, 250));

            // Africa
            GraphViewModel.PlacePoint(20, new PointD(450, 350));
            GraphViewModel.PlacePoint(21, new PointD(550, 350));
            GraphViewModel.PlacePoint(22, new PointD(600, 400));
            GraphViewModel.PlacePoint(23, new PointD(500, 400));
            GraphViewModel.PlacePoint(24, new PointD(500, 450));
            GraphViewModel.PlacePoint(25, new PointD(600, 450));

            // Asia
            GraphViewModel.PlacePoint(26, new PointD(825, 150));
            GraphViewModel.PlacePoint(27, new PointD(725, 150));
            GraphViewModel.PlacePoint(28, new PointD(875, 100));
            GraphViewModel.PlacePoint(29, new PointD(1025, 50));
            GraphViewModel.PlacePoint(30, new PointD(925, 150));
            GraphViewModel.PlacePoint(31, new PointD(725, 225));
            GraphViewModel.PlacePoint(32, new PointD(925, 225));
            GraphViewModel.PlacePoint(33, new PointD(1025, 225));
            GraphViewModel.PlacePoint(34, new PointD(825, 225));
            GraphViewModel.PlacePoint(35, new PointD(675, 300));
            GraphViewModel.PlacePoint(36, new PointD(775, 300));
            GraphViewModel.PlacePoint(37, new PointD(875, 300));

            // Oceania
            GraphViewModel.PlacePoint(38, new PointD(875, 400));
            GraphViewModel.PlacePoint(39, new PointD(975, 400));
            GraphViewModel.PlacePoint(40, new PointD(875, 450));
            GraphViewModel.PlacePoint(41, new PointD(975, 450));

            Enumerable
                .Range(0, 9)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _northAmericaBrush, ""));

            Enumerable
                .Range(9, 4)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _southAmericaBrush, ""));

            Enumerable
                .Range(13, 7)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _europeBrush, ""));

            Enumerable
                .Range(20, 6)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _africaBrush, ""));

            Enumerable
                .Range(26, 12)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _asiaBrush, ""));

            Enumerable
                .Range(38, 4)
                .ToList()
                .ForEach(index => GraphViewModel.StylePoint(index, _oceaniaBrush, ""));
        }
    }
}
