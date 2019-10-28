using System;

namespace TryMassTransit.Shared
{
    public interface Message
    {
        string Text { get; set; }
    }

}