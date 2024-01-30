

using System;
using TL;


namespace Telegram_WPF.Models
{
    internal class ChatBase_Img
    {
        public ChatBase? Group { get; set; }
        public byte[]? Img { get; set; } = null;

        public long GroupId { get; set; }
        public string? GroupType { get; set; }
        public DateTime DataCreated { get; set; }
        public int CountParticipants { get; set; }
        public string? MainUserName { get; set; }
        public string? Title { get; set; }
        public byte[]? ImgGroup { get; set; } = null;
    }
}
