using NetMQ;
using System.Collections.Generic;

namespace ChatCore.Messages
{
    public class MessageDisconnect
        : MessageBase
    {
        public override MessageType MessageType => MessageType.DISCONNECT;

        public string Sender { get; protected set; }

        public MessageDisconnect(string sender)
        {
            Sender = sender;
        }

        public MessageDisconnect()
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
