using System;
using MassTransit;
using MassTransit.AspNetCoreIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TryMassTransit.Shared;

namespace TryMassTransit.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public ILoggerFactory LoggerFactory { get; }

        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            LoggerFactory = loggerFactory;    
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddMassTransit(mt =>
            {
                mt.AddRequestClient<GetMessages>();
                //Before the send a request we have to define the endpoint url which the consumer is listen
                EndpointConvention.Map<GetMessages>(new Uri("rabbitmq://localhost/web-service-endpoint"));

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

                        cfg.UseExtensionsLogging(LoggerFactory);
                    })
                );
            });

            services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BusService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            app.UseMvc();
        }
    }
}
