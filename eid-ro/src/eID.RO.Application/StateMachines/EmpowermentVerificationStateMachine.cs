using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service.Options;
using MassTransit;
using Microsoft.Extensions.Options;

namespace eID.RO.Application.StateMachines;

public class EmpowermentVerificationStateMachine : MassTransitStateMachine<EmpowermentVerificationState>
{
    private readonly ILogger<EmpowermentVerificationStateMachine> _logger;

    public Event<InitiateEmpowermentValidationProcess> InitiateEmpowermentValidationProcess { get; }
    public Schedule<EmpowermentVerificationState, EmpowermentValidationCheckValidityExpired> EmpowermentValidationCheckExpirationSchedule { get; }
    public Request<EmpowermentVerificationState, ChangeEmpowermentStatus, ServiceResult<bool>> ChangeEmpowermentStatusRequest { get; set; }
    public State CheckingUidsRestrictions { get; }
    public State ValidatingLegalEntityEmpowerment { get; }
    public Event<NoRestrictedUidsDetected> NoRestrictedUidsDetected { get; }
    public Event<RestrictedUidsDetected> RestrictedUidsDetected { get; }
    public Event<LegalEntityNotPresentInNTR> LegalEntityNotPresentInNTR { get; }
    public Event<LegalEntityBulstatCheckSucceeded> LegalEntityBulstatCheckSucceeded { get; }
    public Event<LegalEntityBulstatCheckFailed> LegalEntityBulstatCheckFailed { get; }
    public Event<LegalEntityEmpowermentValidated> LegalEntityEmpowermentValidated { get; }
    public Event<LegalEntityEmpowermentValidationFailed> LegalEntityEmpowermentValidationFailed { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EmpowermentVerificationStateMachine(ILogger<EmpowermentVerificationStateMachine> logger,
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
                                             IOptions<ApplicationOptions> applicationOptions)
    {
        #region Saga configurations
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(x => x.CurrentState);
        Event(() => InitiateEmpowermentValidationProcess,
            e =>
            {
                var guid = NewId.NextGuid();
                e.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId)
                    .SelectId(context => context.MessageId ?? guid);
                e.InsertOnInitial = true;
                e.SetSagaFactory(context =>
                {
                    return new EmpowermentVerificationState
                    {
                        CorrelationId = context.MessageId ?? guid,
                        OriginCorrelationId = context.Message.CorrelationId,
                        ReceivedDateTime = DateTime.UtcNow,
                        EmpowermentId = context.Message.EmpowermentId,
                        OnBehalfOf = context.Message.OnBehalfOf,
                        Uid = context.Message.Uid,
                        UidType = context.Message.UidType,
                        Name = context.Message.Name,
                        IssuerPosition = context.Message.IssuerPosition,
                        AuthorizerUids = context.Message.AuthorizerUids.ToList(),
                        EmpoweredUids = context.Message.EmpoweredUids,
                    };
                });
            }
        );

        Schedule(() => EmpowermentValidationCheckExpirationSchedule,
            st => st.EmpowermentValidationCheckExpirationTokenId,
            sc =>
            {
                sc.Received = r => r.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            }
        );

        Request(
            () => ChangeEmpowermentStatusRequest,
            x => x.CurrentRequestId,
            settings =>
            {
                // If we were unable to change the status the request should time out.
                settings.Timeout = TimeSpan.FromMinutes(5);
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
        Event(() => LegalEntityEmpowermentValidated, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityEmpowermentValidationFailed, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        #endregion

        AfterLeave(
            Initial,
            binder => binder
                        .Schedule(
                            EmpowermentValidationCheckExpirationSchedule,
                            context =>
                            {
                                return context.Init<EmpowermentValidationCheckValidityExpired>(new
                                {
                                    context.Saga.CorrelationId,
                                    context.Saga.EmpowermentId
                                });
                            },
                            context =>
                            {
                                DateTime now = DateTime.UtcNow;
                                DateTime target = now.Date.AddDays(1);
                                TimeSpan ttl = target - now;
                                return ttl;
                            }
                        )
        );

        Initially(
            When(InitiateEmpowermentValidationProcess)
            .IfElse(
                context => context.Saga.OnBehalfOf == OnBehalfOf.Individual,
                individual => individual
                    .PublishAsync(ctx => ctx.Init<CheckUidsRestrictions>(new
                    {
                        CorrelationId = ctx.Saga.OriginCorrelationId,
                        ctx.Saga.EmpowermentId,
                        // Combining all
                        Uids = ctx.Saga.EmpoweredUids
                                    .Union(ctx.Saga.AuthorizerUids)
                                    .Distinct(new Service.UserIdentifierEqualityComparer()),
                        RapidRetries = false, // Suppress warning for missing property
                        RespondWithRawServiceResult = false // Suppress warning for missing property
                    }))
                    .TransitionTo(CheckingUidsRestrictions),
                legalEntity => legalEntity.PublishAsync(ctx => ctx.Init<ValidateLegalEntityEmpowerment>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    ctx.Saga.AuthorizerUids
                }))
                .TransitionTo(ValidatingLegalEntityEmpowerment))
        );

        During(ValidatingLegalEntityEmpowerment,
            When(LegalEntityEmpowermentValidated)
                .PublishAsync(ctx => ctx.Init<CheckUidsRestrictions>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    // Combining all
                    Uids = ctx.Saga.EmpoweredUids
                                .Union(ctx.Saga.AuthorizerUids)
                                .Distinct(new Service.UserIdentifierEqualityComparer()),
                    RapidRetries = false, // Suppress warning for missing property
                    RespondWithRawServiceResult = false // Suppress warning for missing property
                }))
                .TransitionTo(CheckingUidsRestrictions),
            When(LegalEntityEmpowermentValidationFailed)
                .Then(context =>
                {
                    context.Saga.DenialReason = context.Message.DenialReason;
                })
                .Request(
                    ChangeEmpowermentStatusRequest,
                    ctx => ctx.Init<ChangeEmpowermentStatus>(new
                    {
                        CorrelationId = ctx.Saga.OriginCorrelationId,
                        ctx.Saga.EmpowermentId,
                        Status = EmpowermentStatementStatus.Denied,
                        ctx.Saga.DenialReason
                    })
                )
                .RequestStarted()
                .TransitionTo(ChangeEmpowermentStatusRequest.Pending),

            When(LegalEntityNotPresentInNTR)
                .IfElse
                (
                    ctx => ctx.Message.MissingOrMalformedResponse,
                    missingResponse => missingResponse.Then(ctx => ctx.SetCompleted()), // There's nothing we can do without this information.
                    notPresentInNTR => notPresentInNTR
                                    .PublishAsync(ctx => ctx.Init<CheckLegalEntityInBulstat>(new
                                    {
                                        CorrelationId = ctx.Saga.OriginCorrelationId,
                                        ctx.Saga.Uid,
                                        ctx.Saga.Name,
                                        ctx.Saga.EmpowermentId,
                                        IssuerUid = ctx.Saga.AuthorizerUids.FirstOrDefault(a => a.IsIssuer)?.Uid,
                                        IssuerUidType = ctx.Saga.AuthorizerUids.FirstOrDefault(a => a.IsIssuer)?.UidType,
                                        IssuerName = ctx.Saga.AuthorizerUids.FirstOrDefault(a => a.IsIssuer)?.Name,
                                        ctx.Saga.IssuerPosition
                                    }))
                ),
            When(LegalEntityBulstatCheckSucceeded)
                .PublishAsync(ctx => ctx.Init<CheckUidsRestrictions>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    // Combining all
                    Uids = ctx.Saga.EmpoweredUids
                                    .Union(ctx.Saga.AuthorizerUids)
                                    .Distinct(new Service.UserIdentifierEqualityComparer()),
                    RapidRetries = false, // Suppress warning for missing property
                    RespondWithRawServiceResult = false // Suppress warning for missing property
                }))
                .TransitionTo(CheckingUidsRestrictions),
            When(LegalEntityBulstatCheckFailed)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = ctx.Message.DenialReason;
                })
                .Request(
                    ChangeEmpowermentStatusRequest,
                    ctx => ctx.Init<ChangeEmpowermentStatus>(new
                    {
                        CorrelationId = ctx.Saga.OriginCorrelationId,
                        ctx.Saga.EmpowermentId,
                        Status = EmpowermentStatementStatus.Denied,
                        ctx.Saga.DenialReason
                    })
                )
                .RequestStarted()
                .TransitionTo(ChangeEmpowermentStatusRequest.Pending)
        );

        During(CheckingUidsRestrictions,
            When(NoRestrictedUidsDetected).Finalize(),
            When(RestrictedUidsDetected)
                .Then(ctx => ctx.Saga.DenialReason = ctx.Message.DenialReason)
                .Request(
                    ChangeEmpowermentStatusRequest,
                    ctx => ctx.Init<ChangeEmpowermentStatus>(new
                    {
                        CorrelationId = ctx.Saga.OriginCorrelationId,
                        ctx.Saga.EmpowermentId,
                        Status = EmpowermentStatementStatus.Denied,
                        ctx.Saga.DenialReason
                    })
                )
                .RequestStarted()
                .TransitionTo(ChangeEmpowermentStatusRequest.Pending)
        );
        During(ChangeEmpowermentStatusRequest.Pending,
            When(ChangeEmpowermentStatusRequest.Completed)
                .RequestCompleted()
                .If(
                    res => res.Message.StatusCode == System.Net.HttpStatusCode.OK && res.Message.Result,
                    binder => binder
                            .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                            {
                                CorrelationId = ctx.Saga.OriginCorrelationId,
                                Uids = ctx.Saga.AuthorizerUids.Cast<UserIdentifier>(),
                                ctx.Saga.EmpowermentId,
                                EventCode = Service.EventsRegistration.Events.EmpowermentDeclined.Code,
                                Service.EventsRegistration.Events.EmpowermentDeclined.Translations
                            }))
                            .ThenAsync(ctx => ctx.SetCompleted())
                )
                .Finalize(),
            When(ChangeEmpowermentStatusRequest.Faulted)
                .Retry(
                    r => r.Interval(3, TimeSpan.FromSeconds(5)),
                    binder => binder
                                .Request(
                                    ChangeEmpowermentStatusRequest,
                                    ctx => ctx.Init<ChangeEmpowermentStatus>(new
                                    {
                                        CorrelationId = ctx.Saga.OriginCorrelationId,
                                        ctx.Saga.EmpowermentId,
                                        Status = EmpowermentStatementStatus.Denied,
                                        ctx.Saga.DenialReason
                                    })
                                )
                                .RequestStarted()
                                .TransitionTo(ChangeEmpowermentStatusRequest.Pending)
                ).Finalize(),
            When(ChangeEmpowermentStatusRequest.TimeoutExpired)
                .Retry(
                    r => r.Interval(3, TimeSpan.FromSeconds(5)),
                    binder => binder
                                .Request(
                                    ChangeEmpowermentStatusRequest,
                                    ctx => ctx.Init<ChangeEmpowermentStatus>(new
                                    {
                                        CorrelationId = ctx.Saga.OriginCorrelationId,
                                        ctx.Saga.EmpowermentId,
                                        Status = EmpowermentStatementStatus.Denied,
                                        ctx.Saga.DenialReason
                                    })
                                )
                                .RequestStarted()
                                .TransitionTo(ChangeEmpowermentStatusRequest.Pending)
                ).Finalize()
            );
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        During(Final,
            When(EmpowermentValidationCheckExpirationSchedule.AnyReceived)
                .ThenAsync(ctx => ctx.SetCompleted())
        );
        DuringAny(
            Ignore(InitiateEmpowermentValidationProcess),
            When(EmpowermentValidationCheckExpirationSchedule.Received)
                .ThenAsync(ctx => ctx.SetCompleted())
    );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        WhenEnter(Final,
            binder => binder
                .Then(ctx => ctx.Saga.FinishedAt = DateTime.UtcNow)
        );
    }
}

public class EmpowermentVerificationStateMachineDefinition : SagaDefinition<EmpowermentVerificationState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<EmpowermentVerificationState> sagaConfigurator)
    {
        endpointConfigurator.PrefetchCount = 100;
        endpointConfigurator.UseInMemoryOutbox();

        var partition = endpointConfigurator.CreatePartitioner(ConcurrentMessageLimit ?? 16);

        sagaConfigurator.Message<InitiateEmpowermentValidationProcess>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<EmpowermentValidationCheckValidityExpired>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<NoRestrictedUidsDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<RestrictedUidsDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityNotPresentInNTR>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityBulstatCheckSucceeded>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityBulstatCheckFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityEmpowermentValidated>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityEmpowermentValidationFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
    }
}
