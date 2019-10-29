using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Shared
{
    public interface ReportCreated
    {
        Guid CorrelationId { get; }

        Guid ReportId { get; }

        DateTime RequestTime { get; }

        string EMail { get; }
    }
}
