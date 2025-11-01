using GalaSoft.MvvmLight;
using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD.Battle
{
    public class TeamStatsListItemViewModel : ViewModelBase
    {
        private int _count;

        public CreatureType CreatureType { get; set; }

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                RaisePropertyChanged();
            }
        }
    }
}
