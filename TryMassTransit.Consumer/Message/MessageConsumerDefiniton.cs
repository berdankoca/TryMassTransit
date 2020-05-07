using MassTransit;
using MassTransit.ConsumeConfigurators;
using MassTransit.Definition;

namespace TryMassTransit.Consumer
{
    public class MessageConsumerDefiniton
        : ConsumerDefinition<MessageConsumer>
    {

        public MessageConsumerDefiniton()
        {
            EndpointName = "deneme-queue";
        }

        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<MessageConsumer> consumerConfigurator)
        {
            
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator);
        }

        
    }
}
