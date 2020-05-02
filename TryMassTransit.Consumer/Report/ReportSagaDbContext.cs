using MassTransit.EntityFrameworkCoreIntegration;
using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Collections.Generic;

namespace TryMassTransit.Consumer
{
    public class ReportSagaDbContext
        : SagaDbContext
    {
        public ReportSagaDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new ReportSagaStateMap(); }
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
