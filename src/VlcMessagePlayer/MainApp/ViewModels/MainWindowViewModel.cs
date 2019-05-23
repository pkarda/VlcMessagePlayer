using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using VlcMessagePlayer.Common;
using VlcMessagePlayer.Common.Events;
using VlcMessagePlayer.MainApp.Notifications;

namespace VlcMessagePlayer.MainApp.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "VLC Message Player";
        private string _mediaPath;
        private string _messagesFilePath;
        private long _playTime;
        private long _oldPlayTime;
        private ObservableCollection<string> _debugMessageList;
        private ControlWindowViewModel _controlWindowViewModel;
        private IDictionary<long, SerialMessage> _messageBuffer;
        private long _totalTime;
        private System.Diagnostics.Stopwatch _stopWatch;
        private MicroTimer microTimer = new MicroTimer();
        protected IDialogService DialogService { get { return _dialogService; } }
        private IDialogService _dialogService;
        private IMessageService _messageService;
        private IEventAggregator _ea;
        private Dispatcher _dispatcher;

        public ICommand PlayCommand { get; private set; }

        public ICommand OpenMediaCommand { get; private set; }

        public ICommand OpenMessagesCommand { get; private set; }

        public ICommand WindowClosing { get; private set; }

        public DelegateCommand PlayPauseCommand { get; private set; }
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string MediaPath
        {
            get { return _mediaPath; }
            set { SetProperty(ref _mediaPath, value); }
        }

        public string MessagesFilePath
        {
            get { return _messagesFilePath; }
            set { SetProperty(ref _messagesFilePath, value); }
        }
        public long PlayTime
        {
            get { return _playTime; }
            set
            {
                _oldPlayTime = _playTime;
                SetProperty(ref _playTime, value);
            }
        }

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { SetProperty(ref _isPlaying, value); }
        }

        private bool _endReached = false;
        public bool EndReached
        {
            get { return _endReached; }
            set { SetProperty(ref _endReached, value); }
        }

        private string _playStateLabel;

        public string PlayStateLabel
        {
            get { return _playStateLabel; }
            set { SetProperty(ref _playStateLabel, value); }
        }

        public ObservableCollection<string> DebugMessageList
        {
            get { return _debugMessageList; }
            set { SetProperty(ref _debugMessageList, value); }
        }

        private int? _skipForwardSeconds;
        public int? SkipForwardSeconds
        {
            get { return _skipForwardSeconds; }
            set { SetProperty(ref _skipForwardSeconds, value); }
        }

        private float? _position;
        public float? Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }


        private long? _startFrom;
        public long? StartFrom
        {
            get { return _startFrom; }
            set { SetProperty(ref _startFrom, value); }
        }
        public InteractionRequest<IOptionsNotification> CustomNotificationRequest { get; set; }
        public DelegateCommand CustomNotificationCommand { get; set; }

        public ICommand SkipForward5sCommand { get; private set; }
        public ICommand SkipForward30sCommand { get; private set; }
        public ICommand SkipForward60sCommand { get; private set; }

        public MainWindowViewModel(IDialogService dialogService, IMessageService messageService, IEventAggregator ea)
        {
            this.OpenMediaCommand = new DelegateCommand(this.OnOpenMediaCommand);
            this.OpenMessagesCommand = new DelegateCommand(this.OnOpenMessagesCommand);
            this.PlayPauseCommand = new DelegateCommand(this.OnPlayPauseCommand, CanPlayPause);
            this.SkipForward5sCommand = new DelegateCommand<object>(OnSkipForwardCommand, CanSkip);
            this.SkipForward30sCommand = new DelegateCommand<object>(OnSkipForwardCommand, CanSkip);
            this.SkipForward60sCommand = new DelegateCommand<object>(OnSkipForwardCommand, CanSkip);
            this.PlayStateLabel = "Play";
            this._dialogService = dialogService;
            this._controlWindowViewModel = new ControlWindowViewModel();

            CustomNotificationRequest = new InteractionRequest<IOptionsNotification>();
            CustomNotificationCommand = new DelegateCommand(RaiseOptionNotifications);
            this._debugMessageList = new ObservableCollection<string>();
            this.PropertyChanged += MainWindowViewModel_PropertyChanged;
            this._messageService = messageService;
            this._ea = ea;
            this._stopWatch = new System.Diagnostics.Stopwatch();
            this.microTimer.MicroTimerElapsed += new MicroTimer.MicroTimerElapsedEventHandler(OnTimedEvent);
            this.microTimer.Interval = 10000;
            _dispatcher = System.Windows.Application.Current.Dispatcher;
            this.WindowClosing = new DelegateCommand<object>(this.OnWindowClosing);
        }

        private bool CanSkip(object arg)
        {
            return true;
        }

        private void OnSkipForwardCommand(object obj)
        {
            if (obj != null)
            {
                int skipSeconds = Convert.ToInt32(obj);
                Position = (float)0.5;
            }
            //throw new NotImplementedException();
        }

        private bool CanPlayPauseOnSkip(int arg)
        {
            return CanPlayPause();
        }

        private void OnWindowClosing(object sender)
        {
            this.microTimer.Enabled = false;
            System.Windows.Application.Current.Shutdown();
        }

        private void OnTimedEvent(object sender, MicroTimerEventArgs timerEventArgs)
        {
            //_debugMessageList.Add(timerEventArgs.ElapsedMicroseconds.ToString());
            _dispatcher.BeginInvoke((Action)(() =>
            {
                _totalTime += 10;


                //_debugMessageList.Add(_totalTime.ToString())
                if (_messageBuffer != null)
                {

                    var message = new SerialMessage();
                    _messageBuffer.TryGetValue(_totalTime, out message);
                    if (message != null)
                    {
                        var result = _messageService.Send(message).GetAwaiter().GetResult();
                        if (String.IsNullOrWhiteSpace(result))
                        {
                            _debugMessageList.Add($"Sending message '{message.Message}' at {_totalTime}ms to port: {message.PortName}, OK");
                        }
                        else
                        {
                            _debugMessageList.Add($"Sending message '{message.Message}' at {_totalTime}ms to port: {message.PortName}, FAILED, {result}");
                        }

                        // _ea.GetEvent<MessageSendEvent>().Publish("");
                    }
                }
            }

            ));
        }

        private bool CanPlayPause()
        {
            return !String.IsNullOrWhiteSpace(MediaPath);
        }

        private void MainWindowViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EndReached")
            {
                microTimer.Enabled = false;
                IsPlaying = false;
            }

            if(e.PropertyName == "StartFrom")
            {
                _totalTime = _startFrom != null ? _startFrom.Value : 0;
            }
        }

        private void RaiseOptionNotifications()
        {
            // TODO: Options/settings window
            CustomNotificationRequest.Raise(new OptionsNotification()
            {
                Title = "Options",
                Content = "",
                VlcInstallDiretory = "VLC installation directory"
            });
        }

        private void OnPlayPauseCommand()
        {
            IsPlaying = !IsPlaying;
            PlayStateLabel = IsPlaying ? "Pause" : "Play";
            if (IsPlaying)
            {
                microTimer.Enabled = true;
            }
            else
            {
                microTimer.Enabled = false;
            }
        }

        private void OnOpenMediaCommand()
        {
            var fileName = DialogService.OpenFileDialog();
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                MediaPath = fileName;
                _debugMessageList.Add("Opening media file: " + fileName);
                _totalTime = _startFrom != null ? _startFrom.Value : 0;
            }

            PlayPauseCommand.RaiseCanExecuteChanged();
        }

        private void OnOpenMessagesCommand()
        {
            var fileName = DialogService.OpenFileDialog();
            if (!String.IsNullOrWhiteSpace(fileName))
            {
                MessageFileProcessor messageProcessor = new MessageFileProcessor(fileName);
                _messageBuffer = messageProcessor.CreateMessageBuffer();
                _debugMessageList.Add("Opening message file: " + fileName);
            }
        }
    }
}
