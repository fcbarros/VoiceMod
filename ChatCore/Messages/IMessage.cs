using NetMQ;

namespace ChatCore.Messages
{
    public interface IMessage
    {
        MessageType MessageType { get; }

        bool Receive(NetMQSocket socket);

        bool Send(NetMQSocket socket);
    }
}
