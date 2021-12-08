using NetMQ;
using System.Collections.Generic;

namespace ChatCore.Messages
{
    public class MessageServerDisconnect
        : MessageBase
    {
        public override MessageType MessageType => MessageType.SERVER_DISCONNECT;

        public MessageServerDisconnect()
        {
        }

        public override bool Receive(NetMQSocket socket)
        {
            (bool result, NetMQMessage msg) = TryReceiveMultipartMessage(socket, 1);
            return result;
        }

        public override bool Send(NetMQSocket socket)
        {
            return TrySendMultipartMessage(socket, new List<string>() { "true" });
        }

        public override string ToString()
        {
            return $"{MessageType}";
        }
    }
}
