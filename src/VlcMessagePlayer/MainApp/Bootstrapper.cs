using Prism.Unity;
using System.Windows;
using Microsoft.Practices.Unity;
using VlcMessagePlayer.Common;
using VlcMessagePlayer.MainApp.Views;

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
