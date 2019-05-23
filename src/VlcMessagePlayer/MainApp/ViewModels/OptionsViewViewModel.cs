using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VlcMessagePlayer.MainApp.Notifications;

namespace VlcMessagePlayer.MainApp.ViewModels
{
    public class OptionsViewViewModel : BindableBase, IInteractionRequestAware
    {
        public Action FinishInteraction { get; set; }

        private IOptionsNotification _notification;

        public INotification Notification
        {
            get { return _notification; }
            set { SetProperty(ref _notification, (IOptionsNotification)value); }
        }
    }
}
