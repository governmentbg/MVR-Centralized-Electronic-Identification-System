using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace eID.RO.Application.StateMachines;

public class SignaturesCollectionStateMachine :
    MassTransitStateMachine<SignaturesCollectionState>
{
    private readonly ILogger<SignaturesCollectionStateMachine> _logger;

    public Event<CollectAuthorizerSignatures> InitiateSignatureCollectionProcess { get; }
    public Event<EmpowermentSigned> EmpowermentSigned { get; }
    public State CollectingSignatures { get; }
    public Schedule<SignaturesCollectionState, SignatureCollectionTimedOut> SignatureCollectionTimedOut { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SignaturesCollectionStateMachine(ILogger<SignaturesCollectionStateMachine> logger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(x => x.CurrentState);
        Event(() => InitiateSignatureCollectionProcess,
                e =>
                {
                    var guid = NewId.NextGuid();
                    e.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId)
                        .SelectId(context => context.MessageId ?? guid);
                    e.InsertOnInitial = true;
                    e.SetSagaFactory(context =>
                    {
                        return new SignaturesCollectionState
                        {
                            OriginCorrelationId = context.Message.CorrelationId,
                            CorrelationId = context.MessageId ?? guid,
                            ReceivedDateTime = DateTime.UtcNow,
                            EmpowermentId = context.Message.EmpowermentId,
                            AuthorizerUids = context.Message.AuthorizerUids,
                            SignaturesCollectionDeadline = context.Message.SignaturesCollectionDeadline
                        };
                    });
                });

        Schedule(
            () => SignatureCollectionTimedOut,
            x => x.SignatureCollectionTimeoutTokenId,
            x =>
            {
                x.Received = r => r.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            }
        );
        Event(() => EmpowermentSigned, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });

        Initially(
                When(InitiateSignatureCollectionProcess)
                .Schedule(
                    SignatureCollectionTimedOut,
                    context =>
                    {
                        return context.Init<SignatureCollectionTimedOut>(new
                        {
                            context.Saga.CorrelationId,
                            context.Saga.EmpowermentId
                        });
                    },
                    context => context.Saga.SignaturesCollectionDeadline
                )
                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uids = ctx.Saga.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
                    ctx.Saga.EmpowermentId,
                    EventCode = Service.EventsRegistration.Events.EmpowermentNeedsSignature.Code,
                    Service.EventsRegistration.Events.EmpowermentNeedsSignature.Translations
                }))
                .TransitionTo(CollectingSignatures)
            );

        During(
            CollectingSignatures,
            When(EmpowermentSigned)
                .Then(ctx =>
                {
                    if (ctx.Saga.AuthorizerUids.Any(au => au.Uid == ctx.Message.SignerUid && au.UidType == ctx.Message.SignerUidType))
                    {
                        ctx.Saga.SignedUids.Add(new UserIdentifierWithNameData { Name = ctx.Message.SignerName, Uid = ctx.Message.SignerUid, UidType = ctx.Message.SignerUidType });
                    }
                })
                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uids = ctx.Saga.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
                    ctx.Saga.EmpowermentId,
                    EventCode = Service.EventsRegistration.Events.EmpowermentSigned.Code,
                    Service.EventsRegistration.Events.EmpowermentSigned.Translations
                }))
                .If(
                    check => !check.Saga.AuthorizerUids.Except(check.Saga.SignedUids, new UserIdentifierWithNamesEqualityComparer()).Any(),
                    allSignaturesCollected => allSignaturesCollected.Finalize()
                )
        );


#pragma warning disable CS8602 // Dereference of a possibly null reference.
        DuringAny(
            When(SignatureCollectionTimedOut.Received)
            .PublishAsync(ctx => ctx.Init<NotifyUids>(new
            {
                CorrelationId = ctx.Saga.OriginCorrelationId,
                Uids = ctx.Saga.AuthorizerUids.Select(x => new UserIdentifierData { Uid = x.Uid, UidType = x.UidType }),
                ctx.Saga.EmpowermentId,
                EventCode = Service.EventsRegistration.Events.EmpowermentTimeout.Code,
                Service.EventsRegistration.Events.EmpowermentTimeout.Translations
            }))
            .Finalize()
            );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        BeforeEnter(Final,
                binder => binder
                    .IfElse(
                        check => !check.Saga.AuthorizerUids.Except(check.Saga.SignedUids, new UserIdentifierWithNamesEqualityComparer()).Any(),
                        everyoneSigned => everyoneSigned
                                .PublishAsync(ctx => ctx.Init<SignaturesCollected>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    ctx.Saga.EmpowermentId
                                })),
                        missingSignatures => missingSignatures
                                .PublishAsync(ctx => ctx.Init<SignatureCollectionFailed>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    ctx.Saga.EmpowermentId
                                }))
                    )
            );

        WhenEnter(Final,
            binder => binder
                .Unschedule(SignatureCollectionTimedOut)
        );
        SetCompletedWhenFinalized();
    }
}
public class SignatureCollectionStateMachineDefinition :
    SagaDefinition<SignaturesCollectionState>
{

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<SignaturesCollectionState> sagaConfigurator)
    {
        endpointConfigurator.PrefetchCount = 100;
        //endpointConfigurator.UseMessageRetry(r =>
        //{
        //    r.Incremental(5, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(20));
        //    r.Handle<DbUpdateConcurrencyException>();
        //    r.Handle<SagaException>(e => e.Message.Contains("An existing saga instance was not found"));
        //});
        endpointConfigurator.UseInMemoryOutbox();

        var partition = endpointConfigurator.CreatePartitioner(ConcurrentMessageLimit ?? 16);
        sagaConfigurator.Message<CollectAuthorizerSignatures>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<EmpowermentSigned>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
    }
}
