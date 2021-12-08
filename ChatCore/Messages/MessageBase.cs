using NetMQ;
using System;
using System.Collections.Generic;

namespace ChatCore.Messages
{
    public abstract class MessageBase
        : IMessage
    {
        public virtual MessageType MessageType { get; }

        public abstract bool Receive(NetMQSocket socket);

        public abstract bool Send(NetMQSocket socket);

        public abstract override string ToString();

        protected static (bool, NetMQMessage) TryReceiveMultipartMessage(NetMQSocket socket, int numFrames)
        {
            NetMQMessage message = new();
            bool? result = socket?.TryReceiveMultipartMessage(TimeSpan.FromMilliseconds(2000), ref message, numFrames);
            return (result.HasValue ? result.Value : false, message);
        }

        protected bool TrySendMultipartMessage(NetMQSocket socket, List<string> list)
        {
            List<NetMQFrame> frames = new();
            frames.Add(new NetMQFrame(MessageType.ToString()));
            list.ForEach(s => frames.Add(new NetMQFrame(s)));
            NetMQMessage msg = new(frames);
            bool? sent = socket?.TrySendMultipartMessage(TimeSpan.FromMilliseconds(2000), msg);
            return sent.HasValue ? sent.Value : false;
        }

        public static IMessage Create(NetMQSocket socket)
        {
            string msgTypeStr = socket.ReceiveFrameString();
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), msgTypeStr);
            MessageBase msg = type switch
            {
                MessageType.TEXT => new MessageText(),
                MessageType.CONNECT => new MessageConnect(),
                MessageType.DISCONNECT => new MessageDisconnect(),
                MessageType.USER_LIST => new MessageUserList(),
                MessageType.SERVER_DISCONNECT => new MessageServerDisconnect(),
                _ => throw new NotImplementedException()
            };
            msg.Receive(socket);
            return msg;
        }
    }
}
