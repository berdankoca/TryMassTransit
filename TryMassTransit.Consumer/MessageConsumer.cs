using System;
using System.Threading.Tasks;
using MassTransit;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class MessageConsumer : IConsumer<Message>
    {
        private readonly MessageDBContext _messageDBContext;

        public MessageConsumer(MessageDBContext messageDBContext)
        {
            _messageDBContext = messageDBContext;
        }

        public Task Consume(ConsumeContext<Message> context)
        {
            Console.WriteLine($"Consumer: { context.Message.Text }");
            _messageDBContext.Add(context.Message);

            return Task.CompletedTask;
        }
    }
}