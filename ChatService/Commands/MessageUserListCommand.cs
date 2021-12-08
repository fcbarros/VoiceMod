using ChatCore.Messages;
using NetMQ.Sockets;

namespace ChatService.Commands
{
    public class MessageUserListCommand
        : Command<bool>
    {
        public MessageUserListCommand(MessageUserList message, PublisherSocket socket)
        {
            Message = message;
            Socket = socket;
        }

        public MessageUserList Message { get; set; }
        public PublisherSocket Socket { get; set; }
    }
}
