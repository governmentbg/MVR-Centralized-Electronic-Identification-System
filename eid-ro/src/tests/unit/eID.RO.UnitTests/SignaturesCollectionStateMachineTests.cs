using eID.RO.Application.StateMachines;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using MassTransit;
using MassTransit.QuartzIntegration;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Quartz;

namespace eID.RO.UnitTests;

internal class SignaturesCollectionStateMachineTests
{
    private NullLogger<SignaturesCollectionStateMachine> _logger = NullLogger<SignaturesCollectionStateMachine>.Instance;

    [Test]
    public async Task IsCreated_Async()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddSagaStateMachine<SignaturesCollectionStateMachine, SignaturesCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateSignatureCollectionProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                AuthorizerUids = new UserIdentifierWithNameData[] { new() { Name = "Test Name", Uid = "8802184852", UidType = IdentifierType.EGN } },
                SignaturesCollectionDeadline = DateTime.UtcNow.AddHours(1)
            };
            await harness.Bus.Publish<CollectAuthorizerSignatures>(command);
            var sagaHarness = harness.GetSagaStateMachineHarness<SignaturesCollectionStateMachine, SignaturesCollectionState>();
            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>());
            Assert.That(await sagaHarness.Consumed.Any<CollectAuthorizerSignatures>());
            Assert.That(await sagaHarness.Created.Any(x => x.OriginCorrelationId == command.CorrelationId));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task IsFinalizedAfterLastSignatureAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddSagaStateMachine<SignaturesCollectionStateMachine, SignaturesCollectionState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateSignatureCollectionProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                AuthorizerUids = new UserIdentifierWithNameData[]
                {
                    new() { Name = "Test Name", Uid = "8802184852", UidType = IdentifierType.EGN },
                    new() { Name = "Test Name", Uid = "8402241834", UidType = IdentifierType.EGN }
                },
                SignaturesCollectionDeadline = DateTime.UtcNow.AddHours(1)
            };
            await harness.Bus.Publish<CollectAuthorizerSignatures>(command);
            var sagaHarness = harness.GetSagaStateMachineHarness<SignaturesCollectionStateMachine, SignaturesCollectionState>();
            Assert.Multiple(async () =>
            {
                Assert.That(await harness.Published.Any<CollectAuthorizerSignatures>(), Is.True);
                Assert.That(await sagaHarness.Consumed.Any<CollectAuthorizerSignatures>());
            });
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).First();
            Assert.That(currSaga?.Saga.SignedUids.Count, Is.EqualTo(0));

            await harness.Bus.Publish<EmpowermentSigned>(new
            {
                command.EmpowermentId,
                SignerName = command.AuthorizerUids.First().Name,
                SignerUid = command.AuthorizerUids.First().Uid,
                SignerUidType = command.AuthorizerUids.First().UidType
            });
            Assert.Multiple(async () =>
            {
                Assert.That(await sagaHarness.Consumed.Any<EmpowermentSigned>(s =>
                            s.Context.Message.SignerName == command.AuthorizerUids.First().Name &&
                            s.Context.Message.SignerUid == command.AuthorizerUids.First().Uid &&
                            s.Context.Message.SignerUidType == command.AuthorizerUids.First().UidType));

                Assert.That(currSaga?.Saga.CurrentState, Is.EqualTo(sagaHarness.StateMachine.CollectingSignatures.Name));
            });
            await harness.Bus.Publish<EmpowermentSigned>(new
            {
                command.EmpowermentId,
                SignerName = command.AuthorizerUids.Last().Name,
                SignerUid = command.AuthorizerUids.Last().Uid,
                SignerUidType = command.AuthorizerUids.Last().UidType
            });
            Assert.Multiple(async () =>
            {
                Assert.That(await sagaHarness.Consumed.Any<EmpowermentSigned>(s =>
                            s.Context.Message.SignerName == command.AuthorizerUids.Last().Name &&
                            s.Context.Message.SignerUid == command.AuthorizerUids.Last().Uid &&
                            s.Context.Message.SignerUidType == command.AuthorizerUids.Last().UidType));

                Assert.That(await sagaHarness.Created.Any(x => x.OriginCorrelationId == command.CorrelationId));
                Assert.That(currSaga?.Saga.CurrentState, Is.EqualTo(sagaHarness.StateMachine.Final.Name));
                Assert.That(currSaga?.Saga.SignedUids.Count, Is.EqualTo(command.AuthorizerUids.Count()));
                Assert.That(await harness.Published.Any<SignaturesCollected>());
            });
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task GetsFinalizedAfterDeadlineAsync()
    {
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
                    mt.AddSagaStateMachine<SignaturesCollectionStateMachine, SignaturesCollectionState>();
                    mt.UsingInMemory((context, cfg) =>
                    {
                        cfg.UsePublishMessageScheduler();

                        cfg.ConfigureEndpoints(context);
                    });
                })
                .BuildServiceProvider(true);

        using var adjustment = new QuartzTimeAdjustment(provider);
        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateSignatureCollectionProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                AuthorizerUids = new UserIdentifierWithNameData[]
                {
                    new() { Uid = "8802184852", UidType = IdentifierType.EGN },
                    new() { Uid = "8402241834", UidType = IdentifierType.EGN }
                },
                SignaturesCollectionDeadline = DateTime.UtcNow.AddHours(1)
            };
            await harness.Bus.Publish<CollectAuthorizerSignatures>(command);
            var sagaHarness = harness.GetSagaStateMachineHarness<SignaturesCollectionStateMachine, SignaturesCollectionState>();
            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>());
            Assert.That(await sagaHarness.Consumed.Any<CollectAuthorizerSignatures>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.SignedUids.Count == 0);
            await adjustment.AdvanceTime(TimeSpan.FromHours(2));
            Assert.That(await harness.Published.Any<SignatureCollectionFailed>());
        }
        finally
        {
            await harness.Stop();
        }
    }
}
