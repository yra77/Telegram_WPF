

using Telegram_WPF.Models;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Telegram_WPF.Services.ListenMsg
{
    internal interface IValidateListenMsg
    {
        Task<bool> IsValidShouldBeMsg(string? ShouldBeText, List<ChatBase_Img?> SelectedGroupList, TL.Message msg);
        Task<bool> IsValidNotShouldBeMsg(string? NotShouldBeText, List<ChatBase_Img?> SelectedGroupList, TL.Message msg);
    }
}
