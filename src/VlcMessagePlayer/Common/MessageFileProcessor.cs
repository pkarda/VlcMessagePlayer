using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Configuration;

namespace VlcMessagePlayer.Common
{
    public class MessageFileProcessor
    {
        private string _inputFile;
        public MessageFileProcessor(string inputFile)
        {
            _inputFile = inputFile;
        }

        public IDictionary<long, SerialMessage> CreateMessageBuffer()
        {
            var output = new Dictionary<long, SerialMessage>();

            using (TextFieldParser parser = new TextFieldParser(_inputFile))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(ConfigurationManager.AppSettings["CsvDelimiter"]);
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    output.Add(Convert.ToInt64(fields[0]), new SerialMessage { PortName = fields[1], Message = fields[2] });
                }
            }

            return output;
        }
    }
}
