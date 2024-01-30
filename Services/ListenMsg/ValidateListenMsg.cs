

using Telegram_WPF.Models;

using TL;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace Telegram_WPF.Services.ListenMsg
{
    internal class ValidateListenMsg : IValidateListenMsg
    {


        static string[]? arrShouldBe;
        static string? oldShouldBeText;
        static string[]? arrNotShouldBe;
        static string? oldNotShouldBeText;


        public async Task<bool> IsValidShouldBeMsg(string? ShouldBeText, List<ChatBase_Img?> SelectedGroupList, Message msg)
        {
            if (SelectedGroupList.Count > 0)
            {
                bool res = SelectedGroupList.Exists(x => x.Group.ID == msg.peer_id.ID);
                if (!res)
                {
                    return false;
                }

                if (ShouldBeText != null && ShouldBeText.Length > 2)//search word
                {
                    if (arrShouldBe == null || oldShouldBeText == null || oldShouldBeText.Length != ShouldBeText.Length)
                    {
                        arrShouldBe = ShouldBeText.Split(new char[] { ' ', ',', '.', ';' });
                        oldShouldBeText = ShouldBeText;//сохраняем в статик для последующей проверки
                    }

                    foreach (string str in arrShouldBe)
                    {
                        if (msg.message.Contains(str, StringComparison.InvariantCultureIgnoreCase) == true)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                return true;
            }

            return false;
        }

        public async Task<bool> IsValidNotShouldBeMsg(string? NotShouldBeText, List<ChatBase_Img?> SelectedGroupList, Message msg)
        {
            if (SelectedGroupList.Count > 0)
            {
                bool res = SelectedGroupList.Exists(x => x.Group.ID == msg.peer_id.ID);
                if (!res)
                {
                    return false;
                }

                if (NotShouldBeText != null && NotShouldBeText.Length > 2)//search word
                {
                    if (arrShouldBe == null || oldNotShouldBeText == null || oldNotShouldBeText.Length != NotShouldBeText.Length)
                    {
                        arrNotShouldBe = NotShouldBeText.Split(new char[] { ' ', ',', '.', ';' });
                        oldNotShouldBeText = NotShouldBeText;//сохраняем в статик для последующей проверки
                    }

                    foreach (string str in arrNotShouldBe)
                    {
                        if (msg.message.Contains(str, StringComparison.InvariantCultureIgnoreCase) == true)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return true;
            }

            return false;
        }

    }
}
