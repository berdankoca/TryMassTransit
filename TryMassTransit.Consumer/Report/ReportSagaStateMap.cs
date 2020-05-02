using MassTransit.EntityFrameworkCoreIntegration.Mappings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TryMassTransit.Consumer
{
    public class ReportSagaStateMap : SagaClassMap<ReportSagaState>
    {
        protected override void Configure(EntityTypeBuilder<ReportSagaState> entity, ModelBuilder model)
        {
            base.Configure(entity, model);

            entity.Property(b => b.CorrelationId);
            entity.Property(b => b.CurrentState)
                .HasMaxLength(200);
            entity.Property(b => b.ReportId);
            entity.Property(b => b.RequestTime);
            entity.Property(b => b.EMail)
                .HasMaxLength(200);

            entity.ToTable("sagaReportSagaState");
        }
    }
}
