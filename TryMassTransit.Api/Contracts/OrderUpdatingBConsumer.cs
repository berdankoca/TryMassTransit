using MassTransit;
using System;
using System.Threading.Tasks;

namespace TryMassTransit.Api.Contracts
{
    public class OrderUpdatingBConsumer
        : IConsumer<OrderUpdating>
    {
        public Task Consume(ConsumeContext<OrderUpdating> context)
        {
            Console.WriteLine($"Check the order when updated, OrderId: {context.Message.OrderId}");

            return Task.CompletedTask;
           }
    }
}
