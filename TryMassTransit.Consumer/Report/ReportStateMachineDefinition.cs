using MassTransit;
using MassTransit.Definition;

namespace TryMassTransit.Consumer
{
    public class ReportStateMachineDefinition
        : SagaDefinition<ReportSagaState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<ReportSagaState> sagaConfigurator)
        {
            endpointConfigurator.UseInMemoryOutbox();
        }
    }
}
