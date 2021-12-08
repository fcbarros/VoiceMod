using NetMQ;
using System.Collections.Generic;

namespace ChatCore.Messages
{
    public class MessageText
        : MessageBase
    {
        public override MessageType MessageType => MessageType.TEXT;

        public string Sender { get; set; }
        public string Text { get; set; }

        public MessageText(string sender, string text)
        {
            Sender = sender;
            Text = text;
        }

        public MessageText()
        {
        }

        public override bool Receive(NetMQSocket socket)
        {
            (bool result, NetMQMessage msg) = TryReceiveMultipartMessage(socket, 2);
            if (result)
            {
                Sender = msg[0].ConvertToString();
                Text = msg[1].ConvertToString();
            }
            return result;
        }

        public override bool Send(NetMQSocket socket)
        {
            return TrySendMultipartMessage(socket, new List<string>() { Sender, Text });
        }

        public override string ToString()
        {
            return $"{MessageType}: {Sender}: {Text}";
        }
    }
}
