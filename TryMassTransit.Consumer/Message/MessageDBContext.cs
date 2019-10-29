using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class MessageDBContext
    {
        private ConcurrentBag<Message> Messages = new ConcurrentBag<Message>();

        public void Add(Message message)
        {
            Messages.Add(message);
        }

        public IQueryable<Message> GetList()
        {
            return Messages.AsQueryable();
        }
    }
}
