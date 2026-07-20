using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Craft.Math;
using Temple.Application.Core;
using Temple.Application.State.Payloads;
using Temple.Application.Interfaces.Readers;

namespace Temple.ViewModel.DD.Wilderness
{
    public class WildernessViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;

        private SiteListBoxItemViewModel _selectedSite;

        public ObservableCollection<SiteListBoxItemViewModel> Sites { get; } = new();

        public SiteListBoxItemViewModel SelectedSite
        {
            get => _selectedSite;
            set
            {
                _selectedSite = value;
                RaisePropertyChanged();

                if (_selectedSite.Text == "maze")
                {
                    // Test site
                    _controller.ApplicationData.ExplorationPosition = new Vector2D(1.5, 0.5);
                    _controller.ApplicationData.ExplorationOrientation = 90;
                }

                _controller.GoToNextApplicationState(new ExplorationPayload
                {
                    SiteId = _selectedSite.Text
                });
            }
        }

        public RelayCommand GoToInGameMenu_Command { get; }

        public WildernessViewModel(
            ApplicationController controller,
            ISitesUnlockedReader sitesUnlockedReader)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            sitesUnlockedReader.SitesUnlocked.ToList().ForEach(siteId =>
            {
                Sites.Add(new SiteListBoxItemViewModel
                {
                    Text = siteId
                });
            });

            // An extra site for testing
            Sites.Add(new SiteListBoxItemViewModel
            {
                Text = "maze"
            });

            GoToInGameMenu_Command = new RelayCommand(() =>
            {
                var payload = new InGameMenuPayload
                {
                    PayloadForNextState = new WildernessPayload()
                };

                _controller.GoToNextApplicationState(payload);
            });
        }
    }
}
