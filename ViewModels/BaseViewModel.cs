

using Telegram_WPF.Models;
using Telegram_WPF.Services.ListenMsg;
using Telegram_WPF.Services.FilesService;

using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using Prism.Events;

using System.Collections.Generic;


namespace Telegram_WPF.ViewModels
{
    internal class BaseViewModel : BindableBase
    {

        protected static WTelegram.Client _client;
        protected static List<ChatBase_Img> _listGroup_static;

        // protected IEventAggregator? _eventAggregator;
        protected IValidateListenMsg _validateListenMsg;
        protected IDialogService _dialogService;
        protected IRegionManager _regionManager;
        protected IListenGroups _listenGroups;
        protected IFilesWork _filesWork;

        protected bool _isPressed;


        static BaseViewModel()
        {
            _client = new WTelegram.Client(121500, "e430448d339179368eec2653bfac411e");
        }

        public BaseViewModel()
        {
            _isPressed = false;
        }
    }
}
