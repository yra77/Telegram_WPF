

using Telegram_WPF.Models;
using Telegram_WPF.Services.ListenMsg;
using Telegram_WPF.Services.FilesService;

using Prism.Regions;
using Prism.Commands;
using Prism.Services.Dialogs;

using TL;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Channels;
using WTelegram;
using System.Windows.Controls.DataVisualization.Charting;
using Telegram_WPF.Views;
using System.Windows.Threading;

namespace Telegram_WPF.ViewModels
{
    internal class HomeViewModel : BaseViewModel, INavigationAware
    {

        private byte[] _imgNoPhoto;


        public HomeViewModel(IFilesWork filesWork,
                             IDialogService dialogService)
            : base()
        {
            _filesWork = filesWork;
            _dialogService = dialogService;

            ListGroup = new List<ChatBase_Img>();
            ListMessage = new ObservableCollection<MessageModel>();
            GetGroups();
        }


        #region property

        private List<ChatBase_Img> _listGroup;
        public List<ChatBase_Img> ListGroup
        {
            get => _listGroup;
            set => SetProperty(ref _listGroup, value);
        }


        private ChatBase_Img? _selectedGroup;
        public ChatBase_Img? SelectedGroup
        {
            get => _selectedGroup;
            set => SetProperty(ref _selectedGroup, value);
        }


        private ObservableCollection<MessageModel> _listMessage;
        public ObservableCollection<MessageModel> ListMessage
        {
            get => _listMessage;
            set => SetProperty(ref _listMessage, value);
        }


        private MessageModel? _selectedMessage;
        public MessageModel? SelectedMessage
        {
            get => _selectedMessage;
            set => SetProperty(ref _selectedMessage, value);
        }


        public DelegateCommand SettingsBtn => new DelegateCommand(Settings_Click);
        public DelegateCommand CreateMsgBtn => new DelegateCommand(Create_Message);
        public DelegateCommand ListenToGroups => new DelegateCommand(Listen_ToGroups);
        public DelegateCommand GroupInfo_Btn => new DelegateCommand(GroupInfo_Click);

        #endregion


        private void Listen_ToGroups()
        {
            // var dialogs = await _client.Messages_GetAllDialogs(); // dialogs = groups/channels/users
            //  dialogs.CollectUsersChats(Users, Chats);

            var dispatcher = Application.Current.MainWindow.Dispatcher;
            _ = Task.Run(() =>
            {
                //BeginInvoke делегирует работу диспетчеру, связанному с потоком пользовательского интерфейса.
                //    Invoke является синхронным, BeginInvoke — асинхронным
                dispatcher.BeginInvoke(new Action(() =>
                {
                    _dialogService.Show("WindowListenGroup");
                }));
            });
        }

        private void Create_Message()
        {
            _dialogService.ShowDialog("CreateMessage", new DialogParameters() { }, (r) => { });
        }

        private void Settings_Click()
        {

        }

        private async void GroupInfo_Click()
        {
            if (SelectedGroup != null)
            {
                MemoryStream byteStreamBig = new MemoryStream();
                await _client.DownloadProfilePhotoAsync(SelectedGroup.Group, byteStreamBig, true, false);
                SelectedGroup.ImgGroup = (byteStreamBig.Length == 0) ? _imgNoPhoto : byteStreamBig.ToArray();
                _dialogService.ShowDialog("Modal", new DialogParameters() { { "groupinfo", SelectedGroup } }, (r) => { });
            }
        }

        private async void GetGroups()
        {
            if (_client.User == null)
            {
                MessageBox.Show("You must login first.");
                return;
            }
            var chats = await _client.Messages_GetAllChats();

            _imgNoPhoto = await File.ReadAllBytesAsync("..\\..\\..\\..\\Images\\editphoto.png");

            foreach (var (id, chat) in chats.chats)
            {
                if (chat.IsActive)
                {
                    try
                    {
                        MemoryStream byteStream = new MemoryStream();
                        await _client.DownloadProfilePhotoAsync(chat, byteStream, false, true);

                        string groupType = "";
                        DateTime datecreate = DateTime.UtcNow;
                        int countPerticipans = 0;

                        switch (chat)
                        {
                            case Chat smallgroup when smallgroup.IsActive:
                                groupType = "Small group";
                                countPerticipans = smallgroup.participants_count;
                                datecreate = smallgroup.date;
                                break;

                            case TL.Channel channel when channel.IsChannel:
                                groupType = $"Channel, aditional name - ";
                                if (channel.ActiveUsernames != null)
                                    foreach (var item in channel.ActiveUsernames)
                                    {
                                        groupType += item + "; ";
                                    }
                                countPerticipans = channel.participants_count;
                                datecreate = channel.date;
                                break;

                            case TL.Channel group: // no broadcast flag => it's a big group, also called supergroup or megagroup
                                groupType = $"Big group, aditional name - ";
                                if (group.ActiveUsernames != null)
                                    foreach (var item in group.ActiveUsernames)
                                    {
                                        groupType += item + "; ";
                                    }
                                countPerticipans = group.participants_count;
                                datecreate = group.date;
                                break;
                        }

                        ListGroup.Add(new ChatBase_Img()
                        {
                            Group = chat,
                            Img = (byteStream.Length == 0) ? _imgNoPhoto : byteStream.ToArray(),
                            GroupId = chat.ID,
                            GroupType = groupType,
                            DataCreated = datecreate,
                            CountParticipants = countPerticipans,
                            MainUserName = chat.MainUsername ?? " ",
                            Title = chat.Title
                        });
                        RaisePropertyChanged("ListGroup");
                        byteStream.Dispose();
                    }
                    catch
                    { }
                }
            }
            RaisePropertyChanged("ListGroup");
            _listGroup_static = new List<ChatBase_Img>(ListGroup);
        }

        private async Task GetMessages(ChatBase? chat)
        {

            InputPeer peer = chat;
            long groupId = 0;

            var messagesBase = await _client.Messages_GetHistory(peer, 0, default, 0, 15, 0, 0, 0);
            if (messagesBase is not Messages_ChannelMessages channelMessages) return;

            MessageModel messageModel = new MessageModel();

            foreach (var msgBase in channelMessages.Messages)
            {
                if (msgBase is TL.Message msg)
                {
                    byte[]? tempByte = null;
                    string? tempPath = null;
                    PollModel? tempPoll = null;

                    if (msg.media is MessageMediaPoll { poll: Poll poll, results: PollResults pollresult })
                    {
                        tempPoll = GetPoll(poll, pollresult);
                    }

                    if (msg.media is MessageMediaPhoto { photo: Photo photo })
                    {
                        tempByte = await GetPhoto(photo);
                    }

                    if (msg.media is MessageMediaDocument { document: Document document })
                    {
                        if (document.mime_type.Split('/')[0] == "video") tempPath = await GetVideo(document);
                    }

                    if (groupId == 0 || groupId != msg.grouped_id)
                    {
                        groupId = msg.grouped_id;

                        ObservableCollection<string?>? textFormat = null;
                        if (msg.entities != null)
                        {
                            textFormat = new ObservableCollection<string?>();
                            foreach (var item in msg.entities)
                            {
                                textFormat.Add(item.Type + " " + item.Offset + " " + item.Length);
                               // Debug.WriteLine(item.Type + " " + item.Offset + " " + item.Length);
                            }
                           // Debug.WriteLine("--------------------------------");
                        }

                        messageModel = new MessageModel()
                        {
                            Message = msg.message,
                            Author = msg.post_author,
                            Date = msg.Date,
                            Id = msg.ID,
                            Imgs = new ObservableCollection<byte[]?>(),
                            Videos = new ObservableCollection<string?>(),
                            Poll = new PollModel(),
                            TextFormat = textFormat
                        };

                        if (tempByte != null) messageModel.Imgs.Add(tempByte);
                        if (tempPath != null) messageModel.Videos.Add(tempPath);
                        if (tempPoll != null) messageModel.Poll = tempPoll;

                        ListMessage.Add(messageModel);
                    }
                    else if (groupId == msg.grouped_id)
                    {
                        var index = ListMessage.IndexOf(messageModel);

                        messageModel.Message += msg.message;

                        if (tempByte != null) messageModel.Imgs.Add(tempByte);
                        if (tempPath != null) messageModel.Videos.Add(tempPath);

                        if (index > -1) ListMessage.RemoveAt(index);

                        ListMessage.Add(messageModel);
                    }
                }
            }
        }

        private PollModel GetPoll(Poll poll, PollResults pollresult)
        {

            PollModel pollModel = new PollModel()
            {
                Id = poll.id,
                PollQuestion = poll.question,
                PollSolution = pollresult.solution,
                PollTotalVoters = pollresult.total_voters,
                PollAnswers = new List<string?>(),
                PollResults = new List<int?>()
            };

            foreach (var item in poll.answers)
            {
                pollModel.PollAnswers?.Add(item.text);
            }

            if (pollresult.results != null)
                foreach (var item1 in pollresult.results)
                {
                    pollModel.PollResults?.Add(item1.voters);
                }

            return pollModel;
        }

        private async Task<byte[]?> GetPhoto(Photo photo)
        {
            try
            {
                using MemoryStream mStream = new MemoryStream();
                await _client.DownloadFileAsync(photo, mStream);

                return mStream.ToArray();
            }
            catch { }

            return null;
        }

        private async Task<string?> GetVideo(Document document)
        {
            var temp = document.Filename;

            if (string.IsNullOrEmpty(temp))
            {
                Random random = new Random();
                temp = random.Next(1, 800000).ToString() + ".mp4";
            }

            try
            {
                using var fileStream = _filesWork.CreateFile("TempVideo/" + temp);
                await _client.DownloadFileAsync(document, fileStream);
                // Debug.WriteLine(fileStream.Name);
                return fileStream.Name;
            }
            catch { }

            return null;
        }

        private void WatchMedia_Modal(MessageModel? message)
        {
            _dialogService.ShowDialog("Modal", new DialogParameters() { { "message", message } }, (r) => { });//, r =>
            //{
            //    if (r.Result == ButtonResult.None)
            //        Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAa");
            //    else if (r.Result == ButtonResult.OK)
            //        Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAa");
            //    else if (r.Result == ButtonResult.Cancel)
            //        Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAa");
            //    else
            //        Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAa");
            //});
        }


        #region Interfaces

        protected async override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {
                case "SelectedGroup":
                    if (SelectedGroup != null)
                    {
                        ListMessage.Clear();
                        _filesWork.ClearFolder("TempVideo/");
                        await GetMessages(SelectedGroup.Group);
                        //SelectedGroup = null;
                    }
                    break;

                case "SelectedMessage":
                    if (SelectedMessage != null)
                    {
                        WatchMedia_Modal(SelectedMessage);
                        SelectedMessage = null;
                    }
                    break;
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        #endregion
    }
}
