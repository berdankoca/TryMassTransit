using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using MassTransit;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit.Saga;

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

                    var reportStateMachine = new ReportStateMachine();
                    var reportSagaRepository = new InMemorySagaRepository<ReportSagaState>();

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
                                // ep.UseMessageRetry(r => r.Interval(3, 10000)); //try to consume 3 times message after 10 seconds
                                ep.ConfigureConsumer<MessageConsumer>(provider);

                            });

                            cfg.ReceiveEndpoint(host, "web-service-endpoint", ep =>
                            {
                                ep.ConfigureConsumer<GetMessagesConsumer>(provider);
                            });

                            cfg.ReceiveEndpoint(host, "web-service-request-endpoint", ep =>
                            {
                                ep.StateMachineSaga(reportStateMachine, reportSagaRepository);
                            });

                        }));
                    });

                    services.AddSingleton<IHostedService, BusService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
