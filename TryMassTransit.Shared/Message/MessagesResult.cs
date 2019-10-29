using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Shared
{
    public interface MessagesResult
    {

        List<Message> Messages { get; set; }
    }
}
