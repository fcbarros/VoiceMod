using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService.Commands.Handlers
{
    public class MessageServerDisconnectCommandHandler
        : IRequestHandler<MessageServerDisconnectCommand, bool>
    {
        private readonly ILogger<MessageTextCommandHandler> m_logger;
        public MessageServerDisconnectCommandHandler(ILogger<MessageTextCommandHandler> logger)
        {
            m_logger = logger;
        }

        public async Task<bool> Handle(MessageServerDisconnectCommand request, CancellationToken cancellationToken)
        {
            request.Message.Send(request.Socket);

            return true;
        }
    }
}
