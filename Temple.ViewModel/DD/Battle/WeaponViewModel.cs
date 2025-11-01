using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD.Battle
{
    public class WeaponViewModel : BoardItemViewModel
    {
        private double _baseRotationAngle;
        private double _rotationAngle;

        public double BaseRotationAngle
        {
            get { return _baseRotationAngle; }
            set
            {
                _baseRotationAngle = value;
                RaisePropertyChanged();
            }
        }

        public double RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = _baseRotationAngle + value;
                RaisePropertyChanged();
            }
        }

        public void Initialize(
            double left,
            double top)
        {
            Left = left;
            Top = top;
        }

        public WeaponViewModel(
            Weapon weapon,
            double left,
            double top,
            double diameter) : base(left, top, diameter)
        {
            ImagePath = "Images/Arrow.png";
        }
    }
}
