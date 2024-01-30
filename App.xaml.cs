

using Telegram_WPF.Views;
using Telegram_WPF.ViewModels;
using Telegram_WPF.Views.Modals;
using Telegram_WPF.Services.ListenMsg;
using Telegram_WPF.Services.FilesService;

using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;

using System.Windows;


namespace Telegram_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var region = containerProvider.Resolve<IRegion>();
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("MainRegion", typeof(MainWindow));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(Home)); 
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            // register services
            containerRegistry.Register<IFilesWork, FilesWork>();
            containerRegistry.Register<IListenGroups, ListenGroups>();
            containerRegistry.Register<IValidateListenMsg, ValidateListenMsg>();

            // register pages
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
            containerRegistry.RegisterForNavigation<Home, HomeViewModel>();
            containerRegistry.RegisterForNavigation<WindowListenGroup, WindowListenGroupViewModel>();

            // register modal
            containerRegistry.RegisterDialog<Modal, ModalViewModel>();
            containerRegistry.RegisterDialog<CreateMessage, CreateMessageViewModel>();

            containerRegistry.RegisterDialogWindow<WindowListenGroup>(nameof(WindowListenGroup));
        }

        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }

    }
    }
