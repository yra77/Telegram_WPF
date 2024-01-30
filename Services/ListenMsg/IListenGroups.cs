

using System.Collections.Generic;
using System.Threading.Tasks;
using WTelegram;
using TL;


namespace Telegram_WPF.Services.ListenMsg
{
    internal interface IListenGroups
    {
        Task Client_OnUpdate(UpdatesBase updates, Client client, ViewModels.MessageCallback? messageCallback);
    }
}
