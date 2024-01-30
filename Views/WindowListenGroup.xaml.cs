

using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Telegram_WPF.Views
{
    /// <summary>
    /// Логика взаимодействия для WindowListenGroup.xaml
    /// </summary>
    public partial class WindowListenGroup : UserControl, IDialogWindow
    {

        public IDialogResult Result { get; set; }
        public object Content { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Window Owner { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public object DataContext { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Style Style { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public WindowListenGroup()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler Loaded;
        public event EventHandler Closed;
        public event CancelEventHandler Closing;

        public void Close()
        {
            
        }

        public void Show()
        {
           
        }

        public bool? ShowDialog()
        {
            return false;
        }
    }
}
