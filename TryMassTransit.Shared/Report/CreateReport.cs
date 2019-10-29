using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Shared
{
    public interface CreateReport
    {
        Guid CorrelationId { get; set; }

        Guid ReportId { get; set; }

        DateTime RequestTime { get; set; }

        string EMail { get; set; }
    }
}
