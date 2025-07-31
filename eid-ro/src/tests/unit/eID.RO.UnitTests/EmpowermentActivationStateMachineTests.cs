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

internal class EmpowermentActivationStateMachineTests
{
    private NullLogger<EmpowermentActivationStateMachine> _logger = NullLogger<EmpowermentActivationStateMachine>.Instance;

    [Test]
    [TestCaseSource(nameof(_validStatementsTestCases))]
    public async Task IsCreated_Async(string caseName, InitiateEmpowermentActivationProcess command)
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);
            var saga = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == command.CorrelationId));

            var sagaWithOriginCorrId = saga.Created.Select(x => x.OriginCorrelationId == command.CorrelationId).First().Saga;
            Assert.That(sagaWithOriginCorrId.DenialReason, Is.EqualTo(EmpowermentsDenialReason.None));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task LegalEntity_WaitsForSignaturesAfterSuccessfulNTRCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<CheckLegalEntityInNTRConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);
            var saga = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == command.CorrelationId));

            Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInNTR>(), "CheckLegalEntityInNTR message was not published.");
            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>(), "CollectAuthorizerSignatures message was not published.");
            Assert.IsTrue(await harness.Published.Any<ChangeEmpowermentStatus>(), "ChangeEmpowermentStatus message was not published.");

            var sagaWithOriginCorrId = saga.Created.Select(x => x.OriginCorrelationId == command.CorrelationId).First().Saga;
            Assert.That(sagaWithOriginCorrId.DenialReason, Is.EqualTo(EmpowermentsDenialReason.None));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task LegalEntity_FinalizedAfterFailedNTRCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<FailingCheckLegalEntityInNTRConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNTRCheckFailed>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.NTRCheckFailed);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task LegalEntity_WaitsForSignaturesAfterCannotBeConfirmedNTRCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<CannotBeConfirmedCheckLegalEntityInNTRConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess was not published.");
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess was not consumed.");
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNTRCheckSucceeded>(), "LegalEntityNTRCheckSucceeded was not consumed.");
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.LegalEntityCannotBeConfirmed == true, "LegalEntityCannotBeConfirmed is not true.");

            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>(), "CollectAuthorizerSignatures message was not published.");
        }
        finally
        {
            await harness.Stop();
        }
    }


    [Test]
    public async Task LegalEntity_WaitsForSignaturesAfterCanBeConfirmedNTRCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<CanBeConfirmedCheckLegalEntityInNTRConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess was not published.");
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess was not consumed.");
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNTRCheckSucceeded>(), "LegalEntityNTRCheckSucceeded was not consumed.");
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.LegalEntityCannotBeConfirmed == false, "LegalEntityCannotBeConfirmed is not false.");

            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>(), "CollectAuthorizerSignatures message was not published.");
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task LegalEntity_WillCheckBulstatWhenNotFoundInNTRAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<NoDataForLegalEntityInNTRConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not published.");
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not consumed.");
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNotPresentInNTR>(), "LegalEntityNotPresentInNTR message was not consumed.");
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInBulstat>(), "CheckLegalEntityInBulstat message was not published.");
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task LegalEntity_FinalizedAfterFailedBulstatCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<NoDataForLegalEntityInNTRConsumer>();
                    mt.AddConsumer<FailingLegalEntityCheckInBulstatConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not published.");
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not consumed.");
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNotPresentInNTR>(), "LegalEntityNotPresentInNTR message was not consumed.");
            Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInBulstat>(), "CheckLegalEntityInBulstat message was not published.");

            Assert.That(await sagaHarness.Consumed.Any<LegalEntityBulstatCheckFailed>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.UnsuccessfulLegalEntityCheck);
        }
        finally
        {
            await harness.Stop();
        }
    }
    //[Test]
    //public async Task LegalEntity_FinalizedAfterSuccessfulUnconfirmedBulstatCheckAsync()
    //{
    //    await using var provider = new ServiceCollection()
    //            .AddMassTransitTestHarness(mt =>
    //            {
    //                mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
    //                mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
    //                mt.AddConsumer<NoDataForLegalEntityInNTRConsumer>();
    //                mt.AddConsumer<SucceedingLegalEntityCheckInBulstatConsumer>();
    //                mt.AddConsumer<SuccessfulAuthorizerRestrictionsConsumer>();
    //                mt.AddConsumer<SuccessfulSigningConsumer>();
    //                mt.AddConsumer<SuccessfulValidateLegalEntityEmpowermentConsumer>();
    //                mt.AddConsumer<SuccessfulVerifyUidsRegistrationStatusConsumer>();
    //                mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //            })
    //            .BuildServiceProvider(true);

    //    var harness = provider.GetTestHarness();

    //    await harness.Start();
    //    try
    //    {
    //        var command = new InitiateEmpowermentActivationProcessCommand
    //        {
    //            CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
    //            EmpowermentId = Guid.NewGuid(),
    //            Uid = "8802184852",
    //            Name = "Фирма",
    //            IssuerName = "Иван Иванов Иванов",
    //            IssuerPosition = "Шеф",
    //            OnBehalfOf = OnBehalfOf.LegalEntity,
    //            AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
    //            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            ExpiryDate = null
    //        };
    //        await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

    //        var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //        Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not published.");
    //        Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not consumed.");
    //        Assert.That(await sagaHarness.Consumed.Any<LegalEntityNotPresentInNTR>(), "LegalEntityNotPresentInNTR message was not consumed.");
    //        Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInBulstat>(), "CheckLegalEntityInBulstat message was not published.");

    //        Assert.That(await sagaHarness.Consumed.Any<LegalEntityBulstatCheckSucceeded>(), "LegalEntityBulstatCheckSucceeded message was not consumed.");
    //        Assert.That(await sagaHarness.Consumed.Any<NoRestrictedUidsDetected>(), "NoRestrictedUidsDetected message was not consumed.");
    //        Assert.That(await sagaHarness.Consumed.Any<RegistrationStatusAllAvailable>(), "RegistrationStatusAllAvailable message was not consumed.");

    //        var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
    //        Assert.That(currSaga?.Saga.LegalEntityCannotBeConfirmed == true, "LegalEntityCannotBeConfirmed false");
    //        Assert.IsTrue(await harness.Published.Any<ChangeEmpowermentStatus>(f => f.Context.Message.Status == EmpowermentStatementStatus.Unconfirmed), "ChangeEmpowermentStatus message with Unconfirmed Status was not published.");
    //        Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name, $"State is not final but {currSaga?.Saga.CurrentState}");
    //    }
    //    finally
    //    {
    //        await harness.Stop();
    //    }
    //}

    [Test]
    public async Task LegalEntity_CollectsSignaturesAfterSuccessfulBulstatCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<NoDataForLegalEntityInNTRConsumer>();
                    mt.AddConsumer<SucceedingLegalEntityCheckInBulstatConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not published.");
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not consumed.");
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNotPresentInNTR>(), "LegalEntityNotPresentInNTR message was not consumed.");
            Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInBulstat>(), "CheckLegalEntityInBulstat message was not published.");

            Assert.That(await sagaHarness.Consumed.Any<LegalEntityBulstatCheckSucceeded>(), "LegalEntityBulstatCheckSucceeded message was not consumed.");
            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>(), "CollectAuthorizerSignatures message was not published.");

        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task LegalEntity_EmpowermentIsWithdrawnDuringCollectAuthorizerSignaturesAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<NoDataForLegalEntityInNTRConsumer>();
                    mt.AddConsumer<SucceedingLegalEntityCheckInBulstatConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not published.");
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "InitiateEmpowermentActivationProcess message was not consumed.");
            Assert.That(await sagaHarness.Consumed.Any<LegalEntityNotPresentInNTR>(), "LegalEntityNotPresentInNTR message was not consumed.");
            Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInBulstat>(), "CheckLegalEntityInBulstat message was not published.");

            Assert.That(await sagaHarness.Consumed.Any<LegalEntityBulstatCheckSucceeded>(), "LegalEntityBulstatCheckSucceeded message was not consumed.");
            Assert.IsTrue(await harness.Published.Any<CollectAuthorizerSignatures>(), "CollectAuthorizerSignatures message was not published.");

            await harness.Bus.Publish<EmpowermentIsWithdrawn>(new
            {
                command.EmpowermentId
            });

            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).First();
            Assert.Multiple(async () =>
            {
                Assert.That(await harness.Published.Any<EmpowermentIsWithdrawn>(), Is.True);
                Assert.That(await sagaHarness.Consumed.Any<EmpowermentIsWithdrawn>(s => s.Context.Message.EmpowermentId == command.EmpowermentId));
                Assert.That(currSaga.Saga.IsEmpowermentWithdrawn, Is.True);
                Assert.That(currSaga.Saga.CurrentState, Is.EqualTo(sagaHarness.StateMachine.Final.Name));
            });

        }
        finally
        {
            await harness.Stop();
        }
    }


    [Test]
    public async Task LegalEntity_ChecksNTRBeforeAllElseAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<CheckLegalEntityInNTRConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);
            var saga = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.That(await saga.Created.Any(x => x.OriginCorrelationId == command.CorrelationId));

            var sagaWithOriginCorrId = saga.Created.Select(x => x.OriginCorrelationId == command.CorrelationId).First().Saga;
            //var instanceId = await saga.Exists(sagaWithOriginCorrId.CorrelationId, x => x.Final);
            //Assert.That(instanceId, Is.Not.Null);
            Assert.IsTrue(await harness.Published.Any<CheckLegalEntityInNTR>(), "CheckLegalEntityInNTR message was not published.");
            Assert.That(sagaWithOriginCorrId.DenialReason, Is.EqualTo(EmpowermentsDenialReason.None));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task Individual_FinalizedAfterSigningAndNegativeRestrictionCheckAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
                    mt.AddConsumer<SuccessfulSigningConsumer>();
                    mt.AddConsumer<FailingAuthorizerRestrictionsConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Иван Иванов Иванов",
                OnBehalfOf = OnBehalfOf.Individual,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<NoBelowLawfulAgeDetected>());
            Assert.That(await sagaHarness.Consumed.Any<RestrictedUidsDetected>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.DeceasedUid);
            Assert.IsFalse(currSaga?.Saga.SuccessfulCompletion);
        }
        finally
        {
            await harness.Stop();
        }
    }

    //[Test]
    //public async Task Individual_FinalizedAfterSigningAndPositiveRestrictionCheckAsync()
    //{
    //    await using var provider = new ServiceCollection()
    //            .AddMassTransitTestHarness(mt =>
    //            {
    //                mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
    //                mt.AddConsumer<SuccessfulSigningConsumer>();
    //                mt.AddConsumer<SuccessfulAuthorizerRestrictionsConsumer>();
    //                mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
    //                mt.AddConsumer<SuccessfulVerifyUidsRegistrationStatusConsumer>();
    //                mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //            })
    //            .BuildServiceProvider(true);

    //    var harness = provider.GetTestHarness();

    //    await harness.Start();
    //    try
    //    {
    //        var command = new InitiateEmpowermentActivationProcessCommand
    //        {
    //            CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
    //            EmpowermentId = Guid.NewGuid(),
    //            Uid = "8802184852",
    //            Name = "Иван Иванов Иванов",
    //            OnBehalfOf = OnBehalfOf.Individual,
    //            AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            ExpiryDate = null
    //        };
    //        await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

    //        var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //        Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
    //        Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
    //        Assert.That(await sagaHarness.Consumed.Any<SignaturesCollected>(m => command.EmpowermentId == m.Context.Message.EmpowermentId));
    //        Assert.That(await sagaHarness.Consumed.Any<NoRestrictedUidsDetected>(m => command.EmpowermentId == m.Context.Message.EmpowermentId));
    //        Assert.That(await sagaHarness.Consumed.Any<NoBelowLawfulAgeDetected>(m => command.EmpowermentId == m.Context.Message.EmpowermentId));
    //        Assert.That(await sagaHarness.Consumed.Any<RegistrationStatusAllAvailable>(m => command.EmpowermentId == m.Context.Message.EmpowermentId));
    //        var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
    //        Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
    //        Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.None);
    //        Assert.IsTrue(currSaga?.Saga.SuccessfulCompletion);
    //    }
    //    finally
    //    {
    //        await harness.Stop();
    //    }
    //}

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
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
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
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = "8802184852",
                Name = "Иван Иванов Иванов",
                OnBehalfOf = OnBehalfOf.Individual,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
            await adjustment.AdvanceTime(TimeSpan.FromDays(8));
            Assert.That(await sagaHarness.Consumed.Any<EmpowermentActivationTimedOut>(), Is.True);
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.IsFalse(currSaga?.Saga.SuccessfulCompletion);
            Assert.That(currSaga?.Saga.DenialReason, Is.EqualTo(EmpowermentsDenialReason.TimedOut));
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task GetsFinalizedIfTimemestampEmpowermentXmlFailedAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<FailingTimestampEmpowermentXmlConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = $"2542184851", // 20 years guarantee
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "2542184851", UidType = IdentifierType.LNCh } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<TimestampEmpowermentXmlFailed>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.UnsuccessfulTimestamping);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task GetsFinalizedIfBelowLegalAgeUidIsDetectedAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<FailingLawfulAgeVerificationConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = $"2542184851", // 20 years guarantee
                Name = "Фирма",
                IssuerName = "Иван Иванов Иванов",
                IssuerPosition = "Шеф",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "2542184851", UidType = IdentifierType.LNCh } },
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<BelowLawfulAgeDetected>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.BelowLawfulAge);
        }
        finally
        {
            await harness.Stop();
        }
    }

    [Test]
    public async Task GetsFinalizedIfVerifyAgeInfoIsNotAvailableAsync()
    {
        await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(mt =>
                {
                    mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
                    mt.AddConsumer<NotAvailableVerifyUidsLawfulAgeConsumer>();
                    mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
                })
                .BuildServiceProvider(true);

        var harness = provider.GetTestHarness();

        await harness.Start();
        try
        {
            var command = new InitiateEmpowermentActivationProcessCommand
            {
                CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
                EmpowermentId = Guid.NewGuid(),
                Uid = $"2542184851", // 20 years guarantee
                Name = "Име",
                IssuerName = "Име именов",
                OnBehalfOf = OnBehalfOf.Individual,
                AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
                EmpoweredUids = new UserIdentifierData[] { new() { Uid = "63202034795", UidType = IdentifierType.LNCh } }, // Lnch not present in database - NotFound expected
                ExpiryDate = null
            };
            await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

            var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
            Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>());
            Assert.That(await sagaHarness.Consumed.Any<LawfulAgeInfoNotAvailable>());
            var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
            Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
            Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.LawfulAgeInfoNotAvailable);
        }
        finally
        {
            await harness.Stop();
        }
    }


    //[Test]
    //public async Task GetsFinalizedIfInvalidRegistrationStatusDetectedAsync()
    //{
    //    await using var provider = new ServiceCollection()
    //            .AddMassTransitTestHarness(mt =>
    //            {
    //                mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
    //                mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
    //                mt.AddConsumer<SuccessfulSigningConsumer>();
    //                mt.AddConsumer<SuccessfulAuthorizerRestrictionsConsumer>();
    //                mt.AddConsumer<FailingVerifyUidsRegistrationStatusConsumer>();
    //                mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //            })
    //            .BuildServiceProvider(true);

    //    var harness = provider.GetTestHarness();

    //    await harness.Start();
    //    try
    //    {
    //        var command = new InitiateEmpowermentActivationProcessCommand
    //        {
    //            CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
    //            EmpowermentId = Guid.NewGuid(),
    //            Uid = "8802184852",
    //            Name = "Иван Иванов Иванов",
    //            OnBehalfOf = OnBehalfOf.Individual,
    //            AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            ExpiryDate = null
    //        };
    //        await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

    //        var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //        Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "Didn't published InitiateEmpowermentActivationProcess");
    //        Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "Didn't consumed InitiateEmpowermentActivationProcess");
    //        Assert.That(await sagaHarness.Consumed.Any<SignaturesCollected>(m => command.EmpowermentId == m.Context.Message.EmpowermentId), "Didn't consumed SignaturesCollected");
    //        Assert.That(await sagaHarness.Consumed.Any<NoRestrictedUidsDetected>(), "Didn't consumed NoRestrictedUidsDetected");
    //        Assert.That(await sagaHarness.Consumed.Any<NoBelowLawfulAgeDetected>(), "Didn't consumed NoBelowLawfulAgeDetected");
    //        Assert.That(await sagaHarness.Consumed.Any<InvalidRegistrationStatusDetected>(), "Didn't consumed InvalidRegistrationStatusDetected");
    //        var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
    //        Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name, "Not in final state");
    //        Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.InvalidEmpoweredUidRegistrationStatusDetected, "Unexpected denial reason");
    //    }
    //    finally
    //    {
    //        await harness.Stop();
    //    }
    //}

    //[Test]
    //public async Task GetsFinalizedIfRegistrationStatusInfoNotAvailableAsync()
    //{
    //    await using var provider = new ServiceCollection()
    //            .AddMassTransitTestHarness(mt =>
    //            {
    //                mt.AddConsumer<SuccessfulTimestampEmpowermentXmlConsumer>();
    //                mt.AddConsumer<SuccessfulLawfulAgeVerificationConsumer>();
    //                mt.AddConsumer<SuccessfulSigningConsumer>();
    //                mt.AddConsumer<SuccessfulAuthorizerRestrictionsConsumer>();
    //                mt.AddConsumer<NotAvailableVerifyUidsRegistrationStatusConsumer>();
    //                mt.AddSagaStateMachine<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //            })
    //            .BuildServiceProvider(true);

    //    var harness = provider.GetTestHarness();

    //    await harness.Start();
    //    try
    //    {
    //        var command = new InitiateEmpowermentActivationProcessCommand
    //        {
    //            CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
    //            EmpowermentId = Guid.NewGuid(),
    //            Uid = "8802184852",
    //            Name = "Иван Иванов Иванов",
    //            OnBehalfOf = OnBehalfOf.Individual,
    //            AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
    //            ExpiryDate = null
    //        };
    //        await harness.Bus.Publish<InitiateEmpowermentActivationProcess>(command);

    //        var sagaHarness = harness.GetSagaStateMachineHarness<EmpowermentActivationStateMachine, EmpowermentActivationState>();
    //        Assert.IsTrue(await harness.Published.Any<InitiateEmpowermentActivationProcess>(), "Didn't published InitiateEmpowermentActivationProcess");
    //        Assert.That(await sagaHarness.Consumed.Any<InitiateEmpowermentActivationProcess>(), "Didn't consumed InitiateEmpowermentActivationProcess");
    //        Assert.That(await sagaHarness.Consumed.Any<SignaturesCollected>(m => command.EmpowermentId == m.Context.Message.EmpowermentId), "Didn't consumed SignaturesCollected");
    //        Assert.That(await sagaHarness.Consumed.Any<NoRestrictedUidsDetected>(), "Didn't consumed NoRestrictedUidsDetected");
    //        Assert.That(await sagaHarness.Consumed.Any<NoBelowLawfulAgeDetected>(), "Didn't consumed NoBelowLawfulAgeDetected");
    //        Assert.That(await sagaHarness.Consumed.Any<RegistrationStatusInfoNotAvailable>(), "Didn't consumed RegistrationStatusInfoNotAvailable");
    //        var currSaga = sagaHarness.Sagas.Select(q => q.EmpowermentId == command.EmpowermentId).FirstOrDefault();
    //        Assert.That(currSaga?.Saga.CurrentState == sagaHarness.StateMachine.Final.Name);
    //        Assert.That(currSaga?.Saga.DenialReason == EmpowermentsDenialReason.UidsRegistrationStatusInfoNotAvailable);
    //    }
    //    finally
    //    {
    //        await harness.Stop();
    //    }
    //}

    private static readonly object[] _validStatementsTestCases =
    {
     new object[] {
        "IndividualStatement",
        new InitiateEmpowermentActivationProcessCommand
         {
             CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
             EmpowermentId = Guid.NewGuid(),
             Uid = "8802184852",
             Name = "Иван Иванов Иванов",
             OnBehalfOf = OnBehalfOf.Individual,
             AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
             EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
             ExpiryDate = null
         }
     },
     new object[] {
        "LegalEntityStatement",
        new InitiateEmpowermentActivationProcessCommand
        {
            CorrelationId = new Guid("aaaaaaa0-0000-0000-0000-00000000aaaa"),
            EmpowermentId = Guid.NewGuid(),
            Uid = "8802184852",
            Name = "Фирма",
            IssuerName = "Иван Иванов Иванов",
            IssuerPosition = "Шеф",
            OnBehalfOf = OnBehalfOf.LegalEntity,
            AuthorizerUids = new AuthorizerIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
            ExpiryDate = null
        }
     }
    };
    public class CheckLegalEntityInNTRConsumer :
        IConsumer<CheckLegalEntityInNTR>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInNTR> context)
        {
            await context.RespondAsync<LegalEntityNTRCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class FailingCheckLegalEntityInNTRConsumer :
        IConsumer<CheckLegalEntityInNTR>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInNTR> context)
        {
            await context.RespondAsync<LegalEntityNTRCheckFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.NTRCheckFailed
            });
        }
    }
    public class CannotBeConfirmedCheckLegalEntityInNTRConsumer :
        IConsumer<CheckLegalEntityInNTR>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInNTR> context)
        {
            await context.RespondAsync<LegalEntityNTRCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                CanBeConfirmed = false
            });
        }
    }
    public class CanBeConfirmedCheckLegalEntityInNTRConsumer :
        IConsumer<CheckLegalEntityInNTR>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInNTR> context)
        {
            await context.RespondAsync<LegalEntityNTRCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                CanBeConfirmed = true
            });
        }
    }
    public class NoDataForLegalEntityInNTRConsumer :
        IConsumer<CheckLegalEntityInNTR>
    {
        public async Task Consume(ConsumeContext<CheckLegalEntityInNTR> context)
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
    public class SuccessfulSigningConsumer :
        IConsumer<CollectAuthorizerSignatures>
    {
        public async Task Consume(ConsumeContext<CollectAuthorizerSignatures> context)
        {
            await context.RespondAsync<SignaturesCollected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class SuccessfulAuthorizerRestrictionsConsumer :
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
    public class FailingAuthorizerRestrictionsConsumer :
        IConsumer<CheckUidsRestrictions>
    {
        public async Task Consume(ConsumeContext<CheckUidsRestrictions> context)
        {
            await context.RespondAsync<RestrictedUidsDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.DeceasedUid
            });
        }
    }
    public class SuccessfulLawfulAgeVerificationConsumer :
        IConsumer<VerifyUidsLawfulAge>
    {
        public async Task Consume(ConsumeContext<VerifyUidsLawfulAge> context)
        {
            await context.RespondAsync<NoBelowLawfulAgeDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class FailingLawfulAgeVerificationConsumer :
        IConsumer<VerifyUidsLawfulAge>
    {
        public async Task Consume(ConsumeContext<VerifyUidsLawfulAge> context)
        {
            await context.RespondAsync<BelowLawfulAgeDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }

    public class NotAvailableVerifyUidsLawfulAgeConsumer :
        IConsumer<VerifyUidsLawfulAge>
    {
        public async Task Consume(ConsumeContext<VerifyUidsLawfulAge> context)
        {
            await context.RespondAsync<LawfulAgeInfoNotAvailable>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class SuccessfulVerifyUidsRegistrationStatusConsumer :
        IConsumer<VerifyUidsRegistrationStatus>
    {
        public async Task Consume(ConsumeContext<VerifyUidsRegistrationStatus> context)
        {
            await context.RespondAsync<RegistrationStatusAllAvailable>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class FailingVerifyUidsRegistrationStatusConsumer :
        IConsumer<VerifyUidsRegistrationStatus>
    {
        public async Task Consume(ConsumeContext<VerifyUidsRegistrationStatus> context)
        {
            await context.RespondAsync<InvalidRegistrationStatusDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }

    public class NotAvailableVerifyUidsRegistrationStatusConsumer :
        IConsumer<VerifyUidsRegistrationStatus>
    {
        public async Task Consume(ConsumeContext<VerifyUidsRegistrationStatus> context)
        {
            await context.RespondAsync<RegistrationStatusInfoNotAvailable>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class SuccessfulValidateLegalEntityEmpowermentConsumer :
        IConsumer<ValidateLegalEntityEmpowerment>
    {
        public async Task Consume(ConsumeContext<ValidateLegalEntityEmpowerment> context)
        {
            await context.RespondAsync<LegalEntityEmpowermentValidated>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class FailingValidateLegalEntityEmpowermentConsumer :
        IConsumer<ValidateLegalEntityEmpowerment>
    {
        public async Task Consume(ConsumeContext<ValidateLegalEntityEmpowerment> context)
        {
            await context.RespondAsync<LegalEntityEmpowermentValidationFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                DenialReason = EmpowermentsDenialReason.LegalEntityNotActive
            });
        }
    }
    public class SuccessfulTimestampEmpowermentXmlConsumer :
        IConsumer<TimestampEmpowermentXml>
    {
        public async Task Consume(ConsumeContext<TimestampEmpowermentXml> context)
        {
            await context.RespondAsync<TimestampEmpowermentXmlSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
    public class FailingTimestampEmpowermentXmlConsumer :
        IConsumer<TimestampEmpowermentXml>
    {
        public async Task Consume(ConsumeContext<TimestampEmpowermentXml> context)
        {
            await context.RespondAsync<TimestampEmpowermentXmlFailed>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
    }
}

internal class InitiateEmpowermentActivationProcessCommand : InitiateEmpowermentActivationProcess
{
    public Guid CorrelationId { get; set; }
    public Guid EmpowermentId { get; set; }
    public string Uid { get; set; } = string.Empty;
    public IdentifierType UidType { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IssuerPosition { get; set; } = string.Empty;
    public string IssuerName { get; set; } = string.Empty;
    public OnBehalfOf OnBehalfOf { get; set; }
    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; } = Enumerable.Empty<AuthorizerIdentifierData>();
    public IEnumerable<UserIdentifier> EmpoweredUids { get; set; } = Array.Empty<UserIdentifierData>();
    public DateTime? ExpiryDate { get; set; }
}

internal class InitiateSignatureCollectionProcessCommand : CollectAuthorizerSignatures
{
    public Guid CorrelationId { get; set; }
    public Guid EmpowermentId { get; set; }
    public IEnumerable<AuthorizerIdentifier> AuthorizerUids { get; set; } = Array.Empty<AuthorizerIdentifierData>();
    public DateTime SignaturesCollectionDeadline { get; set; }
}
