using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VlcMessagePlayer.MainApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _isFullScreen = false;
        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += MainWindow_KeyDown;
            //PlayerControl.MediaPlayer.Playing += MediaPlayer_Playing;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.F))
            {
                if (!_isFullScreen)
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                    this.ControlButtonsPanel.Visibility = Visibility.Collapsed;
                    this.ResizeMode = ResizeMode.NoResize;
                    this.Left = 0;
                    this.Top = 0;
                    this.Width = SystemParameters.VirtualScreenWidth;
                    this.Height = SystemParameters.VirtualScreenHeight;
                    this.Topmost = true;



                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                    this.ControlButtonsPanel.Visibility = Visibility.Visible;

                    this.ResizeMode = ResizeMode.CanResize;
                    this.Left = 0;
                    this.Top = 0;
                    this.Width = 756;
                    this.Height = 498;
                    this.Topmost = false;

                }

                _isFullScreen = !_isFullScreen;
            }
        }

        private void ContentControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!_isFullScreen)
            {
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                this.ControlButtonsPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.WindowStyle = WindowStyle.SingleBorderWindow;
                this.WindowState = WindowState.Normal;
                this.ControlButtonsPanel.Visibility = Visibility.Visible;

            }

            _isFullScreen = !_isFullScreen;

        }
    }
}
