using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace VlcMessagePlayer.MainApp.Notifications
{
    public class OptionsNotification : IOptionsNotification
    {
        private readonly string _vlcInstallDirectoryKey = "VlcInstallDirectory";
        public object Content
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public string VlcInstallDiretory
        {
            get { return ConfigurationManager.AppSettings[_vlcInstallDirectoryKey]; }
            set { ConfigurationManager.AppSettings.Set(_vlcInstallDirectoryKey, value); }
        }
    }
}
