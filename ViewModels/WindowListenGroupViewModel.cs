

using Telegram_WPF.Models;
using Telegram_WPF.Services.ListenMsg;
using Telegram_WPF.Services.FilesService;

using Prism.Commands;
using Prism.Services.Dialogs;

using TL;

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Linq;
using System.IO;
using System;
using System.Windows.Interop;


namespace Telegram_WPF.ViewModels
{

    public delegate void MessageCallback(Message msg);

    internal class WindowListenGroupViewModel : BaseViewModel, IDialogAware
    {


        public event Action<IDialogResult> RequestClose;

        private MessageCallback? messageCallback = null;
        private System.Windows.Threading.Dispatcher _dispatcher;
        private bool _isForward; 


        public WindowListenGroupViewModel(IListenGroups listenGroups,
                                          IFilesWork filesWork,
                                          IDialogService dialogService,
                                          IValidateListenMsg validateListenMsg)
        : base()
        {
            _validateListenMsg = validateListenMsg;
            _dialogService = dialogService;
            _listenGroups = listenGroups;
            _filesWork = filesWork;

            messageCallback = Message_Callback;
            ListMessage = new ObservableCollection<MessageModel>();
            ListGroup = new List<ChatBase_Img>(_listGroup_static);
            SelectedGroupList = new List<ChatBase_Img?>();

            _dispatcher = Application.Current.MainWindow.Dispatcher;

            IsVisibleListGroup = "Hidden";
            IsValidInput = false;
            ShouldBeText = "";
            NotShouldBeText = "";
            IsCheckedForwardHeader = false;
        }


        #region public property

        private string _title = "Listen group";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
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

        private List<ChatBase_Img> _listGroup;
        public List<ChatBase_Img> ListGroup
        {
            get => _listGroup;
            set => SetProperty(ref _listGroup, value);
        }


        private List<ChatBase_Img?> _selectedGroupList;
        public List<ChatBase_Img?> SelectedGroupList
        {
            get => _selectedGroupList;
            set => SetProperty(ref _selectedGroupList, value);
        }


        private ChatBase_Img? _selectGroupToForward;
        public ChatBase_Img? SelectGroupToForward
        {
            get => _selectGroupToForward;
            set => SetProperty(ref _selectGroupToForward, value);
        }


        private string _isVisibleListGroup;
        public string IsVisibleListGroup
        {
            get => _isVisibleListGroup;
            set => SetProperty(ref _isVisibleListGroup, value);
        }


        private string? _shouldBeText;
        public string? ShouldBeText
        {
            get => _shouldBeText;
            set => SetProperty(ref _shouldBeText, value);
        }


        private string? _notShouldBeText;
        public string? NotShouldBeText
        {
            get => _notShouldBeText;
            set => SetProperty(ref _notShouldBeText, value);
        }


        private bool _isValidInput;
        public bool IsValidInput
        {
            get => _isValidInput;
            set => SetProperty(ref _isValidInput, value);
        }


        private bool _isCheckedForwardHeader;
        public bool IsCheckedForwardHeader
        {
            get => _isCheckedForwardHeader;
            set => SetProperty(ref _isCheckedForwardHeader, value);
        }


        public DelegateCommand<string> StartListenBtn => new DelegateCommand<string>(StartListen);
        public DelegateCommand<object[]> SelectedCommand => new DelegateCommand<object[]>(Selected_Groups);
        public DelegateCommand ListChooseGroupBtn => new DelegateCommand(GroupListVisible_Click);


        #endregion


        private void StartListen(string type)
        {
            if (!_isPressed)
            {
                _isPressed = true;

                if (SelectedGroupList == null || SelectedGroupList.Count == 0)
                {
                    SelectedGroupList = new List<ChatBase_Img?>(_listGroup_static);
                }

                _client.OnUpdate += Client_OnUpdate;

                if (type == "listen_forward")//listen and forward
                {
                    if (SelectGroupToForward != null)
                    {
                        _isForward = true;         
                    }
                    else
                    {
                        MessageBox.Show("Choose which group to forward the message to", "Error", MessageBoxButton.OK);
                        _isPressed = false;
                    }
                }
                else//just listen
                {
                    _isForward = false;
                }
            }
        }

        private void GroupListVisible_Click()
        {
            IsVisibleListGroup = (IsVisibleListGroup == "Hidden") ? "Visible" : "Hidden";
        }

        private void Selected_Groups(object[] selectedItems)
        {

            if (selectedItems != null && selectedItems.Count() > 0)
            {
                foreach (var item in selectedItems)
                {
                    if (item is ChatBase_Img aa) SelectedGroupList.Add(aa);
                }
            }
        }

        private async Task ForwardMsg(Message msg)
        {
            try
            {
                var chat = (SelectedGroupList.FirstOrDefault(x => x.Group.ID == msg.peer_id.ID)).Group;
                InputPeer? inputPeer = chat.ToInputPeer();

                if (IsCheckedForwardHeader)//without forward header from
                {
                    await _client.Messages_ForwardMessages(inputPeer, new[] { msg.ID },
                                     new[] { WTelegram.Helpers.RandomLong() },
                                     SelectGroupToForward.Group, drop_author: true);
                    return;
                }

                //with forward
                await _client.Messages_ForwardMessages(inputPeer, new[] { msg.ID },
                                     new[] { WTelegram.Helpers.RandomLong() },
                                     SelectGroupToForward.Group);
            }
            catch { }
        }

        private async void Message_Callback(Message msg)
        {
            if (msg != null)
            {
                if (await _validateListenMsg.IsValidShouldBeMsg(ShouldBeText, SelectedGroupList, msg)
                    && await _validateListenMsg.IsValidNotShouldBeMsg(NotShouldBeText, SelectedGroupList, msg))
                {

                    if(_isForward) await ForwardMsg(msg);

                    byte[]? tempByte = null;
                    string? tempPath = null;
                    PollModel? tempPoll = null;
                    long groupId = 0;
                    MessageModel messageModel = new MessageModel();

                    //if (msg.media is MessageMediaPoll { poll: Poll poll, results: PollResults pollresult })
                    //{
                    //    tempPoll = GetPoll(poll, pollresult);
                    //}

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
                            }
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

                        await _dispatcher.BeginInvoke(new Action(() =>
                        {
                            ListMessage.Add(messageModel);
                        }));
                    }
                    else if (groupId == msg.grouped_id)
                    {
                        var index = ListMessage.IndexOf(messageModel);

                        messageModel.Message += msg.message;

                        if (tempByte != null) messageModel.Imgs.Add(tempByte);
                        if (tempPath != null) messageModel.Videos.Add(tempPath);

                        if (index > -1)
                        {
                            await _dispatcher.BeginInvoke(new Action(() =>
                            {
                                ListMessage.RemoveAt(index);
                            }));
                        }

                        await _dispatcher.BeginInvoke(new Action(() =>
                        {
                            ListMessage.Add(messageModel);
                        }));
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

        private async Task Client_OnUpdate(UpdatesBase arg)
        {
            await _listenGroups.Client_OnUpdate(arg, _client, messageCallback);
        }

        private void WatchMedia_Modal(MessageModel? message)
        {
            _dialogService.ShowDialog("Modal", new DialogParameters() { { "message", message } }, (r) => { });
        }


        #region Interfaces

        protected override void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnPropertyChanged(args);

            switch (args.PropertyName)
            {

                case "SelectedMessage":
                    if (SelectedMessage != null)
                    {
                        WatchMedia_Modal(SelectedMessage);
                        SelectedMessage = null;
                    }
                    break; 

                        case "SelectGroupToForward":
                    if (SelectGroupToForward != null)
                    {
                        SelectGroupToForward = null;
                    }
                    break;
            }
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            _client.OnUpdate -= Client_OnUpdate;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            IsVisibleListGroup = "Hidden";
            IsValidInput = false;
            ShouldBeText = "";
            NotShouldBeText = "";
            _isPressed = false;
            IsCheckedForwardHeader = false;
        }

        #endregion
    }
}
