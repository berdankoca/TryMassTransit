using System;
using System.Threading.Tasks;
using MassTransit;

namespace TryMassTransit.Consumer
{
    public class MessageConsumer : IConsumer<Shared.Message>
    {
        private readonly MessageDBContext _messageDBContext;

        public MessageConsumer(MessageDBContext messageDBContext)
        {
            _messageDBContext = messageDBContext;
        }

        public Task Consume(ConsumeContext<Shared.Message> context)
        {
            Console.WriteLine($"Consumer: { context.Message.Text }");
            _messageDBContext.Add(context.Message);

            return Task.CompletedTask;
        }
    }
}