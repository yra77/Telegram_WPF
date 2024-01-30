

using Prism.Commands;
using Prism.Regions;
using System.Diagnostics;
using System.Windows.Controls;


namespace Telegram_WPF.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel, INavigationAware
    {

        public MainWindowViewModel(IRegionManager regionManager)
            : base()
        {
            _regionManager = regionManager;

            TelLogin("+380688815592");
        }
       

        private async void TelLogin(string? str)
        {
            var what = await _client.Login(str);

            if (what != null)
            {
              //  Debug.WriteLine("PPPPPPPPPPPPPPPP login Ok " + what);
                _regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.Auth));
            }
            else
            {
               // Debug.WriteLine("EEEEEEEEEEEEEEEE Error " + what);
                _regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.Home));
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }
    }
}
