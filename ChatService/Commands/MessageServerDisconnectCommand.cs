using ChatCore.Messages;
using NetMQ.Sockets;

namespace ChatService.Commands
{
    public class MessageServerDisconnectCommand
        : Command<bool>
    {
        public MessageServerDisconnectCommand(MessageServerDisconnect message, PublisherSocket socket)
        {
            Message = message;
            Socket = socket;
        }

        public MessageServerDisconnect Message { get; set; }
        public PublisherSocket Socket { get; set; }
    }
}
