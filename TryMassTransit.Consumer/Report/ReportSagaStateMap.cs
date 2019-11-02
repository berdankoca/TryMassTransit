using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TryMassTransit.Consumer
{
    public class ReportSagaStateMap : IEntityTypeConfiguration<ReportSagaState>
    {
        public void Configure(EntityTypeBuilder<ReportSagaState> builder)
        {
            builder.Property(b => b.CorrelationId);
            builder.Property(b => b.CurrentState)
                .HasMaxLength(200);
            builder.Property(b => b.ReportId);
            builder.Property(b => b.RequestTime);
            builder.Property(b => b.EMail)
                .HasMaxLength(200);

            builder.ToTable("sagaReportSagaState");
        }
    }
}
