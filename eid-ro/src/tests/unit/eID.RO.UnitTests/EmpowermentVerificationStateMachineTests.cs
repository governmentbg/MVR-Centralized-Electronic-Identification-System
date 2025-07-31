using System.Net;
using eID.RO.Application.StateMachines;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using MassTransit;
using MassTransit.QuartzIntegration;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Quartz;

namespace eID.RO.UnitTests;

internal class EmpowermentVerificationStateMachineTests
{
    [Test]
    public async Task Individual_EmpowermentValidation_SuccessAsync()
    {
        var message = CreateInitiateIndividual();

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(mt =>
            {
                mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();

        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));
        Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>());
        Assert.That(await saga.Sagas.Any(s => s.FinishedAt != null));
    }

    [Test]
    public async Task LegalEntity_ValidationFailed_StatusIsDeniedAsync()
    {
        var message = CreateInitiateLegalEntity();

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(mt =>
            {
                mt.AddConsumer<FailingValidationConsumer>();
                mt.AddConsumer<ChangeEmpowermentStatusSucceededConsumer>();
                mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();

        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));
        Assert.That(await saga.Consumed.Any<LegalEntityEmpowermentValidationFailed>());
        Assert.That(await harness.Consumed.Any<ChangeEmpowermentStatus>(es => es.Context.Message.Status == EmpowermentStatementStatus.Denied), Is.True, "ChangeEmpowermentStatus was not consumed.");
    }

    [Test]
    public async Task LegalEntity_BulstatCheckSucceeded_ContinuesValidationAsync()
    {
        var message = CreateInitiateLegalEntity();

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(mt =>
            {
                mt.AddConsumer<NotFoundInNTRConsumer>();
                mt.AddConsumer<BulstatCheckSucceededConsumer>();
                mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();

        Assert.That(await saga.Consumed.Any<LegalEntityNotPresentInNTR>());
        Assert.That(await saga.Consumed.Any<LegalEntityBulstatCheckSucceeded>());
        Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>());
        Assert.That(await saga.Sagas.Any(s => s.FinishedAt != null));
    }

    [Test]
    public async Task LegalEntity_MissingOrMalformedResponse_EndsSagaAsync()
    {
        var message = CreateInitiateLegalEntity();

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(mt =>
            {
                mt.AddConsumer<MalformedNTRConsumer>();
                mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);

        var saga = harness.GetSagaStateMachineHarness<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
        Assert.That(await saga.Consumed.Any<LegalEntityNotPresentInNTR>());
        var currSaga = saga.Sagas.Select(q => q.EmpowermentId == message.EmpowermentId).FirstOrDefault();
        Assert.True(await saga.NotExists(currSaga.Saga.CorrelationId) is null, "Saga did not complete after timeout.");
    }
    [Test]
    public async Task LegalEntity_BulstatCheckFailed_StatusIsDeniedAsync()
    {
        var message = CreateInitiateLegalEntity();

        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(mt =>
            {
                mt.AddConsumer<NotFoundInNTRConsumer>();
                mt.AddConsumer<BulstatCheckFailedConsumer>();
                mt.AddConsumer<ChangeEmpowermentStatusSucceededConsumer>();
                mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();

        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();

        Assert.That(await saga.Consumed.Any<LegalEntityBulstatCheckFailed>(), "Saga did not consume LegalEntityBulstatCheckFailed.");
        Assert.That(await harness.Consumed.Any<ChangeEmpowermentStatus>(m => m.Context.Message.Status == EmpowermentStatementStatus.Denied), "Saga did not publish ChangeEmpowermentStatus.");
    }

    [Test]
    public async Task EndOfDayReached_CompletesSagaAsync()
    {
        var message = CreateInitiateIndividual();

        await using var provider = new ServiceCollection()
            .AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            })
            .AddMassTransitTestHarness(mt =>
            {
                mt.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(2));
                mt.AddPublishMessageScheduler();
                mt.AddQuartzConsumers();
                mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                mt.AddSagaStateMachine<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
                mt.UsingInMemory((context, cfg) =>
                {
                    cfg.UsePublishMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        using var adjustment = new QuartzTimeAdjustment(provider);

        var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentVerificationStateMachine, EmpowermentVerificationState>();
        Assert.That(await sagaHarness.Sagas.Any(x => x.OriginCorrelationId == message.CorrelationId), Is.True, "Saga was not created.");
        Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentValidationProcess>());
        var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == message.EmpowermentId).FirstOrDefault();

        await adjustment.AdvanceTime(TimeSpan.FromDays(2));
        Assert.That(await sagaHarness.Consumed.Any<EmpowermentValidationCheckValidityExpired>());
        Assert.True(await sagaHarness.NotExists(currSaga.Saga.CorrelationId) is null, "Saga did not complete after timeout.");
    }

    private class MalformedNTRConsumer : IConsumer<ValidateLegalEntityEmpowerment>
    {
        public Task Consume(ConsumeContext<ValidateLegalEntityEmpowerment> context)
        {
            return context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                MissingOrMalformedResponse = true
            });
        }
    }

    private static InitiateEmpowermentValidationProcess CreateInitiateIndividual() => new InitiateEmpowermentValidationProcessDTO()
    {
        CorrelationId = Guid.NewGuid(),
        EmpowermentId = Guid.NewGuid(),
        OnBehalfOf = OnBehalfOf.Individual,
        Uid = "8802184852",
        UidType = IdentifierType.EGN,
        Name = "Test Individual",
        AuthorizerUids = new[] { new AuthorizerIdentifierData { Uid = "8802184852", UidType = IdentifierType.EGN, IsIssuer = true } },
        EmpoweredUids = new[] { new UserIdentifierData { Uid = "8802184852", UidType = IdentifierType.EGN } }
    };

    private static InitiateEmpowermentValidationProcess CreateInitiateLegalEntity() => new InitiateEmpowermentValidationProcessDTO()
    {
        CorrelationId = Guid.NewGuid(),
        EmpowermentId = Guid.NewGuid(),
        OnBehalfOf = OnBehalfOf.LegalEntity,
        Uid = "147119101",
        UidType = IdentifierType.NotSpecified,
        Name = "Company X",
        IssuerPosition = "CEO",
        AuthorizerUids = new[] { new AuthorizerIdentifierData { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Manager", IsIssuer = true } },
        EmpoweredUids = new[] { new UserIdentifierData { Uid = "2804115607", UidType = IdentifierType.EGN } }
    };

    internal class InitiateEmpowermentValidationProcessDTO : InitiateEmpowermentValidationProcess
    {
        public Guid EmpowermentId { get; set; }

        public OnBehalfOf OnBehalfOf { get; set; }

        public string Uid { get; set; }
        public IdentifierType UidType { get; set; }
        public string Name { get; set; }
        public string IssuerPosition { get; set; }
        public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; }
        public IEnumerable<UserIdentifier> EmpoweredUids { get; set; }

        public Guid CorrelationId { get; set; }
    }

    private class SuccessUidsRestrictionsConsumer : IConsumer<CheckUidsRestrictions>
    {
        public Task Consume(ConsumeContext<CheckUidsRestrictions> context)
        {
            return context.RespondAsync<NoRestrictedUidsDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }

    private class FailingValidationConsumer : IConsumer<ValidateLegalEntityEmpowerment>
    {
        public Task Consume(ConsumeContext<ValidateLegalEntityEmpowerment> context)
        {
            return context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.NTRCheckFailed
            });
        }
    }
    private class BulstatCheckFailedConsumer : IConsumer<CheckLegalEntityInBulstat>
    {
        public Task Consume(ConsumeContext<CheckLegalEntityInBulstat> context)
        {
            return context.RespondAsync<LegalEntityBulstatCheckFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.UnsuccessfulLegalEntityCheck
            });
        }
    }

    private class NotFoundInNTRConsumer : IConsumer<ValidateLegalEntityEmpowerment>
    {
        public Task Consume(ConsumeContext<ValidateLegalEntityEmpowerment> context)
        {
            return context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                MissingOrMalformedResponse = false
            });
        }
    }

    private class BulstatCheckSucceededConsumer : IConsumer<CheckLegalEntityInBulstat>
    {
        public Task Consume(ConsumeContext<CheckLegalEntityInBulstat> context)
        {
            return context.RespondAsync<LegalEntityBulstatCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    private class ChangeEmpowermentStatusSucceededConsumer : IConsumer<ChangeEmpowermentStatus>
    {
        public Task Consume(ConsumeContext<ChangeEmpowermentStatus> context)
        {
            return context.RespondAsync(new ServiceResult<bool>
            {
                StatusCode = HttpStatusCode.OK,
                Result = true
            });
        }
    }
}
