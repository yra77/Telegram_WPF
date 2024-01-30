

using System;
using System.Collections.ObjectModel;
using System.IO;


namespace Telegram_WPF.Models
{
    internal class MessageModel
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime? Date { get; set; }
        public string? Author { get; set; }
        public ObservableCollection<byte[]?>? Imgs { get; set; } = new ObservableCollection<byte[]?>();
        public ObservableCollection<string?>? Videos { get; set; } = new ObservableCollection<string?>();
        public ObservableCollection<string?>? TextFormat { get; set; } = null;
        public PollModel? Poll { get; set; } = null;
    }
}
