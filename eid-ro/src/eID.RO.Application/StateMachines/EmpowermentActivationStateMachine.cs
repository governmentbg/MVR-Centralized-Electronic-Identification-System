using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using MassTransit;

namespace eID.RO.Application.StateMachines;

public class EmpowermentActivationStateMachine :
    MassTransitStateMachine<EmpowermentActivationState>
{
    private readonly ILogger<EmpowermentActivationStateMachine> _logger;
    private const int ActivationDeadlinePeriodInDays = 7; // This may be moved to configuration if deemed necessary

    public Event<InitiateEmpowermentActivationProcess> InitiateEmpowermentActivationProcess { get; }
    public Schedule<EmpowermentActivationState, EmpowermentActivationTimedOut> EmpowermentActivationTimedOut { get; }
    public State VerifyingAllUidsAboveLawfulAge { get; }
    public State CheckingDataInNTR { get; }
    public State CollectingAuthorizerSignatures { get; }
    public State CheckingUidsRestrictions { get; }
    public State VerifyUidsRegistrationStatus { get; }
    public State ValidatingLegalEntityEmpowerment { get; }
    public State TimestampingEmpowermentXml { get; }
    public Event<NoBelowLawfulAgeDetected> NoBelowLawfulAgeDetected { get; }
    public Event<BelowLawfulAgeDetected> BelowLawfulAgeDetected { get; }
    public Event<LegalEntityNTRCheckSucceeded> LegalEntityNTRCheckSucceeded { get; }
    public Event<LegalEntityNTRCheckFailed> LegalEntityNTRCheckFailed { get; }
    public Event<LegalEntityNotPresentInNTR> LegalEntityNotPresentInNTR { get; }
    public Event<SignaturesCollected> SignaturesCollected { get; }
    public Event<SignatureCollectionFailed> SignatureCollectionFailed { get; }
    public Event<NoRestrictedUidsDetected> NoRestrictedUidsDetected { get; }
    public Event<RestrictedUidsDetected> RestrictedUidsDetected { get; }
    public Event<LawfulAgeInfoNotAvailable> LawfulAgeInfoNotAvailable { get; }
    public Event<LegalEntityEmpowermentValidated> LegalEntityEmpowermentValidated { get; }
    public Event<LegalEntityEmpowermentValidationFailed> LegalEntityEmpowermentValidationFailed { get; }
    public Event<LegalEntityBulstatCheckSucceeded> LegalEntityBulstatCheckSucceeded { get; }
    public Event<LegalEntityBulstatCheckFailed> LegalEntityBulstatCheckFailed { get; }
    public Event<TimestampEmpowermentXmlSucceeded> TimestampEmpowermentXmlSucceeded { get; }
    public Event<TimestampEmpowermentXmlFailed> TimestampEmpowermentXmlFailed { get; }
    public Event<RegistrationStatusAllAvailable> RegistrationStatusAllAvailable { get; }
    public Event<InvalidRegistrationStatusDetected> InvalidRegistrationStatusDetected { get; }
    public Event<RegistrationStatusInfoNotAvailable> RegistrationStatusInfoNotAvailable { get; }
    public Event<EmpowermentIsWithdrawn> EmpowermentIsWithdrawn { get; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EmpowermentActivationStateMachine(ILogger<EmpowermentActivationStateMachine> logger)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InstanceState(x => x.CurrentState);
        Event(() => InitiateEmpowermentActivationProcess,
                e =>
                {
                    var guid = NewId.NextGuid();
                    e.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId)
                        .SelectId(context => context.MessageId ?? guid);
                    e.InsertOnInitial = true;
                    e.SetSagaFactory(context =>
                    {
                        var deadline = DateTime.UtcNow.AddDays(ActivationDeadlinePeriodInDays);
                        // Use empowerment's expiry date as deadline when its closer than the default deadline period
                        if (context.Message.ExpiryDate.HasValue && context.Message.ExpiryDate.Value < deadline)
                        {
                            deadline = context.Message.ExpiryDate.Value;
                        }

                        return new EmpowermentActivationState
                        {
                            OriginCorrelationId = context.Message.CorrelationId,
                            CorrelationId = context.MessageId ?? guid,
                            ReceivedDateTime = DateTime.UtcNow,
                            EmpowermentId = context.Message.EmpowermentId,
                            OnBehalfOf = context.Message.OnBehalfOf,
                            Uid = context.Message.Uid,
                            UidType = context.Message.UidType,
                            Name = context.Message.Name,
                            IssuerName = context.Message.IssuerName,
                            IssuerPosition = context.Message.IssuerPosition,
                            AuthorizerUids = context.Message.AuthorizerUids.ToList(),
                            EmpoweredUids = context.Message.EmpoweredUids,
                            ActivationDeadline = deadline
                        };
                    });
                });

        Event(() => SignaturesCollected, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => SignatureCollectionFailed, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => NoRestrictedUidsDetected, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => RestrictedUidsDetected, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityNTRCheckSucceeded, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityNTRCheckFailed, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityNotPresentInNTR, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => NoBelowLawfulAgeDetected, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => BelowLawfulAgeDetected, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => LawfulAgeInfoNotAvailable, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
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
        Event(() => LegalEntityBulstatCheckSucceeded, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => LegalEntityBulstatCheckFailed, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => TimestampEmpowermentXmlSucceeded, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => TimestampEmpowermentXmlFailed, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => RegistrationStatusAllAvailable, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => InvalidRegistrationStatusDetected, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => RegistrationStatusInfoNotAvailable, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });
        Event(() => EmpowermentIsWithdrawn, x =>
        {
            x.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            x.OnMissingInstance(m => m.Discard());
        });

        Schedule(
            () => EmpowermentActivationTimedOut,
            x => x.EmpowermentActivationTimeoutTokenId,
            x =>
            {
                x.Received = r => r.CorrelateBy((state, context) => state.EmpowermentId == context.Message.EmpowermentId);
            }
        );

        Initially(
                When(InitiateEmpowermentActivationProcess)
                .PublishAsync(ctx => ctx.Init<TimestampEmpowermentXml>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId
                }))
                .TransitionTo(TimestampingEmpowermentXml)
            );

        AfterLeave(
            Initial,
            binder => binder
                        .Schedule(
                            EmpowermentActivationTimedOut,
                            context =>
                            {
                                return context.Init<EmpowermentActivationTimedOut>(new
                                {
                                    context.Saga.CorrelationId,
                                    context.Saga.EmpowermentId
                                });
                            },
                            context => context.Saga.ActivationDeadline
                        ));

        During(TimestampingEmpowermentXml,
            When(TimestampEmpowermentXmlSucceeded)
                .PublishAsync(ctx => ctx.Init<VerifyUidsLawfulAge>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    // Combining all
                    Uids = ctx.Saga
                                .EmpoweredUids
                                .Union(ctx.Saga.AuthorizerUids)
                                .Distinct(new Service.UserIdentifierEqualityComparer())
                }))
                .TransitionTo(VerifyingAllUidsAboveLawfulAge),
            When(TimestampEmpowermentXmlFailed)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = EmpowermentsDenialReason.UnsuccessfulTimestamping;
                })
                .Finalize()
        );


        During(VerifyingAllUidsAboveLawfulAge,
                    When(NoBelowLawfulAgeDetected)
                    .IfElse(
                        context => context.Saga.OnBehalfOf == OnBehalfOf.Individual,
                        individual => individual
                                .PublishAsync(ctx => ctx.Init<CollectAuthorizerSignatures>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    ctx.Saga.AuthorizerUids,
                                    ctx.Saga.EmpowermentId,
                                    SignaturesCollectionDeadline = ctx.Saga.ActivationDeadline
                                }))
                                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentStatus>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    ctx.Saga.EmpowermentId,
                                    Status = EmpowermentStatementStatus.CollectingAuthorizerSignatures,
                                    DenialReason = EmpowermentsDenialReason.None // Suppress warning for missing property
                                }))
                                .TransitionTo(CollectingAuthorizerSignatures),
                        legalEntity =>
                            legalEntity
                                .PublishAsync(ctx => ctx.Init<CheckLegalEntityInNTR>(new
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
                                .TransitionTo(CheckingDataInNTR)
                        ),
                    When(BelowLawfulAgeDetected)
                        .Then(ctx =>
                        {
                            ctx.Saga.DenialReason = EmpowermentsDenialReason.BelowLawfulAge;
                        })
                        .Finalize(),
                    When(LawfulAgeInfoNotAvailable)
                        .Then(ctx =>
                        {
                            ctx.Saga.DenialReason = EmpowermentsDenialReason.LawfulAgeInfoNotAvailable;
                        })
                        .Finalize()
                );

        During(CheckingDataInNTR,
            When(LegalEntityNTRCheckSucceeded)
                .Then(ctx =>
                {
                    ctx.Saga.LegalEntityCannotBeConfirmed = !ctx.Message.CanBeConfirmed;
                })
                .PublishAsync(ctx => ctx.Init<CollectAuthorizerSignatures>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.AuthorizerUids,
                    ctx.Saga.EmpowermentId,
                    SignaturesCollectionDeadline = ctx.Saga.ActivationDeadline
                }))
                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    Status = EmpowermentStatementStatus.CollectingAuthorizerSignatures,
                    DenialReason = EmpowermentsDenialReason.None // Suppress warning for missing property
                }))
                .TransitionTo(CollectingAuthorizerSignatures),
            When(LegalEntityNTRCheckFailed)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = ctx.Message.DenialReason;
                })
                .Finalize(),
            When(LegalEntityNotPresentInNTR)
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
                })),
            When(LegalEntityBulstatCheckSucceeded)
                .PublishAsync(ctx => ctx.Init<CollectAuthorizerSignatures>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.AuthorizerUids,
                    ctx.Saga.EmpowermentId,
                    SignaturesCollectionDeadline = ctx.Saga.ActivationDeadline
                }))
                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    Status = EmpowermentStatementStatus.CollectingAuthorizerSignatures,
                    DenialReason = EmpowermentsDenialReason.None // Suppress warning for missing property
                }))
                .Then(ctx =>
                {
                    ctx.Saga.LegalEntityCannotBeConfirmed = true;
                })
                .TransitionTo(CollectingAuthorizerSignatures),
            When(LegalEntityBulstatCheckFailed)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = ctx.Message.DenialReason;
                })
                .Finalize()
        );

        During(CollectingAuthorizerSignatures,
            When(SignaturesCollected)
                .IfElse(check => check.Saga.OnBehalfOf == OnBehalfOf.Individual,
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
                .TransitionTo(ValidatingLegalEntityEmpowerment)),
            When(SignatureCollectionFailed)
                .Then(ctx => 
                {
                    ctx.Saga.DenialReason = EmpowermentsDenialReason.SignatureCollectionTimeOut;
                })
                .Finalize()
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
                .Finalize(),
            When(LegalEntityNotPresentInNTR)
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
                })),
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
                .Then(ctx =>
                {
                    ctx.Saga.LegalEntityCannotBeConfirmed = true;
                })
                .TransitionTo(CheckingUidsRestrictions),
            When(LegalEntityBulstatCheckFailed)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = ctx.Message.DenialReason;
                })
                .Finalize()
        );

        During(CheckingUidsRestrictions,
            When(NoRestrictedUidsDetected)
                .PublishAsync(ctx => ctx.Init<VerifyUidsRegistrationStatus>(new
                {
                    CorrelationId = ctx.Saga.OriginCorrelationId,
                    ctx.Saga.EmpowermentId,
                    Uids = ctx.Saga.EmpoweredUids
                }))
                .TransitionTo(VerifyUidsRegistrationStatus),
            When(RestrictedUidsDetected)
                .Then(ctx => ctx.Saga.DenialReason = ctx.Message.DenialReason)
                .Finalize()
        );

        During(VerifyUidsRegistrationStatus,
            When(RegistrationStatusAllAvailable)
                .Then(context => context.Saga.SuccessfulCompletion = true)
                .Finalize(),
            When(InvalidRegistrationStatusDetected)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = ctx.Message.DenialReason;
                })
                .Finalize(),
            When(RegistrationStatusInfoNotAvailable)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = EmpowermentsDenialReason.UidsRegistrationStatusInfoNotAvailable;
                })
                .Finalize()
        );

#pragma warning disable CS8602 // Dereference of a possibly null reference.
        DuringAny(
            When(EmpowermentActivationTimedOut.Received)
                .Then(ctx =>
                {
                    ctx.Saga.DenialReason = EmpowermentsDenialReason.TimedOut;
                })
                .Finalize(),
            When(EmpowermentIsWithdrawn)
                .Then(ctx =>
                {
                    ctx.Saga.IsEmpowermentWithdrawn = true;
                })
                .Finalize()
        );
#pragma warning restore CS8602 // Dereference of a possibly null reference.

        BeforeEnter(Final,
            binder => binder
                .If(
                    // When EmpowermentIsWithdrawn is received during CollectingSignatures, the saga is completed without any further actions.
                    check => !check.Saga.IsEmpowermentWithdrawn, 
                    wasNotWithdrawn => wasNotWithdrawn
                        .IfElse(
                            check => check.Saga.SuccessfulCompletion,
                            success => success
                                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentStatus>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    ctx.Saga.EmpowermentId,
                                    Status = ctx.Saga.LegalEntityCannotBeConfirmed ? EmpowermentStatementStatus.Unconfirmed : EmpowermentStatementStatus.Active,
                                    DenialReason = EmpowermentsDenialReason.None // Suppress warning for missing property
                                }))
                                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    Uids = ctx.Saga.AuthorizerUids.Cast<UserIdentifier>(),
                                    ctx.Saga.EmpowermentId,
                                    EventCode = Service.EventsRegistration.Events.EmpowermentCompleted.Code,
                                    Service.EventsRegistration.Events.EmpowermentCompleted.Translations
                                }))
                                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    Uids = ctx.Saga.EmpoweredUids,
                                    ctx.Saga.EmpowermentId,
                                    EventCode = Service.EventsRegistration.Events.EmpowermentToMeCompleted.Code,
                                    Service.EventsRegistration.Events.EmpowermentToMeCompleted.Translations
                                })),
                            failure => failure
                                .PublishAsync(ctx => ctx.Init<ChangeEmpowermentStatus>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    ctx.Saga.EmpowermentId,
                                    Status = EmpowermentStatementStatus.Denied,
                                    ctx.Saga.DenialReason
                                }))
                                .PublishAsync(ctx => ctx.Init<NotifyUids>(new
                                {
                                    CorrelationId = ctx.Saga.OriginCorrelationId,
                                    Uids = ctx.Saga.AuthorizerUids.Cast<UserIdentifier>(),
                                    ctx.Saga.EmpowermentId,
                                    EventCode = Service.EventsRegistration.Events.EmpowermentDeclined.Code,
                                    Service.EventsRegistration.Events.EmpowermentDeclined.Translations
                                }))
                        )
                )
        );

        WhenEnter(Final,
            binder => binder
                .Unschedule(EmpowermentActivationTimedOut)
        );
        SetCompletedWhenFinalized();
    }
}
public class EmpowermentActivationStateMachineDefinition :
    SagaDefinition<EmpowermentActivationState>
{

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<EmpowermentActivationState> sagaConfigurator)
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
        sagaConfigurator.Message<InitiateEmpowermentActivationProcess>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<EmpowermentActivationTimedOut>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityNTRCheckSucceeded>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityNTRCheckFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityNotPresentInNTR>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<SignaturesCollected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<SignatureCollectionFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<NoRestrictedUidsDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<RestrictedUidsDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityEmpowermentValidated>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<LegalEntityEmpowermentValidationFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<TimestampEmpowermentXmlSucceeded>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<TimestampEmpowermentXmlFailed>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<RegistrationStatusAllAvailable>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<InvalidRegistrationStatusDetected>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<RegistrationStatusInfoNotAvailable>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
        sagaConfigurator.Message<EmpowermentIsWithdrawn>(x => x.UsePartitioner(partition, m => m.Message.EmpowermentId));
    }
}
