using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeX.BinaryMessageBus.Parser
{
    class MessageCommand : IMessageParser
    {
        private string GUID = Guid.NewGuid().ToString();
        private byte MessageType = (byte)'C';

        public string GetGuid()
        {
            return GUID;
        }

        public byte GetMessageType()
        {
            return MessageType;
        }

        public bool ProcessMessage(byte[] inMessage, ref byte[] outMessage)
        {
            Console.WriteLine("MessageCommand {0} - {1}", Encoding.UTF8.GetString(inMessage), GUID);
            return true;
        }
    }
}
