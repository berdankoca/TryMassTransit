using System;
using System.Collections.Generic;
using System.Text;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class ReportRequestReceivedEvent : ReportRequestReceived
    {
        private readonly ReportSagaState _reportSagaState;

        public ReportRequestReceivedEvent(ReportSagaState reportSagaState)
        {
            _reportSagaState = reportSagaState;
        }

        public Guid CorrelationId => _reportSagaState.CorrelationId;

        public Guid ReportId => _reportSagaState.ReportId;

        public DateTime RequestTime => _reportSagaState.RequestTime;

        public string EMail => _reportSagaState.EMail;
    }
}
