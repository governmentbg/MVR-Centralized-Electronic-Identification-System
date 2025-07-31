using eID.RO.Application.StateMachines;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service;
using eID.RO.Service.EventsRegistration;
using MassTransit;
using MassTransit.QuartzIntegration;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Quartz;

namespace eID.RO.UnitTests;

internal class WithdrawalsCollectionStateMachineTests
{
    [Test]
    [TestCaseSource(nameof(_validStatementsTestCases))]
    public async Task IsCreated_Async(string caseName, InitiateEmpowermentWithdrawalProcess command)
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(command);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == command.CorrelationId));
    }

    [Test]
    public async Task LegalEntity_SendVerifyLegalEntityWithdrawalRequesterAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<SuccessVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await harness.Published.Any<VerifyLegalEntityWithdrawalRequester>(), Is.True, "VerifyLegalEntityWithdrawalRequester message was not published.");
        Assert.That(await saga.Consumed.Any<VerifyLegalEntityWithdrawalRequesterSucceeded>());
    }

    [Test]
    public async Task LegalEntity_SuccessVerifyLegalEntityWithdrawalRequesterAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<SuccessVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish<Contracts.Commands.InitiateEmpowermentWithdrawalProcess>(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();


        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<VerifyLegalEntityWithdrawalRequesterSucceeded>());
        Assert.That(await harness.Published.Any<CheckUidsRestrictions>(), Is.True, "CheckUidsRestrictions message was not published.");
    }

    [Test]
    public async Task LegalEntity_FailingVerifyLegalEntityWithdrawalRequesterAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<FailingVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<VerifyLegalEntityWithdrawalRequesterFailed>());

        await CheckWithdrawIsDeniedAsync(harness, saga, message);
    }
    
    [Test]
    public async Task LegalEntity_NotFoundInNTRChecksInBulstatAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<NotFoundInNTRVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<LegalEntityNotPresentInNTR>(), Is.True, "LegalEntityNotPresentInNTR message was not consumed.");
        Assert.That(await harness.Published.Any<CheckLegalEntityInBulstat>(), Is.True, "CheckLegalEntityInBulstat message was not published.");
    }

    [Test]
    public async Task LegalEntity_SuccedingCheckInBulstatWillCheckForRestrictionsAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<NotFoundInNTRVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddConsumer<SucceedingLegalEntityCheckInBulstatConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<LegalEntityNotPresentInNTR>(), Is.True, "LegalEntityNotPresentInNTR message was not consumed.");
        Assert.That(await harness.Published.Any<CheckLegalEntityInBulstat>(), Is.True, "CheckLegalEntityInBulstat message was not published.");
        Assert.That(await saga.Consumed.Any<LegalEntityBulstatCheckSucceeded>(), Is.True, "LegalEntityBulstatCheckSucceeded message was not consumed.");
        Assert.That(await harness.Published.Any<CheckUidsRestrictions>(), Is.True, "CheckUidsRestrictions message was not published.");
    }


    [Test]
    public async Task LegalEntity_FailedCheckInBulstatWillDenyWithdrawalAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<NotFoundInNTRVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddConsumer<FailingLegalEntityCheckInBulstatConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<LegalEntityNotPresentInNTR>(), Is.True, "LegalEntityNotPresentInNTR message was not consumed.");
        Assert.That(await harness.Published.Any<CheckLegalEntityInBulstat>(), Is.True, "CheckLegalEntityInBulstat message was not published.");

        Assert.That(await saga.Consumed.Any<LegalEntityBulstatCheckFailed>(), Is.True, "LegalEntityBulstatCheckFailed message was not consumed.");
        await CheckWithdrawIsDeniedAsync(harness, saga, message);
    }

    [Test]
    public async Task LegalEntity_SuccessCheckingUidsRestrictions_OnePersonAsync()
    {
        var message = GetInitiateLegalEntityOnePerson();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<SuccessVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.Multiple(async () =>
        {
            Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

            Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
            Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
            Assert.That(await harness.Published.Any<VerifyLegalEntityWithdrawalRequester>(), Is.True, "VerifyLegalEntityWithdrawalRequester message was not published.");
            Assert.That(await saga.Consumed.Any<VerifyLegalEntityWithdrawalRequesterSucceeded>(), Is.True, "VerifyLegalEntityWithdrawalRequesterSucceeded message was not consumed.");
            Assert.That(await harness.Published.Any<CheckUidsRestrictions>(), Is.True, "CheckUidsRestrictions message was not published.");
            Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>(), Is.True, "NoRestrictedUidsDetected message was not consumed.");
        });
        await AssertWithdrawalIsCompletedAsync(harness, saga, message);
    }

    [Test]
    public async Task LegalEntity_SuccessCheckingUidsRestrictions_TwoPeopleAsync()
    {
        var message = GetInitiateLegalEntityTwoPeople();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<SuccessVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>());

        //One person is enough to withdraw empowerment
        //We skip the confirmation part and we withdraw empowerment directly
        Assert.That(saga.StateMachine.States.Any(x => x.Name == "WithdrawalIsCompleted"));
        Assert.That(await harness.Published.Any<NotifyUids>());
        Assert.That(await harness.Published.Any<ChangeEmpowermentStatus>());
        Assert.That(await harness.Published.Any<ChangeEmpowermentWithdrawalStatus>());

        AssertSagaState(harness, message.EmpowermentId, "Final");
    }

    [Test]
    public async Task LegalEntity_SuccessWithdrawal_TwoPeopleAsync()
    {
        var message = GetInitiateLegalEntityTwoPeople();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<SuccessVerifyLegalEntityWithdrawalRequesterConsumer>();
                    mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);

        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>());

        //One person is enough to withdraw empowerment
        //We skip the confirmation part and we withdraw empowerment directly
        Assert.That(saga.StateMachine.States.Any(x => x.Name == "WithdrawalIsCompleted"));
        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await harness.Published.Any<NotifyUids>());
        Assert.That(await harness.Published.Any<ChangeEmpowermentStatus>());
        Assert.That(await harness.Published.Any<ChangeEmpowermentWithdrawalStatus>());
        Assert.That(await harness.Published.Any<EmpowermentIsWithdrawn>());

        AssertSagaState(harness, message.EmpowermentId, "Final");

        await AssertWithdrawalIsCompletedAsync(harness, saga, message);
    }

    [Test]
    public async Task LegalEntity_WithdrawalEmpowermentDontEnterTimeout_TwoPeopleAsync()
    {
        var message = GetInitiateLegalEntityTwoPeople();

        await using var provider = new ServiceCollection()
            .AddQuartz(q =>
            {
                q.UseMicrosoftDependencyInjectionJobFactory();
            })
            .AddMassTransitTestHarness(mt =>
            {
                mt.SetTestTimeouts(testInactivityTimeout: TimeSpan.FromSeconds(3));
                mt.AddPublishMessageScheduler();
                mt.AddQuartzConsumers();
                mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                mt.UsingInMemory((context, cfg) =>
                {
                    cfg.UsePublishMessageScheduler();

                    cfg.ConfigureEndpoints(context);
                });
                mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                mt.AddConsumer<SuccessVerifyLegalEntityWithdrawalRequesterConsumer>();
                mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        using var adjustment = new QuartzTimeAdjustment(provider);

        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>());

        await adjustment.AdvanceTime(TimeSpan.FromDays(8));

        Assert.IsTrue(!await saga.Consumed.Any<WithdrawalsCollectionTimedOut>());

        //One person is eonugh to withdraw empowerment
        //We skil the confirmation part and we withdraw empowerment directly
        Assert.That(saga.StateMachine.States.Any(x => x.Name == "WithdrawalIsCompleted"));
        Assert.That(await harness.Published.Any<NotifyUids>());
        Assert.That(await harness.Published.Any<ChangeEmpowermentStatus>());
        Assert.That(await harness.Published.Any<ChangeEmpowermentWithdrawalStatus>());

        AssertSagaState(harness, message.EmpowermentId, "Final");
    }

    [Test]
    public async Task Individual_SuccessCheckingAuthorizersForRestrictionsAsync()
    {
        var message = GetInitiateIndividual();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<SuccessUidsRestrictionsConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish<Contracts.Commands.InitiateEmpowermentWithdrawalProcess>(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.Multiple(async () =>
        {
            Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

            Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
            Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
            Assert.That(await harness.Published.Any<CheckUidsRestrictions>(), Is.True, "CheckAuthorizersForRestrictions message was not published.");
            Assert.That(await saga.Consumed.Any<NoRestrictedUidsDetected>());
        });
        await AssertWithdrawalIsCompletedAsync(harness, saga, message);
    }

    [Test]
    public async Task Individual_FailingTimestampingAsync()
    {
        var message = GetInitiateIndividual();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<FailingTimestampWithdrawalConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalFailed>());

        await CheckWithdrawIsDeniedAsync(harness, saga, message);
    }

    [Test]
    public async Task Individual_FailingAuthorizerRestrictionsAsync()
    {
        var message = GetInitiateIndividual();
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampWithdrawalConsumer>();
                    mt.AddConsumer<FailingAuthorizerRestrictionsConsumer>();
                    mt.AddSagaStateMachine<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();
        await harness.Start();
        await harness.Bus.Publish(message);
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == message.CorrelationId));

        Assert.IsTrue(await harness.Published.Any<TimestampEmpowermentWithdrawal>(), "TimestampEmpowermentWithdrawal message was not published.");
        Assert.That(await saga.Consumed.Any<TimestampEmpowermentWithdrawalSucceeded>());
        Assert.IsTrue(await harness.Published.Any<CheckUidsRestrictions>(), "CheckAuthorizersForRestrictions message was not published.");
        Assert.That(await saga.Consumed.Any<RestrictedUidsDetected>());

        await CheckWithdrawIsDeniedAsync(harness, saga, message);
    }

    private async Task AssertWithdrawalIsCompletedAsync(
        ITestHarness harness,
        ISagaStateMachineTestHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState> saga,
        InitiateEmpowermentWithdrawalProcess command)
    {
        Assert.IsTrue(await harness.Published
            .Any<ChangeEmpowermentWithdrawalStatus>(ml =>
                ((ChangeEmpowermentWithdrawalStatus)ml.MessageObject).Status == EmpowermentWithdrawalStatus.Completed),
            "ChangeEmpowermentWithdrawalStatus message was not published.");

        Assert.IsTrue(await harness.Published
            .Any<ChangeEmpowermentStatus>(ml =>
                ((ChangeEmpowermentStatus)ml.MessageObject).Status == EmpowermentStatementStatus.Withdrawn),
            "ChangeEmpowermentStatus message was not published.");

        var notifyAuthorizersMessagePublished = await harness.Published
                        .SelectAsync<NotifyUids>(m =>
                            !command.AuthorizerUids.Except(m.Context.Message.Uids, new UserIdentifierEqualityComparer()).Any()
                            && m.Context.Message.EventCode == Events.EmpowermentWasWithdrawn.Code)
                        .Any();
        Assert.IsTrue(notifyAuthorizersMessagePublished,
                "NotifyUids for authorizers message was not published.");

        var notifyEmpoweredUidsMessagePublished = await harness.Published
                        .SelectAsync<NotifyUids>(m =>
                            !command.EmpoweredUids.Except(m.Context.Message.Uids, new UserIdentifierEqualityComparer()).Any()
                            && m.Context.Message.EventCode == Events.EmpowermentToMeWasWithdrawn.Code)
                        .Any();

        Assert.IsTrue(notifyEmpoweredUidsMessagePublished,
                "NotifyUids for empowered uids message was not published.");

        AssertSagaState(harness, command.EmpowermentId, "Final");
    }

    private async Task CheckWithdrawIsDeniedAsync(
        ITestHarness harness,
        ISagaStateMachineTestHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState> saga,
        InitiateEmpowermentWithdrawalProcess command)
    {
        Assert.IsTrue(await harness.Published
            .Any<ChangeEmpowermentWithdrawalStatus>(ml =>
                ((ChangeEmpowermentWithdrawalStatus)ml.MessageObject).Status == EmpowermentWithdrawalStatus.Denied),
            "ChangeEmpowermentsWithdrawStatus message was not published.");

        var notifyMessagePublished = await harness.Published
                        .SelectAsync<NotifyUids>(m =>
                            !command.AuthorizerUids.Except(m.Context.Message.Uids, new UserIdentifierEqualityComparer()).Any()
                            && m.Context.Message.EventCode == Events.WithdrawalDeclined.Code)
                        .Any();

        Assert.IsTrue(notifyMessagePublished,
                "NotifyUids for empowered uids message was not published.");

        AssertSagaState(harness, command.EmpowermentId, "Final");
    }

    private async Task AssertWithdrawalIsTimedOutAsync(
        ITestHarness harness,
        ISagaStateMachineTestHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState> saga,
        InitiateEmpowermentWithdrawalProcess command)
    {
        Assert.IsTrue(await harness.Published
            .Any<ChangeEmpowermentWithdrawalStatus>(ml =>
                ((ChangeEmpowermentWithdrawalStatus)ml.MessageObject).Status == EmpowermentWithdrawalStatus.Timeout),
            "ChangeEmpowermentsWithdrawStatus message was not published.");

        var notifyMessagePublished = await harness.Published
            .SelectAsync<NotifyUids>(m =>
                    !command.AuthorizerUids.Except(m.Context.Message.Uids, new UserIdentifierEqualityComparer()).Any()
                    && m.Context.Message.EventCode == Events.WithdrawalTimeout.Code
            )
            .Any();

        Assert.IsTrue(notifyMessagePublished,
                "NotifyUids message was not published.");
        AssertSagaState(harness, command.EmpowermentId, "Final");
    }

    private void AssertSagaState(ITestHarness harness, Guid empowermentId, string state)
    {
        var saga = harness.GetSagaStateMachineHarness<WithdrawalsCollectionStateMachine, WithdrawalsCollectionState>();
        var currSaga = saga.Sagas.Select(q => q.EmpowermentId == empowermentId).FirstOrDefault();

        Assert.IsTrue(currSaga?.Saga.CurrentState == state, "Saga is not in state {0}", state);
    }

    private class SuccessVerifyLegalEntityWithdrawalRequesterConsumer :
        IConsumer<VerifyLegalEntityWithdrawalRequester>
    {
        public async Task Consume(ConsumeContext<VerifyLegalEntityWithdrawalRequester> context)
        {
            await context.RespondAsync<VerifyLegalEntityWithdrawalRequesterSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }

    private class FailingVerifyLegalEntityWithdrawalRequesterConsumer :
        IConsumer<VerifyLegalEntityWithdrawalRequester>
    {
        public async Task Consume(ConsumeContext<VerifyLegalEntityWithdrawalRequester> context)
        {
            await context.RespondAsync<VerifyLegalEntityWithdrawalRequesterFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    private class NotFoundInNTRVerifyLegalEntityWithdrawalRequesterConsumer :
        IConsumer<VerifyLegalEntityWithdrawalRequester>
    {
        public async Task Consume(ConsumeContext<VerifyLegalEntityWithdrawalRequester> context)
        {
            await context.RespondAsync<LegalEntityNotPresentInNTR>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class SucceedingLegalEntityCheckInBulstatConsumer :
        IConsumer<CheckLegalEntityInBulstat>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInBulstat> context)
        {
            await context.RespondAsync<LegalEntityBulstatCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class FailingLegalEntityCheckInBulstatConsumer :
        IConsumer<CheckLegalEntityInBulstat>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInBulstat> context)
        {
            await context.RespondAsync<LegalEntityBulstatCheckFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.UnsuccessfulLegalEntityCheck
            });
        }
    }

    private class SuccessUidsRestrictionsConsumer :
        IConsumer<CheckUidsRestrictions>
    {
        public async Task Consume(ConsumeContext<CheckUidsRestrictions> context)
        {
            await context.RespondAsync<NoRestrictedUidsDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }

    private class FailingAuthorizerRestrictionsConsumer :
        IConsumer<CheckUidsRestrictions>
    {
        public async Task Consume(ConsumeContext<CheckUidsRestrictions> context)
        {
            await context.RespondAsync<RestrictedUidsDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }

    public class SuccessfulTimestampWithdrawalConsumer :
        IConsumer<TimestampEmpowermentWithdrawal>
    {
        public async Task Consume(ConsumeContext<TimestampEmpowermentWithdrawal> context)
        {
            await context.RespondAsync<TimestampEmpowermentWithdrawalSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                context.Message.EmpowermentWithdrawalId
            });
        }
    }
    public class FailingTimestampWithdrawalConsumer :
        IConsumer<TimestampEmpowermentWithdrawal>
    {
        public async Task Consume(ConsumeContext<TimestampEmpowermentWithdrawal> context)
        {
            await context.RespondAsync<TimestampEmpowermentWithdrawalFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                context.Message.EmpowermentWithdrawalId
            });
        }
    }
    private static InitiateEmpowermentWithdrawalProcess GetInitiateIndividual() =>
        new()
        {
            CorrelationId = Guid.NewGuid(),
            StartDateTime = DateTime.Now,
            WithdrawalsCollectionsDeadline = DateTime.Now.AddDays(7),
            IssuerUid = "8802184852",
            IssuerUidType = IdentifierType.EGN,
            Reason = "Вече не е нужен",
            EmpowermentId = Guid.NewGuid(),
            OnBehalfOf = OnBehalfOf.Individual,
            EmpowermentWithdrawalId = Guid.NewGuid(),
            AuthorizerUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
            IssuerName = "Test User Full Name",
        };

    private static InitiateEmpowermentWithdrawalProcess GetInitiateLegalEntityOnePerson() =>
            new()
            {
                CorrelationId = Guid.NewGuid(),
                StartDateTime = DateTime.Now,
                WithdrawalsCollectionsDeadline = DateTime.Now.AddDays(7),
                IssuerUid = "8802184852",
                IssuerUidType = IdentifierType.EGN,
                Reason = "Вече не е нужен",
                EmpowermentId = Guid.NewGuid(),
                OnBehalfOf = OnBehalfOf.LegalEntity,
                EmpowermentWithdrawalId = Guid.NewGuid(),
                AuthorizerUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                LegalEntityUid = "147119101",
                LegalEntityName = "СТИЛО ЕООД",
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            };

    private static InitiateEmpowermentWithdrawalProcess GetInitiateLegalEntityTwoPeople() =>
            new()
            {
                CorrelationId = Guid.NewGuid(),
                StartDateTime = DateTime.Now,
                WithdrawalsCollectionsDeadline = DateTime.Now.AddDays(7),
                IssuerUid = "8802184852",
                IssuerUidType = IdentifierType.EGN,
                Reason = "Вече не е нужен",
                EmpowermentId = Guid.NewGuid(),
                OnBehalfOf = OnBehalfOf.LegalEntity,
                EmpowermentWithdrawalId = Guid.NewGuid(),
                AuthorizerUids = new UserIdentifierData[]
                {
                    new() { Uid = "8802184852", UidType = IdentifierType.EGN },
                    new() { Uid = "2804115607", UidType = IdentifierType.EGN },
                },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                LegalEntityUid = "147119101",
                LegalEntityName = "СТИЛО ЕООД",
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            };

    private static readonly object[] _validStatementsTestCases =
    {
        new object[] {
            "IndividualStatement",
            GetInitiateIndividual()
        },
        new object[] {
            "LegalEntityStatement_1",
            GetInitiateLegalEntityOnePerson()
        },
        new object[] {
            "LegalEntityStatement_2",
            GetInitiateLegalEntityTwoPeople()
        }
    };

    internal class InitiateEmpowermentWithdrawalProcess : Contracts.Commands.InitiateEmpowermentWithdrawalProcess
    {
        public DateTime StartDateTime { get; set; }

        public DateTime? WithdrawalsCollectionsDeadline { get; set; }

        public string IssuerUid { get; set; }

        public IdentifierType IssuerUidType { get; set; }

        public string Reason { get; set; }

        public Guid EmpowermentId { get; set; }

        public OnBehalfOf OnBehalfOf { get; set; }

        public IEnumerable<UserIdentifier> AuthorizerUids { get; set; }

        public IEnumerable<UserIdentifier> EmpoweredUids { get; set; }

        public Guid EmpowermentWithdrawalId { get; set; }

        public string LegalEntityUid { get; set; }

        public string LegalEntityName { get; set; }

        public string IssuerName { get; set; }

        public string IssuerPosition { get; set; }

        public Guid CorrelationId { get; set; }
    }
}
