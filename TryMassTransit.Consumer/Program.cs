using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using MassTransit;
using System.Threading.Tasks;

namespace TryMassTransit.Consumer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MessageDBContext>();

                    services.AddMassTransit(mt =>
                    {
                        mt.AddConsumer<MessageConsumer>();
                        mt.AddConsumer<GetMessagesConsumer>();

                        mt.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var host = cfg.Host(new Uri("rabbitmq://localhost"), hostConfigurator =>
                            {
                                hostConfigurator.Username("guest");
                                hostConfigurator.Password("guest");
                            });


                            cfg.ReceiveEndpoint(host, "first-masstransit-queue", ep =>
                            {
                                ep.ConfigureConsumer<MessageConsumer>(provider);

                            });

                            cfg.ReceiveEndpoint(host, "web-service-endpoint", ep =>
                            {
                                ep.ConfigureConsumer<GetMessagesConsumer>(provider);

                                Console.WriteLine(ep.InputAddress);
                            });

                        }));
                    });

                    services.AddSingleton<IHostedService, BusService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
