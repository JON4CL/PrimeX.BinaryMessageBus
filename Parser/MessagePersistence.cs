using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeX.BinaryMessageBus.Parser
{
    class MessagePersistence : IMessageParser
    {
        private string GUID = Guid.NewGuid().ToString();
        private byte MessageType = (byte)'E';

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
            Console.WriteLine("MessagePersistence {0} - {1}", Encoding.UTF8.GetString(inMessage), GUID);

            Buffer.BlockCopy(inMessage, 0, outMessage, 0, inMessage.Length);

            using (StreamWriter sw = File.AppendText("MessagePersistence.log"))
            {
                sw.WriteLine(Encoding.UTF8.GetString(inMessage) + " " + GUID);
            }

            return true;
        }
    }
}
