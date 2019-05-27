using Unity;
using Prism.Unity;
using VlcMessagePlayer.MainApp.Views;
using System.Windows;
using VlcMessagePlayer.Common;

namespace VlcMessagePlayer.MainApp
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterType<IDialogService, DialogService>();
            Container.RegisterType<IMessageService, MessageService>();
        }
    }
}
