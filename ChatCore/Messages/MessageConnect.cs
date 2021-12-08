using NetMQ;
using System.Collections.Generic;

namespace ChatCore.Messages
{
    public class MessageConnect
        : MessageBase
    {
        public override MessageType MessageType => MessageType.CONNECT;

        public string Sender { get; protected set; }

        public MessageConnect(string sender)
        {
            Sender = sender;
        }

        public MessageConnect()
        {
        }

        public override bool Receive(NetMQSocket socket)
        {
            (bool result, NetMQMessage msg) = TryReceiveMultipartMessage(socket, 1);
            if (result)
            {
                Sender = msg[0].ConvertToString();
            }
            return result;
        }

        public override bool Send(NetMQSocket socket)
        {
            return TrySendMultipartMessage(socket, new List<string>() { Sender });
        }

        public override string ToString()
        {
            return $"{MessageType}: Sender: {Sender}";
        }
    }
}
