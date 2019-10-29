using Automatonymous;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Consumer
{
    public class ReportSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public State CurrentState { get; set; }

        public Guid ReportId { get; set; }
        public DateTime RequestTime { get; set; }
        public string EMail { get; set; }



    }
}
