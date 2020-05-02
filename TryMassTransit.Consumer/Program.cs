using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using MassTransit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Serilog;
using Serilog.Events;

namespace TryMassTransit.Consumer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var buildConfiguration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
               .AddEnvironmentVariables()
               .Build();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            var builder = new HostBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddSerilog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MessageDBContext>();

                    services.AddMassTransit(mt =>
                    {
                        mt.AddConsumer<MessageConsumer>();
                        mt.AddConsumer<GetMessagesConsumer>();
                        mt.AddSagaStateMachine<ReportStateMachine, ReportSagaState>()
                            .EntityFrameworkRepository(r =>
                            {
                                r.ConcurrencyMode = MassTransit.EntityFrameworkCoreIntegration.ConcurrencyMode.Pessimistic;

                                r.AddDbContext<DbContext, ReportSagaDbContext>((provider, builder) =>
                                {
                                    builder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Database=TryMassTransit;Integrated Security=True;", m =>
                                    {
                                        m.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                                        m.MigrationsHistoryTable($"__{nameof(ReportSagaDbContext)}");
                                    });
                                });
                            });

                        mt.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                        {
                            var host = cfg.Host(new Uri("rabbitmq://localhost"), hostConfigurator =>
                            {
                                hostConfigurator.Username("guest");
                                hostConfigurator.Password("guest");
                            });


                            cfg.ReceiveEndpoint("first-masstransit-queue", ep =>
                            {
                                // ep.UseMessageRetry(r => r.Interval(3, 10000)); //try to consume 3 times message after 10 seconds
                                //After the AddConsumer we can use like that
                                ep.ConfigureConsumer<MessageConsumer>(provider);
                            });

                            cfg.ReceiveEndpoint("web-service-endpoint", ep =>
                            {
                                ep.ConfigureConsumer<GetMessagesConsumer>(provider);
                            });

                            cfg.ReceiveEndpoint("web-service-request-endpoint", ep =>
                            {
                                //http://masstransit-project.com/MassTransit/advanced/sagas/persistence.html#publishing-and-sending-from-sagas
                                ep.UseInMemoryOutbox();
                                //ep.
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
