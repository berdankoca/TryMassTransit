using MassTransit;
using System;
using System.Data;
using System.Threading.Tasks;

namespace TryMassTransit.Api.Contracts
{
    //This class represent Shipment entity
    public class Shipment
        : IConsumer<OrderUpdating>
    {
        int ShipmentId { get; set; }

        DateTime ShipmentTime { get; set; }

        int OrderId { get; set; }

        public Task Consume(ConsumeContext<OrderUpdating> context)
        {
            Console.WriteLine($"Check the shipment when order updating if something wrong throw exception, OrderId: {context.Message.OrderId}");

            return Task.CompletedTask;
            //return context.RespondAsync<UpdatingResult>(new { isSuccess = true });
        }
    }
}
