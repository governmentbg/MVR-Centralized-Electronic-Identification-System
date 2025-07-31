using eID.RO.Application.Options;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Service;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Requests;
using MassTransit;
using Microsoft.Extensions.Options;

namespace eID.RO.Application.Consumers
{
    public class CheckLegalEntityInBulstatConsumer : BaseConsumer,
        IConsumer<CheckLegalEntityInBulstat>
    {
        private readonly IVerificationService _verificationService;
        private readonly EmpowermentsService _empowermentsService;
        private readonly ApplicationUrls _applicationUrls;

        public CheckLegalEntityInBulstatConsumer(
            ILogger<CheckLegalEntityInBulstatConsumer> logger,
            IOptions<ApplicationUrls> applicationUrls,
            IVerificationService verificationService,
            EmpowermentsService empowermentsService) : base(logger)
        {
            _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
            _applicationUrls.Validate();
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
            _empowermentsService = empowermentsService ?? throw new ArgumentNullException(nameof(empowermentsService));
        }

        public async Task Consume(ConsumeContext<CheckLegalEntityInBulstat> context)
        {
            Logger.LogInformation("Checking Empowerment {EmpowermentId} in Bulstat.", context.Message.EmpowermentId);
            var authorizerUids = await _empowermentsService.GetAuthorizersByEmpowermentStatementIdAsync(context.Message.EmpowermentId);
            if (authorizerUids is null || !authorizerUids.Any())
            {
                Logger.LogInformation("No authorizers found for Empowerment {EmpowermentId}", context.Message.EmpowermentId);
                await context.RespondAsync<LegalEntityBulstatCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.EmpowermentStatementNotFound
                });
                return;
            }

            var bulstatVerificationResult = await _verificationService.CheckLegalEntityInBulstatAsync(new CheckLegalEntityInBulstatRequest
            {
                CorrelationId = context.Message.CorrelationId,
                AuthorizerUids = authorizerUids,
                Uid = context.Message.Uid
            });
            if (bulstatVerificationResult is null)
            {

                Logger.LogInformation("Null CheckLegalEntityInBulstatAsync response for Empowerment {EmpowermentId}.", context.Message.EmpowermentId);
                await context.RespondAsync<LegalEntityBulstatCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.BulstatCheckFailed
                });
                return;
            }
            if (bulstatVerificationResult.StatusCode != System.Net.HttpStatusCode.OK)
            {

                Logger.LogInformation("Bad CheckLegalEntityInBulstatAsync response for Empowerment {EmpowermentId}. Status Code: {StatusCode}; Error: {Error}; Errors: {Errors}", 
                    context.Message.EmpowermentId,
                    bulstatVerificationResult.StatusCode,
                    bulstatVerificationResult.Error,
                    bulstatVerificationResult.Errors
                );
                await context.RespondAsync<LegalEntityBulstatCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.BulstatCheckFailed
                });
                return;
            }
            if (bulstatVerificationResult.Result is null)
            {
                Logger.LogInformation("CheckLegalEntityInBulstatAsync returned null response for Empowerment {EmpowermentId}", context.Message.EmpowermentId);
                await context.RespondAsync<LegalEntityBulstatCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = EmpowermentsDenialReason.BulstatCheckFailed
                });
                return;
            }
            if (!bulstatVerificationResult.Result.Successful)
            {
                await context.RespondAsync<LegalEntityBulstatCheckFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    DenialReason = bulstatVerificationResult.Result.DenialReason
                });
                return;
            }

            await context.RespondAsync<LegalEntityBulstatCheckSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }

        public class CheckLegalEntityInBulstatConsumerDefinition : ConsumerDefinition<CheckLegalEntityInBulstatConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CheckLegalEntityInBulstatConsumer> consumerConfigurator)
            {
                endpointConfigurator.DiscardFaultedMessages();
                endpointConfigurator.DiscardSkippedMessages();
            }
        }
    }
}
