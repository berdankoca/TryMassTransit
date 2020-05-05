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
using Microsoft.Extensions.DependencyInjection.Extensions;
using MassTransit.Definition;

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
                    services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
                    services.AddSingleton<MessageDBContext>();

                    //If the automatonymous activity need to create new instance via constructor injection we have to register it
                    services.AddScoped<PublishReportCreatedActivity>();
                    
                    services.AddMassTransit(mt =>
                    {
                        mt.AddConsumersFromNamespaceContaining<MessageConsumer>();
                        //mt.AddConsumer<GetMessagesConsumer>();

                        mt.AddSagaStateMachine<ReportStateMachine, ReportSagaState>(typeof(ReportStateMachineDefinition))
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

                            cfg.ConfigureEndpoints(provider);

                        }));
                    });

                    services.AddHostedService<BusService>();
                });

            await builder.RunConsoleAsync();
        }
    }
}
