

using Telegram_WPF.Models;

using Prism.Commands;
using Prism.Services.Dialogs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Telegram_WPF.ViewModels
{
    internal class ModalViewModel : BaseViewModel, IDialogAware
    {


        public event Action<IDialogResult> RequestClose;


        public ModalViewModel()
            :base()
        {
            IsVisbleGroupInfo = "Hidden";
            IsVisbleMediaMix = "Hidden";
        }


        #region property

        private string _isVisbleGroupInfo;
        public string IsVisbleGroupInfo 
        { 
            get => _isVisbleGroupInfo; 
            set => SetProperty(ref _isVisbleGroupInfo, value); 
        }


        private string _isVisbleMediaMix;
        public string IsVisbleMediaMix
        {
            get => _isVisbleMediaMix;
            set => SetProperty(ref _isVisbleMediaMix, value);
        }


        private ChatBase_Img? _groupInfo;
        public ChatBase_Img? GroupInfo
        {
            get => _groupInfo;
            set => SetProperty(ref _groupInfo, value);
        }


        private MessageModel _message;
        public MessageModel Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }


        private string _title = "Watch Media";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        public DelegateCommand<string> CloseDialogCommand => new DelegateCommand<string>(CloseDialog);

        #endregion


        protected virtual void CloseDialog(string parameter)
        {
            ButtonResult result = ButtonResult.None;

            if (parameter?.ToLower() == "true")
                result = ButtonResult.OK;
            else if (parameter?.ToLower() == "false")
                result = ButtonResult.Cancel;

            RaiseRequestClose(new DialogResult(result));
        }

        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose?.Invoke(dialogResult);
        }

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            IsVisbleGroupInfo = "Hidden";
            IsVisbleMediaMix = "Hidden";
            Message = parameters.GetValue<MessageModel>("message");
            if(Message != null)
            {
                IsVisbleMediaMix = "Visible";
            }
            GroupInfo = parameters.GetValue<ChatBase_Img>("groupinfo");
            if (GroupInfo != null)
            {
                IsVisbleGroupInfo = "Visible";
            }
        }
    }
}
