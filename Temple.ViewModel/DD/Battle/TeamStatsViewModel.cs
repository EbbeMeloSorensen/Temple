using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.ViewModel.DD.Battle
{
    public class TeamStatsViewModel : ViewModelBase
    {
        private bool _isVisible;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TeamStatsListItemViewModel> Friendlies { get; set; }
        public ObservableCollection<TeamStatsListItemViewModel> Hostiles { get; set; }

        public TeamStatsViewModel()
        {
            Friendlies = new ObservableCollection<TeamStatsListItemViewModel>();

            Hostiles = new ObservableCollection<TeamStatsListItemViewModel>();
        }

        public void Initialize(
            IEnumerable<Creature> creatures)
        {
            Clear();

            var friendlyCreatureCountDictionary = new Dictionary<CreatureType, int>();
            var hostileCreatureCountDictionary = new Dictionary<CreatureType, int>();

            creatures.ToList().ForEach(c =>
            {
                AppendToDictionary(
                    c.IsHostile
                        ? hostileCreatureCountDictionary
                        : friendlyCreatureCountDictionary,
                    c);
            });

            foreach (var kvp in friendlyCreatureCountDictionary)
            {
                Friendlies.Add(new TeamStatsListItemViewModel
                {
                    CreatureType = kvp.Key,
                    Count = kvp.Value
                });
            }

            foreach (var kvp in hostileCreatureCountDictionary)
            {
                Hostiles.Add(new TeamStatsListItemViewModel
                {
                    CreatureType = kvp.Key,
                    Count = kvp.Value
                });
            }

            IsVisible = true;
        }

        public void Update(
            IEnumerable<Creature> creatures)
        {
            var friendlyCreatureCountDictionary = new Dictionary<CreatureType, int>();
            var hostileCreatureCountDictionary = new Dictionary<CreatureType, int>();

            creatures.ToList().ForEach(c =>
            {
                AppendToDictionary(
                    c.IsHostile
                        ? hostileCreatureCountDictionary
                        : friendlyCreatureCountDictionary,
                    c);
            });

            foreach (var teamStatsListItemViewModel in Friendlies)
            {
                teamStatsListItemViewModel.Count = friendlyCreatureCountDictionary.TryGetValue(
                    teamStatsListItemViewModel.CreatureType, out var value)
                        ? value
                        : 0;
            }

            foreach (var teamStatsListItemViewModel in Hostiles)
            {
                teamStatsListItemViewModel.Count = hostileCreatureCountDictionary.TryGetValue(
                    teamStatsListItemViewModel.CreatureType, out var value)
                    ? value
                    : 0;
            }
        }

        public void Clear()
        {
            IsVisible = false;
            Friendlies.Clear();
            Hostiles.Clear();
        }

        private void AppendToDictionary(
            IDictionary<CreatureType, int> dictionary,
            Creature creature)
        {
            if (!dictionary.ContainsKey(creature.CreatureType))
            {
                dictionary[creature.CreatureType] = 0;
            }

            dictionary[creature.CreatureType]++;
        }
    }
}
