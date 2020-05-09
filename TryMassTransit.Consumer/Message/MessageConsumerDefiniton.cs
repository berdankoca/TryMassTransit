using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;
using MassTransit.RabbitMqTransport.Configuration;
using RabbitMQ.Client;

namespace TryMassTransit.Consumer
{
    public class MessageConsumerDefiniton
        : ConsumerDefinition<MessageConsumer>
    {

        public MessageConsumerDefiniton()
        {
            //EndpointName = "deneme-queue";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<MessageConsumer> consumerConfigurator)
        {
            //((RabbitMqReceiveEndpointConfiguration)endpointConfigurator).ExchangeType = ExchangeType.Direct;
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }

        
    }
}
