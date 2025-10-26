using Craft.ViewModels.Geometry2D.Scrolling;
using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD
{
    public abstract class BoardViewModelBase : ImageEditorViewModel
    {
        private static Dictionary<string, double> _weaponImageBaseRotationAngleMap = new Dictionary<string, double>
        {
            { "Images/Arrow.png", 162.5 },
            { "Images/Sword.png", -56 },
        };

        protected static double _creatureDiameter;
        protected static double _weaponDiameter;

        protected Scene _scene;
        private int _rows;
        private int _columns;
        private double _squareForCurrentCreatureLeft;
        private double _squareForCurrentCreatureTop;
        private double _squareForCurrentCreatureWidth;

    }
}
