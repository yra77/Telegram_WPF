

using Prism.Commands;
using Prism.Regions;
using System.Diagnostics;


namespace Telegram_WPF.ViewModels
{
    internal class AuthViewModel : BaseViewModel, INavigationAware
    {

        public AuthViewModel(IRegionManager regionManager)
            : base()
        {
            _regionManager = regionManager;

            IsValidInput = false;
            Code = "";
        }


        #region property

        private string? _code;
        public string? Code
        {
            get => _code;
            set => SetProperty(ref _code, value);
        }


        private bool _isValidInput;
        public bool IsValidInput
        {
            get => _isValidInput;
            set => SetProperty(ref _isValidInput, value);
        }


        public DelegateCommand CodeOk_Btn => new DelegateCommand(CodeOk);

        #endregion


        private void CodeOk()
        {
            if (IsValidInput && !_isPressed)
            {
                _isPressed = true;
                TelLogin(Code);
            }
        }

        private async void TelLogin(string? str)
        {
            var what = await _client.Login(str);

            if (what != null)
            {
                Debug.WriteLine("PPPPPPPPPPPPPPPP login Ok " + what);
               // _regionManager.RegisterViewWithRegion("MainRegion", typeof(Views.Auth));
            }
            else
            {
                Debug.WriteLine("EEEEEEEEEEEEEEEE Error " + what);
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
