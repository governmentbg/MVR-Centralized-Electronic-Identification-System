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

        //At this point we know those statuses:
        //N - нова
        //Е - Пререгистрирана фирма по Булстат
        //C - Нова партида затворена
        //L - Пререгистрирана фирма по Булстат затворена
        private readonly string[] _notActiveStatuses = { "C", "L" };
        private readonly string[] _checkInBulstatStatuses = { "E", "L" };

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

            var legalEntityActualState = await _verificationService.GetLegalEntityActualStateAsync(context.Message.Uid);

            //Error while getting Legal Entity actual state in TR
            if (legalEntityActualState is null || legalEntityActualState.Result is null || legalEntityActualState?.Result?.Response?.ActualStateResponseV3?.Deed is null)
            {
                await context.RespondAsync<LegalEntityNotPresentInNTR>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }

            //Confirm Authorizers in Legal Entity
            var validAuthorizers = await _empowermentsService.ConfirmAuthorizersInLegalEntityRepresentationAsync(context.Message.EmpowermentId, legalEntityActualState.Result);
            if (!validAuthorizers)
            {
                await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.LegalEntityRepresentationNotMatch
                });

                return;
            }

            //Not active - Legal Entity is not active in TR and we have to deny the empowerment
            //Empowerment status: Denied with Reason: LegalEntityNotActive
            var legalEntityState = legalEntityActualState.Result.Response;
            var legalEntityStatus = legalEntityState.ActualStateResponseV3?.Deed?.DeedStatus;
            if (!string.IsNullOrWhiteSpace(legalEntityStatus))
            {
                if (_checkInBulstatStatuses.Contains(legalEntityStatus))
                {
                    await context.RespondAsync<LegalEntityNotPresentInNTR>(new
                    {
                        context.Message.CorrelationId,
                        context.Message.EmpowermentId
                    });
                    return;
                }
                if (_notActiveStatuses.Contains(legalEntityStatus))
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

            //Confirm requester and Legal Entity name
            var serviceResult = VerificationService.CalculateVerificationResult(context.Message, legalEntityActualState.Result);
            if (!serviceResult.Successfull)
            {
                await context.RespondAsync<LegalEntityNTRCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.NTRCheckFailed
                });
                return;
            }
            await context.RespondAsync<LegalEntityNTRCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
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
