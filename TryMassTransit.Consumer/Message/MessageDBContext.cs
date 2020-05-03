using System.Collections.Concurrent;
using System.Linq;

namespace TryMassTransit.Consumer
{
    public class MessageDBContext
    {
        private ConcurrentBag<Shared.Message> Messages = new ConcurrentBag<Shared.Message>();

        public void Add(Shared.Message message)
        {
            Messages.Add(message);
        }

        public IQueryable<Shared.Message> GetList()
        {
            return Messages.AsQueryable();
        }
    }
}
