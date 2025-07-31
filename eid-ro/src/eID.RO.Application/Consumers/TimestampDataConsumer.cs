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
    public class TimestampDataConsumer : BaseConsumer,
        IConsumer<TimestampEmpowermentXml>,
        IConsumer<TimestampEmpowermentWithdrawal>
    {
        private readonly IVerificationService _verificationService;
        private readonly EmpowermentsService _empowermentsService;
        private readonly ApplicationUrls _applicationUrls;

        public TimestampDataConsumer(
            ILogger<TimestampDataConsumer> logger,
            IOptions<ApplicationUrls> applicationUrls,
            IVerificationService verificationService,
            EmpowermentsService empowermentsService) : base(logger)
        {
            _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
            _applicationUrls.Validate();
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
            _empowermentsService = empowermentsService ?? throw new ArgumentNullException(nameof(empowermentsService));
        }

        public async Task Consume(ConsumeContext<TimestampEmpowermentXml> context)
        {
            var xmlTimestampingResult = await _empowermentsService.TimestampEmpowermentXmlAsync(context.Message.CorrelationId, context.Message.EmpowermentId);
            if (xmlTimestampingResult is null)
            {
                Logger.LogInformation("Null {MethodName} response for Empowerment {EmpowermentId}.", nameof(EmpowermentsService.TimestampEmpowermentXmlAsync), context.Message.EmpowermentId);
                await context.RespondAsync<TimestampEmpowermentXmlFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }
            if (xmlTimestampingResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Logger.LogInformation("Bad {MethodName} response for Empowerment {EmpowermentId}. Status Code: {StatusCode}; Error: {Error}; Errors: {Errors}",
                    nameof(EmpowermentsService.TimestampEmpowermentXmlAsync),
                    context.Message.EmpowermentId,
                    xmlTimestampingResult.StatusCode,
                    xmlTimestampingResult.Error,
                    xmlTimestampingResult.Errors
                );
                await context.RespondAsync<TimestampEmpowermentXmlFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }
            if (xmlTimestampingResult.Result is null)
            {
                Logger.LogInformation("{MethodName} returned null response for Empowerment {EmpowermentId}", nameof(EmpowermentsService.TimestampEmpowermentXmlAsync), context.Message.EmpowermentId);
                await context.RespondAsync<TimestampEmpowermentXmlFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }
            if (!xmlTimestampingResult.Result.Successful)
            {
                await context.RespondAsync<TimestampEmpowermentXmlFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId
                });
                return;
            }

            await context.RespondAsync<TimestampEmpowermentXmlSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId
            });
        }
        public async Task Consume(ConsumeContext<TimestampEmpowermentWithdrawal> context)
        {
            var withdrawalTimestampingResult = await _empowermentsService.TimestampEmpowermentWithdrawalAsync(context.Message.CorrelationId, context.Message.EmpowermentId, context.Message.EmpowermentWithdrawalId);
            if (withdrawalTimestampingResult is null)
            {
                Logger.LogInformation("Null {MethodName} response for Empowerment {EmpowermentId} Withdrawal {WithdrawalId}.", nameof(EmpowermentsService.TimestampEmpowermentWithdrawalAsync), context.Message.EmpowermentId, context.Message.EmpowermentWithdrawalId);
                await context.RespondAsync<TimestampEmpowermentWithdrawalFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    context.Message.EmpowermentWithdrawalId
                });
                return;
            }
            if (withdrawalTimestampingResult.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Logger.LogInformation("Bad {MethodName} response for Empowerment {EmpowermentId}  Withdrawal {WithdrawalId}. Status Code: {StatusCode}; Error: {Error}; Errors: {Errors}",
                    nameof(EmpowermentsService.TimestampEmpowermentWithdrawalAsync),
                    context.Message.EmpowermentId,
                    context.Message.EmpowermentWithdrawalId,
                    withdrawalTimestampingResult.StatusCode,
                    withdrawalTimestampingResult.Error,
                    withdrawalTimestampingResult.Errors
                );
                await context.RespondAsync<TimestampEmpowermentWithdrawalFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    context.Message.EmpowermentWithdrawalId
                });
                return;
            }
            if (withdrawalTimestampingResult.Result is null)
            {
                Logger.LogInformation("{MethodName} returned null response for Empowerment {EmpowermentId}  Withdrawal {WithdrawalId}", nameof(EmpowermentsService.TimestampEmpowermentWithdrawalAsync), context.Message.EmpowermentId, context.Message.EmpowermentWithdrawalId);
                await context.RespondAsync<TimestampEmpowermentWithdrawalFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    context.Message.EmpowermentWithdrawalId
                });
                return;
            }
            if (!withdrawalTimestampingResult.Result.Successful)
            {
                await context.RespondAsync<TimestampEmpowermentWithdrawalFailed>(new
                {
                    context.Message.CorrelationId,
                    context.Message.EmpowermentId,
                    context.Message.EmpowermentWithdrawalId
                });
                return;
            }

            await context.RespondAsync<TimestampEmpowermentWithdrawalSucceeded>(new
            {
                context.Message.CorrelationId,
                context.Message.EmpowermentId,
                context.Message.EmpowermentWithdrawalId
            });
        }

        public class TimestampDataConsumerDefinition : ConsumerDefinition<TimestampDataConsumer>
        {
            protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<TimestampDataConsumer> consumerConfigurator)
            {
                endpointConfigurator.DiscardFaultedMessages();
                endpointConfigurator.DiscardSkippedMessages();
            }
        }
    }
}
