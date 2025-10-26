using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD
{
    public class CreatureViewModel : BoardItemViewModel
    {
        private bool _isHostile;
        private double _hitPointsLeftExtent;
        private bool _isInjured;

        public bool IsHostile
        {
            get { return _isHostile; }
            set
            {
                _isHostile = value;
                RaisePropertyChanged();
            }
        }

        public double HitPointsLeftExtent
        {
            get { return _hitPointsLeftExtent; }
            set
            {
                _hitPointsLeftExtent = value;
                RaisePropertyChanged();
            }
        }

        public bool IsInjured
        {
            get { return _isInjured; }
            set
            {
                _isInjured = value;
                RaisePropertyChanged();
            }
        }

        public void Initialize(
            Creature creature,
            double left,
            double top,
            double diameter)
        {
            IsHostile = creature.IsHostile;
            IsInjured = creature.HitPoints < creature.CreatureType.MaxHitPoints;
            HitPointsLeftExtent = creature.HitPoints * diameter / creature.CreatureType.MaxHitPoints;
            ImagePath = GetImagePath(creature.CreatureType.Name);
            Left = left;
            Top = top;
        }

        public CreatureViewModel(
            double diameter) : base(0, 0, diameter)
        {
            IsVisible = false;
        }

        public CreatureViewModel(
            Creature creature,
            double left,
            double top,
            double diameter) : base(left, top, diameter)
        {
            IsVisible = true;
            IsHostile = creature.IsHostile;
            IsInjured = creature.HitPoints < creature.CreatureType.MaxHitPoints;
            HitPointsLeftExtent = creature.HitPoints * Diameter / creature.CreatureType.MaxHitPoints;
            ImagePath = GetImagePath(creature.CreatureType.Name);
        }
    }
}
