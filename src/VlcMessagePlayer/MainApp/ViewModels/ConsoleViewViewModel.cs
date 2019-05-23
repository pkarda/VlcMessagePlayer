using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VlcMessagePlayer.Common.Events;

namespace VlcMessagePlayer.MainApp.ViewModels
{
    public class ConsoleViewViewModel : BindableBase
    {
        private ObservableCollection<string> _messages;
        public ObservableCollection<string> Messages
        {
            get { return _messages; }
            set { SetProperty(ref _messages, value); }
        }

        private IEventAggregator _ea;
        public ConsoleViewViewModel(IEventAggregator ea)
        {
            _ea = ea;
            _ea.GetEvent<MessageSendEvent>().Subscribe(MessageRecieved);
        }

        private void MessageRecieved(string message)
        {
            Messages.Add(message);
        }
    }
}
