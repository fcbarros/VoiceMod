using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace ChatService.Commands.Handlers
{
    public class MessageTextCommandHandler
        : IRequestHandler<MessageTextCommand, bool>
    {
        private readonly ILogger<MessageTextCommandHandler> m_logger;
        public MessageTextCommandHandler(ILogger<MessageTextCommandHandler> logger)
        {
            m_logger = logger;
        }

        public async Task<bool> Handle(MessageTextCommand request, CancellationToken cancellationToken)
        {
            request.Message.Send(request.Socket);

            return true;
        }
    }
}
