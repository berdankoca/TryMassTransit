using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Consumer
{
    public class ReportSagaDbContext : SagaDbContext<ReportSagaState, ReportSagaStateMap>
    {
        public ReportSagaDbContext(DbContextOptions options) : base(options)
        {

        }
    }

    public class ReportSagaDbContextFactory : IDesignTimeDbContextFactory<ReportSagaDbContext>
    {
        public ReportSagaDbContext CreateDbContext(string[] args)
        {
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json")
            //.Build();
            var optionsBuilder = new DbContextOptionsBuilder<ReportSagaDbContext>();
            optionsBuilder.UseSqlServer("Data Source=(LocalDb)\\MSSQLLocalDB;Database=TryMassTransit;Integrated Security=True;");

            return new ReportSagaDbContext(optionsBuilder.Options);
        }
    }
}
