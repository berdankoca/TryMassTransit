using System;
using MassTransit;
using MassTransit.Definition;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TryMassTransit.Shared;
using MassTransit.MultiBus;
using TryMassTransit.Api.Contracts;
using Microsoft.VisualBasic;
using MassTransit.Mediator;
using MassTransit.Registration;
using MassTransit.Context;

namespace TryMassTransit.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit<IRabbitMQBus>(mt => 
            {
                mt.AddRequestClient<GetMessages>();
                //Before the send a request we have to define the endpoint url which the consumer is listen
                //EndpointConvention.Map<GetMessages>(new Uri("rabbitmq://localhost/web-service-endpoint"));

                EndpointConvention.Map<CreateReport>(new Uri("rabbitmq://localhost/report-saga-state"));

                //When you run first time, consumer have to be started before publisher.    
                //Because exchanges and queue have to be bind each other.
                //And the ReceiveEndpoint do that
                mt.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                    {
                        var host = cfg.Host(new Uri("rabbitmq://localhost")
                            , hostConfigurator =>
                            {
                                hostConfigurator.Username("guest");
                                hostConfigurator.Password("guest");
                            }
                        );

                        cfg.ConfigureEndpoints(provider);
                    })
                );
            });

            services.AddMassTransit<IDomainEventBus>(mt =>
            {
                mt.AddRequestClient<OrderUpdating>();
             
                mt.AddConsumer<Shipment>();

                IMediator MediatorFactory(IServiceProvider serviceProvider)
                {
                    //var provider = serviceProvider.GetRequiredService<IConfigurationServiceProvider>();

                    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                    if (loggerFactory != null)
                        LogContext.ConfigureCurrentLogContext(loggerFactory);

                    return Bus.Factory.CreateMediator(cfg =>
                    {
                        var registrationInstance = serviceProvider.GetRequiredService<Bind<IDomainEventBus, IRegistration>>();

                        registrationInstance.Value.ConfigureConsumers(cfg);
                        registrationInstance.Value.ConfigureSagas(cfg);
                    });
                }

                
                services.TryAddSingleton(MediatorFactory);
                services.AddSingleton(provider => Bind<IDomainEventBus>.Create<IClientFactory>(provider.GetRequiredService<IMediator>()));

            });

            //services.AddSingleton<IHostedService, BusService>();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            logger.LogDebug("Deneme");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
