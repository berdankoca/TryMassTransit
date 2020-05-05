using Automatonymous;
using GreenPipes;
using MassTransit;
using System;
using System.Threading.Tasks;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class PublishReportCreatedActivity
        : Activity<ReportSagaState, ReportCreated>
    {
        private readonly ConsumeContext _context;

        public PublishReportCreatedActivity(ConsumeContext context)
        {
            _context = context;
        }
        public void Probe(ProbeContext context)
        {
            context.CreateScope("publish-report-created");
        }

        public void Accept(StateMachineVisitor visitor)
        {
            visitor.Visit(this);
        }

        public async Task Execute(BehaviorContext<ReportSagaState, ReportCreated> context, Behavior<ReportSagaState, ReportCreated> next)
        {
            Console.WriteLine($"execute publish report created activity: {context.Instance.CorrelationId}");

            Console.WriteLine($"ReportCreated: Report is sending via EMail: {context.Instance.CorrelationId}");

            Console.WriteLine($"ReportCreated: Report sent via EMail: {context.Instance.CorrelationId}");

            //_context.Publish<X>

            await next.Execute(context).ConfigureAwait(false);
        }

        public async Task Faulted<TException>(BehaviorExceptionContext<ReportSagaState, ReportCreated, TException> context, Behavior<ReportSagaState, ReportCreated> next) where TException : Exception
        {
            Console.WriteLine($"faulted publish report created activity {context.Instance.CorrelationId}");

            await next.Faulted(context).ConfigureAwait(false);
        }

    }
}
