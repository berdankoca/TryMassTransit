using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class GetMessagesConsumer : IConsumer<GetMessages>
    {
        private readonly MessageDBContext _messageDBContext;

        public GetMessagesConsumer(MessageDBContext messageDBContext)
        {
            _messageDBContext = messageDBContext;
        }

        public async Task Consume(ConsumeContext<GetMessages> context)
        {
            var result = _messageDBContext.GetList().Where(m => m.Text.EndsWith(context.Message.EndWithFilter));

            await context.RespondAsync<MessagesResult>(new { Messages = result });
        }
    }
}
