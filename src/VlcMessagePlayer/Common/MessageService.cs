using System;
using System.Threading.Tasks;
using System.IO;
using RJCP.IO.Ports;

namespace VlcMessagePlayer.Common
{
    public class MessageService : IMessageService
    {
        public Task<string> Send(SerialMessage message)
        {
            return Task.Run(() =>
            {
                using (SerialPortStream port = new SerialPortStream(message.PortName, 9600, 8, Parity.None, StopBits.One))
                {
                    try
                    {
                        port.Open();
                        port.Write(message.Message);
                        port.Close();

                        return String.Empty;
                    }
                    catch(IOException ex)
                    {
                        return ex.Message;
                    }
                }     
            });
        }
    }
}
