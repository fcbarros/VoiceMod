using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService.Commands.Handlers
{
    public class MessageUserListCommandHandler
        : IRequestHandler<MessageUserListCommand, bool>
    {
        private readonly ILogger<MessageTextCommandHandler> m_logger;
        public MessageUserListCommandHandler(ILogger<MessageTextCommandHandler> logger)
        {
            m_logger = logger;
        }

        public async Task<bool> Handle(MessageUserListCommand request, CancellationToken cancellationToken)
        {
            request.Message.Send(request.Socket);

            return true;
        }
    }
}
