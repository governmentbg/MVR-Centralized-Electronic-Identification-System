using eID.MIS.Contracts.EP.Commands;
using eID.MIS.Service;
using MassTransit;

namespace eID.MIS.Application.Consumers;

public class PaymentRequestsConsumer : BaseConsumer,
    IConsumer<CreatePaymentRequest>,
    IConsumer<SuspendPaymentRequestInEPayments>,
    IConsumer<GetPaymentRequestStatus>,
    IConsumer<GetPaymentRequests>,
    IConsumer<GetClientsByEik>
{
    private readonly PaymentRequestsService _paymentRequestsService;

    public PaymentRequestsConsumer(
        ILogger<PaymentRequestsConsumer> logger,
        PaymentRequestsService paymentRequestsService) : base(logger)
    {
        _paymentRequestsService = paymentRequestsService ?? throw new ArgumentNullException(nameof(paymentRequestsService));
    }

    public async Task Consume(ConsumeContext<CreatePaymentRequest> context)
    {
        await ExecuteMethodAsync(context, () => _paymentRequestsService.CreatePaymentRequestAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SuspendPaymentRequestInEPayments> context)
    {
        using (Logger.BeginScope("{CorrelationId}", context.Message.CorrelationId))
        {
            var result = await _paymentRequestsService.SuspendPaymentRequestInEPaymentsAsync(context.Message);
            if (result.StatusCode == System.Net.HttpStatusCode.BadGateway)
            {
                throw new SuspendPaymentException("Unsuccessful suspend payment request.");
            }
        }
    }

    public async Task Consume(ConsumeContext<GetPaymentRequestStatus> context)
    {
        await ExecuteMethodAsync(context, () => _paymentRequestsService.GetPaymentRequestStatusAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetPaymentRequests> context)
    {
        await ExecuteMethodAsync(context, () => _paymentRequestsService.GetPaymentRequestsAsync(context.Message));
    }
    public async Task Consume(ConsumeContext<GetClientsByEik> context)
    {
        await ExecuteMethodAsync(context, () => _paymentRequestsService.GetClientsByEikAsync(context.Message));
    }
}

public class PaymentRequestsConsumerDefinition : ConsumerDefinition<PaymentRequestsConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator, IConsumerConfigurator<PaymentRequestsConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseScheduledRedelivery(r => r.Intervals(TimeSpan.FromHours(1), TimeSpan.FromHours(12), TimeSpan.FromHours(24)));
        endpointConfigurator.UseMessageRetry(r =>
        {
            r.Handle<SuspendPaymentException>();
            r.Interval(5, TimeSpan.FromSeconds(10));
        });

        endpointConfigurator.DiscardSkippedMessages();
    }
}

public class SuspendPaymentException : Exception
{
    public SuspendPaymentException(string message) : base(message) { }
}
