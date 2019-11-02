using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using MassTransit;
using System.Threading.Tasks;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using MassTransit.EntityFrameworkCoreIntegration.Saga;

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

                    services.AddDbContext<ReportSagaDbContext>(options =>
                        options.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Database=TryMassTransit;Integrated Security=True;", sqlOptions =>
                        {
                            sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                        })
                    );
                    services.AddSingleton<ISagaRepository<ReportSagaState>>(sp => 
                        EntityFrameworkSagaRepository<ReportSagaState>.CreateOptimistic(() => 
                            services.BuildServiceProvider().GetRequiredService<ReportSagaDbContext>()
                        )
                    );
                    //new InMemorySagaRepository<ReportSagaState>()

                    services.AddMassTransit(mt =>
                    {
                        mt.AddConsumer<MessageConsumer>();
                        mt.AddConsumer<GetMessagesConsumer>();
                        mt.AddSagaStateMachine<ReportStateMachine, ReportSagaState>();

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
                                //After the AddConsumer we can use like that
                                ep.ConfigureConsumer<MessageConsumer>(provider);
                            });

                            cfg.ReceiveEndpoint(host, "web-service-endpoint", ep =>
                            {
                                ep.ConfigureConsumer<GetMessagesConsumer>(provider);
                            });

                            cfg.ReceiveEndpoint(host, "web-service-request-endpoint", ep =>
                            {
                                //http://masstransit-project.com/MassTransit/advanced/sagas/persistence.html#publishing-and-sending-from-sagas
                                ep.UseInMemoryOutbox();

                                ep.ConfigureSaga<ReportSagaState>(provider);
                            });

                        }));
                    });

                    services.AddHostedService<BusService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
