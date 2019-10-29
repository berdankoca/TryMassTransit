using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Shared
{
    public interface ReportRequestReceived
    {
        Guid CorrelationId { get; }

        Guid ReportId { get; }

        DateTime RequestTime { get; }

        string EMail { get; }
    }
}
