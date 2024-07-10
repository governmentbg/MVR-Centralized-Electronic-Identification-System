using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Events;
using eID.RO.Service.Interfaces;
using MassTransit;

namespace eID.RO.Application.Consumers;

public class CheckUidsRestrictionsConsumer : BaseConsumer,
    IConsumer<CheckUidsRestrictions>,
    IConsumer<VerifyUidsLawfulAge>
{
    private readonly IVerificationService _verificationService;
    public CheckUidsRestrictionsConsumer(
        ILogger<CheckUidsRestrictionsConsumer> logger,
        IVerificationService verificationService) : base(logger)
    {
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
    }

    public async Task Consume(ConsumeContext<CheckUidsRestrictions> context)
    {
        var serviceResult = await _verificationService.CheckUidsRestrictionsAsync(context.Message);

        if (context.Message.RespondWithRawServiceResult)
        {
            await context.RespondAsync(serviceResult);
            return;
        }

        if (serviceResult?.Result != null && serviceResult.Result.Successfull)
        {
            await context.RespondAsync<NoRestrictedUidsDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
        else
        {
            await context.RespondAsync<RestrictedUidsDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                serviceResult?.Result?.DenialReason
            });
        }
    }
    public async Task Consume(ConsumeContext<VerifyUidsLawfulAge> context)
    {
        var serviceResult = await _verificationService.VerifyUidsLawfulAgeAsync(context.Message);

        if (serviceResult?.Result != null && serviceResult.Result)
        {
            await context.RespondAsync<NoBelowLawfulAgeDetected>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
        else
        {
            var noDataStatuses = new[] {
                System.Net.HttpStatusCode.NotFound, // No response from Regix
                System.Net.HttpStatusCode.InternalServerError // Communication error or malformed response
            };
            if (noDataStatuses.Any(status => status == serviceResult?.StatusCode))
            {
                await context.RespondAsync<LawfulAgeInfoNotAvailable>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
            }
            else
            {
                await context.RespondAsync<BelowLawfulAgeDetected>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
            }
        }
    }
    public class CheckAuthorizersForRestrictionsConsumerDefinition : ConsumerDefinition<CheckUidsRestrictionsConsumer>
    {
        protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<CheckUidsRestrictionsConsumer> consumerConfigurator)
        {
            endpointConfigurator.DiscardFaultedMessages();
            endpointConfigurator.DiscardSkippedMessages();
        }
    }
}
