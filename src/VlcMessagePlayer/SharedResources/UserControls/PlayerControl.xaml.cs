using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using Vlc.DotNet.Core;


namespace VlcMessagePlayer.SharedResources.UserControls
{
    public partial class PlayerControl : UserControl, IComponentConnector
    {
        public static readonly DependencyProperty StartFromProperty = DependencyProperty.Register("StartFrom", typeof(long?), typeof(UserControl), new PropertyMetadata(new PropertyChangedCallback(PlayerControl.OnStartFromChanged)));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(float?), typeof(PlayerControl), new PropertyMetadata(new PropertyChangedCallback(PlayerControl.OnPositionChanged)));
        public static readonly DependencyProperty EndReachedProperty = DependencyProperty.Register("EndReached", typeof(bool), typeof(PlayerControl), new PropertyMetadata((object)false));
        public static readonly DependencyProperty IsPlayingProperty = DependencyProperty.Register("IsPlaying", typeof(bool), typeof(PlayerControl), new PropertyMetadata(new PropertyChangedCallback(PlayerControl.OnPlayingChanged)));
        public static readonly DependencyProperty PlayTimeProperty = DependencyProperty.Register("PlayTime", typeof(long?), typeof(PlayerControl), new PropertyMetadata((PropertyChangedCallback)null));
        public static readonly DependencyProperty VlcPathProperty = DependencyProperty.Register("VlcPath", typeof(string), typeof(PlayerControl), new PropertyMetadata((object)ConfigurationManager.AppSettings["VlcInstallDirectory"]));
        public static readonly DependencyProperty MediaPathProperty = DependencyProperty.Register("MediaPath", typeof(string), typeof(PlayerControl), (PropertyMetadata)new FrameworkPropertyMetadata(new PropertyChangedCallback(PlayerControl.OnMediaPathChanged)));
        private long? _oldTime;
        private Dispatcher _dispatcher;
        //internal Grid PlayerContent;
        //private bool _contentLoaded;

        public string MediaPath
        {
            get
            {
                return (string)this.GetValue(PlayerControl.MediaPathProperty);
            }
            set
            {
                this.SetValue(PlayerControl.MediaPathProperty, (object)value);
            }
        }

        public string VlcPath
        {
            get
            {
                return (string)this.GetValue(PlayerControl.VlcPathProperty);
            }
            set
            {
                this.SetValue(PlayerControl.VlcPathProperty, (object)value);
            }
        }

        public long? PlayTime
        {
            get
            {
                return this._dispatcher.Invoke<long?>((Func<long?>)(() => (long?)this.GetValue(PlayerControl.PlayTimeProperty)));
            }
            set
            {
                this._dispatcher.BeginInvoke((Action)(() => this.SetValue(PlayerControl.PlayTimeProperty, (object)value)), Array.Empty<object>());
            }
        }

        public bool IsPlaying
        {
            get =>
                ((bool)base.GetValue(IsPlayingProperty));
            set =>
                base.SetValue(IsPlayingProperty, value);
        }

        public bool EndReached
        {
            get
            {
                return this._dispatcher.Invoke<bool>((Func<bool>)(() => (bool)this.GetValue(PlayerControl.EndReachedProperty)));
            }
            set
            {
                this._dispatcher.BeginInvoke((Action)(() => this.SetValue(PlayerControl.EndReachedProperty, value)), Array.Empty<object>());
            }
        }

        public float? Position
        {
            get
            {
                return (float?)this.GetValue(PlayerControl.PositionProperty);
            }
            set
            {
                this.SetValue(PlayerControl.PositionProperty, (object)value);
            }
        }

        public long? StartFrom
        {
            get
            {
                return (long?)this.GetValue(PlayerControl.StartFromProperty);
            }
            set
            {
                this.SetValue(PlayerControl.StartFromProperty, (object)value);
            }
        }

        public PlayerControl()
        {
            this.InitializeComponent();
            this._dispatcher = Application.Current.Dispatcher;
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            var libDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            this.VlcWpfPlayerControl.SourceProvider.CreatePlayer(libDirectory/* pass your player parameters here */);
            this.VlcWpfPlayerControl.SourceProvider.MediaPlayer.TimeChanged += new EventHandler<VlcMediaPlayerTimeChangedEventArgs>(this.MediaPlayer_TimeChanged);
            this.VlcWpfPlayerControl.SourceProvider.MediaPlayer.EndReached += new EventHandler<VlcMediaPlayerEndReachedEventArgs>(this.MediaPlayer_EndReached);
        }

        private static void OnStartFromChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PlayerControl).VlcWpfPlayerControl.SourceProvider.MediaPlayer.Time = (long)e.NewValue;
        }

        private static void OnPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlayerControl playerControl = d as PlayerControl;
            if (string.IsNullOrWhiteSpace(playerControl.MediaPath))
                return;
            if (playerControl.IsPlaying)
            {
                playerControl.VlcWpfPlayerControl.SourceProvider.MediaPlayer.Play();
                long? startFrom = playerControl.StartFrom;
                if (startFrom.HasValue)
                {
                    Vlc.DotNet.Core.VlcMediaPlayer mediaPlayer = playerControl.VlcWpfPlayerControl.SourceProvider.MediaPlayer;
                    startFrom = playerControl.StartFrom;
                    long num = startFrom.Value;
                    mediaPlayer.Time = num;
                }
            }
            else
                playerControl.VlcWpfPlayerControl.SourceProvider.MediaPlayer.Pause();
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // NOOP
        }

        private static void OnMediaPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PlayerControl playerControl = d as PlayerControl;
            if (string.IsNullOrWhiteSpace(playerControl.MediaPath))
                return;
            playerControl.VlcWpfPlayerControl.SourceProvider.MediaPlayer.SetMedia(new FileInfo(playerControl.MediaPath), Array.Empty<string>());
        }

        private void MediaPlayer_EndReached(object sender, VlcMediaPlayerEndReachedEventArgs e)
        {
            this.EndReached = true;
        }

        private void MediaPlayer_TimeChanged(object sender, VlcMediaPlayerTimeChangedEventArgs e)
        {
            long newTime = e.NewTime;
            long num = newTime;
            long? nullable = this._oldTime;
            long valueOrDefault = nullable.GetValueOrDefault();
            if (num == valueOrDefault && nullable.HasValue)
                return;
            this.PlayTime = new long?(newTime);
            this._oldTime = new long?(newTime);
        }
    }
}
