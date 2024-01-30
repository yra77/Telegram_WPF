

using Microsoft.Win32;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Telegram_WPF.ViewModels
{
    internal class CreateMessageViewModel : BaseViewModel, IDialogAware
    {


        public event Action<IDialogResult>? RequestClose;

        private List<string> _filePathList;


        public CreateMessageViewModel() : base()
        {
            _filePathList = new List<string>();
            SizeList = new List<string>() { "8", "9", "10", "12", "14", "16", "16", "20", "22" };
        }


        #region property

        public string? Title => "Create message";


        private string _header;
        public string Header 
        { 
            get => _header; 
            set => SetProperty(ref _header, value); 
        }


        private string _msgText;
        public string MsgText
        {
            get => _msgText;
            set => SetProperty(ref _msgText, value);
        }


        private List<string> _sizeList;
        public List<string> SizeList
        {
            get => _sizeList;
            set => SetProperty(ref _sizeList, value);
        }


        public DelegateCommand OpenFileDialog_Btn => new DelegateCommand(Open_FileDialog);


        #endregion


        private void Open_FileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|Video files (*.mp4)|*.mp4*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                    _filePathList.Add(filename);
            }
        }




        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
        }
    }
}
