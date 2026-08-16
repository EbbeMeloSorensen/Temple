using GalaSoft.MvvmLight;

namespace Temple.ViewModel.DD.Exploration;

public class DoorRotationViewModel : ViewModelBase
{
    public double _rotationAngle;

    public double RotationAngle
    {
        get => _rotationAngle;
        set
        {
            _rotationAngle = value;
            RaisePropertyChanged();
        }
    }
}
