using MassTransit;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TryMassTransit.Api
{
    public class BusService : IHostedService
    {
        private readonly IBusControl _busControl;
        public BusService(IBusControl busControl)
        {
            _busControl = busControl;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.Out.WriteLineAsync("Bus started!");

            return _busControl.StartAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.Out.WriteLineAsync("Bus stopped!");

            return _busControl.StopAsync();
        }
    }
}
