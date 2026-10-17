using GalaSoft.MvvmLight;

namespace Temple.ViewModel.DD.Exploration;

public class DoorRotationViewModel : ViewModelBase
{
    private double _rotationAngle;

    public double RotationAngle
    {
        get => _rotationAngle;
        set
        {
            if (_rotationAngle.Equals(value))
            {
                return;
            }

            _rotationAngle = value;
            RaisePropertyChanged();
        }
    }
}
