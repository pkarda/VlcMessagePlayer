using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VlcMessagePlayer.MainApp.Notifications
{
    public interface IOptionsNotification : INotification
    {
        string VlcInstallDiretory { get; set; }
    }
}
