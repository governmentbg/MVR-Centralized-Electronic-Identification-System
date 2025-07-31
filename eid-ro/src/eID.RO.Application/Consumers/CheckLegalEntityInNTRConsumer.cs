using eID.RO.Application.Options;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Service;
using eID.RO.Service.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;

namespace eID.RO.Application.Consumers
{
    public class CheckLegalEntityInNTRConsumer : BaseConsumer,
        IConsumer<CheckLegalEntityInNTR>
    {
        private readonly IVerificationService _verificationService;
        private readonly EmpowermentsService _empowermentsService;
        private readonly ApplicationUrls _applicationUrls;

        public CheckLegalEntityInNTRConsumer(
            ILogger<CheckLegalEntityInNTRConsumer> logger,
            IOptions<ApplicationUrls> applicationUrls,
            IVerificationService verificationService,
            EmpowermentsService empowermentsService) : base(logger)
        {
            _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
            _applicationUrls.Validate();
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
            _empowermentsService = empowermentsService ?? throw new ArgumentNullException(nameof(empowermentsService));
        }

        public async Task Consume(ConsumeContext<CheckLegalEntityInNTR> context)
        {
            var empowermentStatement = await _empowermentsService.GetEmpowermentStatementByIdAsync(context.Message.EmpowermentId);
            if (empowermentStatement is null)
            {
                await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.EmpowermentStatementNotFound
                });

                return;
            }

            var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(context.Message.CorrelationId, context.Message.Uid);
            //Error while getting Legal Entity actual state in TR
            if (legalEntityActualState is null || legalEntityActualState.Result is null || legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed is null)
            {
                await context.RespondAsync<LegalEntityNotPresentInNTR>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    MissingOrMalformedResponse = true
                });
                return;
            }

            var legalEntity = legalEntityActualState.Result;
            var representativesDataIsAvailable = legalEntity.ContainsRepresentativesData();

            //Confirm Authorizers in Legal Entity
            if (representativesDataIsAvailable)
            {
                var moreAuthorizersThanRepersenters = empowermentStatement.AuthorizerUids.Count() > legalEntity.GetRepresentatives().Count();
                if (moreAuthorizersThanRepersenters)
                {
                    await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                    {
                        context.Message.CorrelationId,
                        context.Message.EmpowermentId,
                        DenialReason = EmpowermentsDenialReason.LegalEntityRepresentationNotMatch
                    });

                    return;
                }

                var someAuthorizersAreNotRepresenters = empowermentStatement.AuthorizerUids.Any(authorizer => !legalEntity.IsAmongRepresentatives(authorizer.Uid, authorizer.Name));
                if(someAuthorizersAreNotRepresenters)
                {
                    await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                    {
                        context.Message.CorrelationId,
                        context.Message.EmpowermentId,
                        DenialReason = EmpowermentsDenialReason.LegalEntityRepresentationNotMatch
                    });

                    return;
                }
            } 

            if (legalEntity.HasValidDeedStatus())
            {
                if (legalEntity.IsToBeCheckedInBulstat())
                {
                    await context.RespondAsync<LegalEntityNotPresentInNTR>(new
                    {
                        context.Message.CorrelationId,
                        context.Message.EmpowermentId
                    });
                    return;
                }
                //Not active - Legal Entity is not active in TR and we have to deny the empowerment
                //Empowerment status: Denied with Reason: LegalEntityNotActive
                if (legalEntity.IsInactive())
                {
                    await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                    {
                        context.Message.CorrelationId,
                        context.Message.EmpowermentId,
                        DenialReason = EmpowermentsDenialReason.LegalEntityNotActive
                    });
                    return;
                }
            }

            //Confirm legal entity name and issuer
            var incorrectCompanyData = !legalEntity.MatchCompanyData(context.Message.Name, context.Message.Uid);
            var issuerIsRepresenter = legalEntity.IsAmongRepresentatives(context.Message.IssuerUid, context.Message.IssuerName);
            if (incorrectCompanyData || (representativesDataIsAvailable && !issuerIsRepresenter))
            {
                await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.NTRCheckFailed
                });
                return;
            }

            var representativesCount = legalEntity.GetRepresentatives().Count();
            var authorizersCount = empowermentStatement.AuthorizerUids.Count();
            var wayOfRepresentation = legalEntity.GetWayOfRepresentation();
            var issuerIsSoleRepresenter = representativesCount == 1 && issuerIsRepresenter;
            var issuerIsAllowedToRepresentOnHisOwn = issuerIsRepresenter && wayOfRepresentation.Severally;
            var allAuthorizersArePresent = authorizersCount == representativesCount;

            if (wayOfRepresentation.Jointly && !allAuthorizersArePresent)
            {
                await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.LegalEntityRepresentationNotMatch
                });

                return;
            }

            await context.RespondAsync<LegalEntityNTRCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                CanBeConfirmed = issuerIsSoleRepresenter || issuerIsAllowedToRepresentOnHisOwn
            });

        }
        public class CheckLegalEntityInNTRConsumerDefinition : ConsumerDefinition<CheckLegalEntityInNTRConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CheckLegalEntityInNTRConsumer> consumerConfigurator)
            {
                endpointConfigurator.DiscardFaultedMessages();
                endpointConfigurator.DiscardSkippedMessages();
            }
        }
    }
}
