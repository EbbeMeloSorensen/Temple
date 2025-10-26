using System.Collections.ObjectModel;
using Craft.Utils;
using Craft.ViewModels.Common;
using Temple.Domain.Entities.DD;
using Temple.ViewModel.DD.BusinessLogic;

namespace Temple.ViewModel.DD
{
    public class BoardViewModel : BoardViewModelBase
    {
        private List<PixelViewModel> _pixelViewModels;

        public List<PixelViewModel> PixelViewModels
        {
            get { return _pixelViewModels; }
            set
            {
                _pixelViewModels = value;
                RaisePropertyChanged();
            }
        }

        public BoardViewModel(
            IEngine engine,
            double tileCenterSpacing,
            double obstacleDiameter,
            double creatureDiameter,
            double weaponDiameter,
            ObservableObject<Scene> selectedScene) : base(
            engine,
            tileCenterSpacing,
            obstacleDiameter,
            creatureDiameter,
            weaponDiameter,
            selectedScene)
        {
        }

        public override void LayoutBoard(
            Scene scene)
        {
            _scene = scene;

            if (scene == null)
            {
                Rows = 0;
                Columns = 0;
                BoardWidth = 0;
                BoardHeight = 0;
                ImageWidth = 0;
                ImageHeight = 0;
                ScrollableOffset = new PointD(0, 0);
                ScrollOffset = new PointD(0, 0);

                PixelViewModels = new List<PixelViewModel>();
            }
            else
            {
                Rows = scene.Rows;
                Columns = scene.Columns;
                BoardWidth = Columns * TileCenterSpacing;
                BoardHeight = Rows * TileCenterSpacing;
                ImageWidth = BoardWidth;
                ImageHeight = BoardHeight;

                PixelViewModels = Enumerable.Range(0, Rows * Columns)
                    .Select(i => new PixelViewModel(i, new Pixel(200, 200, 200, 0)))
                    .ToList();

                ApplyTileTextures(scene);
            }

            ObstacleViewModels = new ObservableCollection<ObstacleViewModel>(
                _scene.Obstacles.Select(o =>
                {
                    var left = (o.PositionX + 0.5) * TileCenterSpacing - _obstacleDiameter / 2;
                    var top = (o.PositionY + 0.5) * TileCenterSpacing - _obstacleDiameter / 2;
                    return new ObstacleViewModel(o, left, top, _obstacleDiameter);
                }));
        }

        public override void DetermineCanvasPosition(
            int positionX,
            int positionY,
            out double x,
            out double y)
        {
            x = (positionX + 0.5) * TileCenterSpacing;
            y = (positionY + 0.5) * TileCenterSpacing;
        }

        public override void HighlightPlayerOptions(
            int squareIndexOfCurrentCreature,
            HashSet<int> squareIndexesCurrentCreatureCanMoveTo,
            HashSet<int> squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
            HashSet<int> squareIndexesCurrentCreatureCanAttackWithRangedWeapon)
        {
            // Måske en kende overkill at lave nye Pixels her
            PixelViewModels = Enumerable.Range(0, Rows * Columns)
                .Select(index => GeneratePixel(
                    index,
                    squareIndexOfCurrentCreature,
                    squareIndexesCurrentCreatureCanMoveTo,
                    squareIndexesCurrentCreatureCanAttackWithMeleeWeapon,
                    squareIndexesCurrentCreatureCanAttackWithRangedWeapon))
                .ToList();

            ApplyTileTextures(_scene);
        }

        public override void ClearPlayerOptions()
        {
            // Måske en kende overkill at lave nye Pixels her
            PixelViewModels = Enumerable.Range(0, Rows * Columns)
                .Select(index => new PixelViewModel(index, new Pixel(200, 200, 200, 0)))
                .ToList();

            ApplyTileTextures(_scene);
        }

        private void ApplyTileTextures(
            Scene scene)
        {
            scene.Obstacles.ForEach(_ =>
            {
                var tileIndex = _.PositionX + _.PositionY * scene.Columns;

                PixelViewModels[tileIndex].Pixel.ImagePath = _.ObstacleType switch
                {
                    ObstacleType.Wall => "DD/Images/Wall.jpg",
                    ObstacleType.Water => "DD/Images/Water.PNG",
                    _ => ""
                }; ;
            });
        }
    }
}
