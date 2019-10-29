using System;
using System.Collections.Generic;
using System.Text;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class ReportRequestFailedEvent : ReportRequestFailed
    {
        private readonly ReportSagaState _reportSagaState;

        public ReportRequestFailedEvent(ReportSagaState reportSagaState)
        {
            _reportSagaState = reportSagaState;
        }
        public Guid CorrelationId => _reportSagaState.CorrelationId;

        public Guid ReportId => _reportSagaState.ReportId;

        public DateTime RequestTime => _reportSagaState.RequestTime;

        public string EMail => _reportSagaState.EMail;
    }
}
