

using TL;
using WTelegram;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System;
using Telegram_WPF.ViewModels;


namespace Telegram_WPF.Services.ListenMsg
{
    internal class ListenGroups : IListenGroups
    {


        private Dictionary<long, User> Users = new();
        private Dictionary<long, ChatBase> Chats = new();


        public async Task Client_OnUpdate(UpdatesBase updates, Client client, ViewModels.MessageCallback? messageCallback)
        {

            updates.CollectUsersChats(Users, Chats);
            //if (updates is UpdateShortMessage usm && !Users.ContainsKey(usm.user_id))
            //    (await client.Updates_GetDifference(usm.pts - usm.pts_count, usm.date, 0)).CollectUsersChats(Users, Chats);
            //else if (updates is UpdateShortChatMessage uscm && (!Users.ContainsKey(uscm.from_id) || !Chats.ContainsKey(uscm.chat_id)))
            //    (await client.Updates_GetDifference(uscm.pts - uscm.pts_count, uscm.date, 0)).CollectUsersChats(Users, Chats);
            foreach (var update in updates.UpdateList)
                switch (update)
                {
                    case UpdateNewMessage unm:
                        //    await System.Windows.Application.Current.MainWindow.Dispatcher.BeginInvoke(new Action(() =>
                        //   {
                        if (unm.message is MessageBase mb)
                        {
                            messageCallback(mb as Message);
                        }
                  //  }));
                         break;

                    case UpdateEditMessage uem: await HandleMessage(uem.message, true); break;
                    case UpdateChannelMessageViews messageViews: Debug.WriteLine("Mess views " + messageViews.channel_id + " " + messageViews.views);break;
                    // Note: UpdateNewChannelMessage and UpdateEditChannelMessage are also handled by above cases
                    case UpdateDeleteChannelMessages udcm: Debug.WriteLine($"PPPPPPPPPP {udcm.messages.Length} message(s) deleted in {Chat(udcm.channel_id)}"); break;
                    case UpdateDeleteMessages udm: Debug.WriteLine($"AAAAAAAAAA {udm.messages.Length} message(s) deleted"); break;
                    //case UpdateUserTyping uut: Debug.WriteLine($"SSSSSSSSSS {User(uut.user_id)} is {uut.action}"); break;
                    case UpdateChatUserTyping ucut: Debug.WriteLine($"ZZZZZZZZZZZ {Peer(ucut.from_id)} is {ucut.action} in {Chat(ucut.chat_id)}"); break;
                    case UpdateChannelUserTyping ucut2: Debug.WriteLine($"XXXXXXXXXxx {Peer(ucut2.from_id)} is {ucut2.action} in {Chat(ucut2.channel_id)}"); break;
                    case UpdateChatParticipants { participants: ChatParticipants cp }: Debug.WriteLine($"VVVVVVVVV {cp.participants.Length} participants in {Chat(cp.chat_id)}"); break;
                    case UpdateMessagePoll uus: {  Debug.WriteLine($"poll id {uus.poll_id} questions " + (Poll)uus.poll); } break;
                    case UpdateUserName uun: Debug.WriteLine($"DDDDDDDDDDD {User(uun.user_id)} has changed profile name: {uun.first_name} {uun.last_name}"); break;
                   // case UpdateUser uu: Debug.WriteLine($"FFFFFFFFFFF {User(uu.user_id)} has changed infos/photo"); break;
                    default: Debug.WriteLine("IIIIIIIIIIIIIII " + update.GetType().Name); break; // there are much more update types than the above example cases
                }
        }

        // in this example method, we're not using async/await, so we just return Task.CompletedTask
        private Task HandleMessage(MessageBase messageBase, bool edit = false)
        {
            if (!edit) //Debug.Write("(Edit): ");
            switch (messageBase)
            {
                case Message m: Debug.WriteLine($"TTTTTTTTTTT {m.Peer.ID} in {Peer(m.peer_id)}> {m.message}"); break;
                case MessageService ms: Debug.WriteLine($"UUUUUUUUUUUUU {Peer(ms.from_id)} in {Peer(ms.peer_id)} [{ms.action.GetType().Name[13..]}]"); break;
            }
            return Task.CompletedTask;
        }

        private string User(long id) => Users.TryGetValue(id, out var user) ? user.ToString() : $"User {id}";
        private string Chat(long id) => Chats.TryGetValue(id, out var chat) ? chat.ToString() : $"Chat {id}";
        private string Peer(Peer peer) => peer is null ? null : peer is PeerUser user ? User(user.user_id)
            : peer is PeerChat or PeerChannel ? Chat(peer.ID) : $"Peer {peer.ID}";
   
    }

}
