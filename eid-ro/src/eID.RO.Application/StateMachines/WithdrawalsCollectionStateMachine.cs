using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace eID.RO.Application.StateMachines;

public class WithdrawalsCollectionStateMachine : MassTransitStateMachine<WithdrawalsCollectionState>
{
    private readonly ILogger<WithdrawalsCollectionStateMachine> _logger;
    private readonly ApplicationOptions _applicationOptions;

    public Event<InitiateEmpowermentWithdrawalProcess> InitiateEmpowermentWithdrawalProcess { get; }
    public Schedule<WithdrawalsCollectionState, WithdrawalsCollectionTimedOut> WithdrawalsCollectionTimedOutSchedule { get; }
    public State CheckingUidsRestrictions { get; }
    public State WithdrawalIsCompleted { get; }
    public State VerifyingLegalEntityWithdrawalRequester { get; }
    public State WithdrawalIsDenied { get; }
    public State TimestampingWithdrawal { get; }
    public Event<NoRestrictedUidsDetected> NoRestrictedUidsDetected { get; }
    public Event<RestrictedUidsDetected> RestrictedUidsDetected { get; }
    public Event<VerifyLegalEntityWithdrawalRequesterSucceeded> VerifyLegalEntityWithdrawalRequesterSucceeded { get; }
    public Event<VerifyLegalEntityWithdrawalRequesterFailed> VerifyLegalEntityWithdrawalRequesterFailed { get; }
    public Event<LegalEntityNotPresentInNTR> LegalEntityNotPresentInNTR { get; }
    public Event<LegalEntityBulstatCheckSucceeded> LegalEntityBulstatCheckSucceeded { get; }
    public Event<LegalEntityBulstatCheckFailed> LegalEntityBulstatCheckFailed { get; }
    public Event<TimestampEmpowermentWithdrawalSucceeded> TimestampEmpowermentWithdrawalSucceeded { get; }
    public Event<TimestampEmpowermentWithdrawalFailed> TimestampEmpowermentWithdrawalFailed { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public WithdrawalsCollectionStateMachine(ILogger<WithdrawalsCollectionStateMachine> logger,
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                                             IOptions<ApplicationOptions> applicationOptions)
    {
        #region Saga configurations
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationOptions = (applicationOptions ?? throw new ArgumentNullException(nameof(applicationOptions))).Value;
        _applicationOptions.Validate();

        InstanceState(x => x.CurrentState);
        Event(() => InitiateEmpowermentWithdrawalProcess,
            e =>
            {
                var guid = NewId.NextGuid();
                e.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId)
                    .SelectId(context => context.MessageId ?? guid);
                e.InsertOnInitial = true;
                e.SetSagaFactory(context =>
                {
                    var deadline = DateTime.UtcNow.AddDays(_applicationOptions.CollectResolutionsDeadlinePeriodInDays);
                    if (context.Message.WithdrawalsCollectionsDeadline.HasValue && context.Message.WithdrawalsCollectionsDeadline.Value < deadline)
                    {
                        deadline = context.Message.WithdrawalsCollectionsDeadline.Value;
                    }

                    return new WithdrawalsCollectionState
                    {
                        CorrelationId = context.MessageId ?? guid,
                        OriginCorrelationId = context.Message.CorrelationId,
                        ReceivedDateTime = DateTime.UtcNow,
                        WithdrawalsCollectionsDeadline = deadline,
                        IssuerUid = context.Message.IssuerUid,
                        IssuerUidType = context.Message.IssuerUidType,
                        Reason = context.Message.Reason,
                        EmpowermentId = context.Message.EmpowermentId,
                        OnBehalfOf = context.Message.OnBehalfOf,
                        AuthorizerUids = context.Message.AuthorizerUids,
                        EmpoweredUids = context.Message.EmpoweredUids,
                        EmpowermentWithdrawalId = context.Message.EmpowermentWithdrawalId,
                        LegalEntityUid = context.Message.LegalEntityUid,
                        LegalEntityName = context.Message.LegalEntityName,
                        IssuerName = context.Message.IssuerName,
                        IssuerPosition = context.Message.IssuerPosition
                    };
                });
            }
        );

        Schedule(() => WithdrawalsCollectionTimedOutSchedule,
            st => st.WithdrawalsCollectionTimeoutTokenId,
            sc =>
            {
                sc.Received = r => r.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            }
        );

        Event(() => NoRestrictedUidsDetected, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => RestrictedUidsDetected, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => VerifyLegalEntityWithdrawalRequesterSucceeded, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => VerifyLegalEntityWithdrawalRequesterFailed, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityNotPresentInNTR, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityBulstatCheckSucceeded, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityBulstatCheckFailed, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => TimestampEmpowermentWithdrawalSucceeded, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId && state.EmpowermentWithdrawalId == context.Message.EmpowermentWithdrawalId);
            ec.OnMissingInstance(m => m.Discard());
        });
        Event(() => TimestampEmpowermentWithdrawalFailed, ec =>
        {
            ec.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId && state.EmpowermentWithdrawalId == context.Message.EmpowermentWithdrawalId);
            ec.OnMissingInstance(m => m.Discard());
        });
        #endregion

        Initially(
                When(InitiateEmpowermentWithdrawalProcess)
                .PublishAsync(ctx => ctx.Init<TimestampEmpowermentWithdrawal>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    ctx.Saga.EmpowermentWithdrawalId
                }))
                .TransitionTo(TimestampingWithdrawal)
        );

        During(TimestampingWithdrawal,
            When(TimestampEmpowermentWithdrawalSucceeded)
                .IfElse(check => check.Saga.OnBehalfOf == OnBehalfOf.LegalEntity,
                    legalEntity => legalEntity
                        .PublishAsync(ctx => ctx.Init<VerifyLegalEntityWithdrawalRequester>(new
                        {
                            CorrelationId = ctx.Saga.OriginCorrelationId,
                            ctx.Saga.EmpowermentId,
                            ctx.Saga.IssuerName,
                            ctx.Saga.IssuerUid,
                            ctx.Saga.IssuerUidType
                        }))
                        .TransitionTo(VerifyingLegalEntityWithdrawalRequester),
                    individual => individual
                        .PublishAsync(ctx => ctx.Init<CheckUidsRestrictions>(new
                        {
                            CorrelationId = ctx.Saga.OriginCorrelationId,
                            ctx.Saga.EmpowermentId,
                            Uids = ctx.Saga.EmpoweredUids
                                        .Union(ctx.Saga.AuthorizerUids)
                                        .Distinct(new Service.UserIdentifierEqualityComparer()),
                            RapidRetries = false, // Suppress warning for missing property
                            RespondWithRawServiceResult = false // Suppress warning for missing property
                        }))
                        .TransitionTo(CheckingUidsRestrictions)
                )
                .Schedule(
                    WithdrawalsCollectionTimedOutSchedule,
                    context =>
                    {
                        return context.Init<WithdrawalsCollectionTimedOut>(new
                        {
                            context.Saga.CorrelationId,
                            context.Saga.EmpowermentId
                        });
                    },
                    context => context.Saga.WithdrawalsCollectionsDeadline
                ),
            When(TimestampEmpowermentWithdrawalFailed)
                .TransitionTo(WithdrawalIsDenied)
        );

        During(VerifyingLegalEntityWithdrawalRequester,
            When(VerifyLegalEntityWithdrawalRequesterSucceeded)
                .PublishAsync(ctx => ctx.Init<CheckUidsRestrictions>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    // We check only withdrawal requester for restrictions
                    Uids = new[] { new UserIdentifierData { Uid = ctx.Saga.IssuerUid, UidType = ctx.Saga.IssuerUidType } },
                    RapidRetries = false, // Suppress warning for missing property
                    RespondWithRawServiceResult = false // Suppress warning for missing property
                }))
                .TransitionTo(CheckingUidsRestrictions),
            When(VerifyLegalEntityWithdrawalRequesterFailed)
                .TransitionTo(WithdrawalIsDenied),
            When(LegalEntityNotPresentInNTR)
                .PublishAsync(ctx => ctx.Init<CheckLegalEntityInBulstat>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uid = ctx.Saga.LegalEntityUid,
                    Name = ctx.Saga.LegalEntityName,
                    ctx.Saga.EmpowermentId,
                    ctx.Saga.IssuerUid,
                    ctx.Saga.IssuerUidType,
                    ctx.Saga.IssuerName,
                    ctx.Saga.IssuerPosition
                })),
            When(LegalEntityBulstatCheckSucceeded)
                .PublishAsync(ctx => ctx.Init<CheckUidsRestrictions>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    // We check only withdrawal requester for restrictions
                    Uids = new[] { new UserIdentifierData { Uid = ctx.Saga.IssuerUid, UidType = ctx.Saga.IssuerUidType } },
                    RapidRetries = false, // Suppress warning for missing property
                    RespondWithRawServiceResult = false // Suppress warning for missing property
                }))
                .TransitionTo(CheckingUidsRestrictions),
            When(LegalEntityBulstatCheckFailed)
                .TransitionTo(WithdrawalIsDenied)

        );

        During(CheckingUidsRestrictions,
            When(NoRestrictedUidsDetected)
                .TransitionTo(WithdrawalIsCompleted),
            When(RestrictedUidsDetected)
                .TransitionTo(WithdrawalIsDenied)
        );

        WhenEnter(WithdrawalIsDenied,
            binder => binder
                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentWithdrawalStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentWithdrawalId,
                    Status = EmpowermentWithdrawalStatus.Denied
                }))
                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uids = ctx.Saga.AuthorizerUids.AsEnumerable(),
                    ctx.Saga.EmpowermentId,
                    EventCode = Service.EventsRegistration.Events.WithdrawalDeclined.Code,
                    Service.EventsRegistration.Events.WithdrawalDeclined.Translations
                }))
                .Finalize()
        );

        WhenEnter(WithdrawalIsCompleted,
            binder => binder
                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentWithdrawalStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentWithdrawalId,
                    Status = EmpowermentWithdrawalStatus.Completed
                }))
                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    Status = EmpowermentStatementStatus.Withdrawn,
                    DenialReason = EmpowermentsDenialReason.None // Suppress warning for missing property
                }))
                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uids = ctx.Saga.AuthorizerUids.AsEnumerable(),
                    ctx.Saga.EmpowermentId,
                    EventCode = Service.EventsRegistration.Events.EmpowermentWasWithdrawn.Code,
                    Service.EventsRegistration.Events.EmpowermentWasWithdrawn.Translations
                }))
                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uids = ctx.Saga.EmpoweredUids.AsEnumerable(),
                    ctx.Saga.EmpowermentId,
                    EventCode = Service.EventsRegistration.Events.EmpowermentToMeWasWithdrawn.Code,
                    Service.EventsRegistration.Events.EmpowermentToMeWasWithdrawn.Translations
                }))
                .PublishAsync(ctx => ctx.Init<EmpowermentIsWithdrawn>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId
                }))
                .Finalize()
            );

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        DuringAny(
            When(WithdrawalsCollectionTimedOutSchedule.Received)
                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentWithdrawalStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentWithdrawalId,
                    Status = EmpowermentWithdrawalStatus.Timeout
                }))
                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    Uids = ctx.Saga.AuthorizerUids.AsEnumerable(),
                    ctx.Saga.EmpowermentId,
                    EventCode = Service.EventsRegistration.Events.WithdrawalTimeout.Code,
                    Service.EventsRegistration.Events.WithdrawalTimeout.Translations
                }))
                .Finalize()
        );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        WhenEnter(Final,
            binder => binder
                .Unschedule(WithdrawalsCollectionTimedOutSchedule)
        );

        SetCompletedWhenFinalized();
    }
}

public class WithdrawalsCollectionStateMachineDefinition : SagaDefinition<WithdrawalsCollectionState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<WithdrawalsCollectionState> sagaConfigurator)
    {
        endpointConfigurator.PrefetchCount = 100;
        endpointConfigurator.UseInMemoryOutbox();

        var partition = endpointConfigurator.CreatePartitioner(ConcurrentMessageLimit ?? 16);

        sagaConfigurator.Message<InitiateEmpowermentWithdrawalProcess>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<WithdrawalsCollectionTimedOut>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<NoRestrictedUidsDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<RestrictedUidsDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<VerifyLegalEntityWithdrawalRequesterSucceeded>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<VerifyLegalEntityWithdrawalRequesterFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityNotPresentInNTR>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityBulstatCheckSucceeded>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityBulstatCheckFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<TimestampEmpowermentWithdrawalSucceeded>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentWithdrawalId));
        sagaConfigurator.Message<TimestampEmpowermentWithdrawalFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentWithdrawalId));
    }
}
