using MediatR;
using System;

namespace ChatService.Commands
{
    public abstract class Command<TResult>
            : IRequest<TResult>
    {
        public Guid MessageId { get; private set; }
        public string MessageType { get; protected set; }

        protected Command()
        {
            MessageType = GetType().Name;
            Timestamp = DateTime.Now;
        }

        public DateTime Timestamp { get; set; }
    }
}
