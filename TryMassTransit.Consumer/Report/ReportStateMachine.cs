using Automatonymous;
using System;
using System.Collections.Generic;
using System.Text;
using TryMassTransit.Shared;

namespace TryMassTransit.Consumer
{
    public class ReportStateMachine : MassTransitStateMachine<ReportSagaState>
    {
        public State Created { get; private set; }

        public State Failed { get; private set; }

        //Events
        public Event<CreateReport> CreateReport { get; private set; }
        public Event<ReportRequestReceived> ReportRequestReceived { get; set; }
        public Event<ReportCreated> ReportCreated { get; set; }
        public Event<ReportRequestFailed> ReportRequestFailed { get; set; }

        public ReportStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => CreateReport, e => e.CorrelateById(c => c.ReportId, context => context.Message.ReportId).SelectId(context => Guid.NewGuid()));

            Event(() => ReportRequestReceived, e => e.CorrelateById(c => c.Message.CorrelationId));

            Event(() => ReportCreated, e => e.CorrelateById(c => c.Message.CorrelationId));

            Event(() => ReportRequestFailed, e => e.CorrelateById(c => c.Message.CorrelationId));

            Initially(
                When(CreateReport)
                    .Then(context =>
                    {
                        context.Instance.ReportId = context.Data.ReportId;
                        context.Instance.RequestTime = context.Data.RequestTime;
                        context.Instance.EMail = context.Data.EMail;

                        if (string.IsNullOrEmpty(context.Instance.EMail))
                            throw new ArgumentNullException();
                    })
                    .ThenAsync(async context =>
                    {
                        await Console.Out.WriteLineAsync($"CreateReportConsume: { context.Instance.CorrelationId }");
                    })
                    //.Activity()
                    .Publish(context => new ReportRequestReceivedEvent(context.Instance))
                    .Catch<ArgumentNullException>(context => context.Publish(x => new ReportRequestFailedEvent(x.Instance)).TransitionTo(Failed)),

                When(ReportRequestReceived)
                    .Then(context => 
                    {
                        context.Instance.ReportId = context.Data.ReportId;
                        context.Instance.RequestTime = context.Data.RequestTime;
                        context.Instance.EMail = context.Data.EMail;

                        Console.WriteLine($"ReportRequestReceived: Report is preparing: {context.Instance.CorrelationId }");
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"ReportRequestReceived: Report prepared: {context.Instance.CorrelationId }"))
                    .Publish(context => new ReportCreatedEvent(context.Instance))
                    .TransitionTo(Created)
            );

            During(Created,
                When(ReportCreated)
                    .Then(context =>
                    {
                        context.Instance.ReportId = context.Data.ReportId;
                        context.Instance.RequestTime = context.Data.RequestTime;
                        context.Instance.EMail = context.Data.EMail;

                        Console.WriteLine($"ReportCreated: Report is sending via EMail: {context.Instance.CorrelationId }");
                    })
                    .ThenAsync(context => Console.Out.WriteLineAsync($"ReportCreated: Report sent via EMail: {context.Instance.CorrelationId }"))
                    .Finalize()
            );

            During(Failed,
                When(ReportRequestFailed)
                    .Then(context =>
                    {
                        Console.WriteLine("ReportRequestFailed: Report failed:");
                    })
                    .Finalize()
            );

            SetCompletedWhenFinalized();
        }
    }
}
