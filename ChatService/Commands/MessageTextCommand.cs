using ChatCore.Messages;
using NetMQ.Sockets;

namespace ChatService.Commands
{
    public class MessageTextCommand
        : Command<bool>
    {
        public MessageTextCommand(MessageText message, PublisherSocket socket)
        {
            Message = message;
            Socket = socket;
        }

        public MessageText Message { get; set; }
        public PublisherSocket Socket { get; set; }
    }
}
