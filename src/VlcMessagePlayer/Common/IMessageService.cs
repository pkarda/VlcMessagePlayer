using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VlcMessagePlayer.Common
{
    public interface IMessageService
    {
        Task<string> Send(SerialMessage message);
    }
}
