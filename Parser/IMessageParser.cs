namespace PrimeX.BinaryMessageBus.Parser
{
    internal interface IMessageParser
    {

        byte GetMessageType();
        bool ProcessMessage(byte[] inMessage, ref byte[] outMessage);
        string GetGuid();
    }
}