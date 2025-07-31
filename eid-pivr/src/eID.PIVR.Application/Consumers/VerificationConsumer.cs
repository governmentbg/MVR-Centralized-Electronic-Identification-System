using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using MassTransit;

namespace eID.PIVR.Application.Consumers;

public class VerificationConsumer : BaseConsumer,
    IConsumer<VerifySignature>
{
    private readonly VerificationService _verificationService;

    public VerificationConsumer(ILogger<VerificationConsumer> logger, VerificationService verificationService) : base(logger)
    {
        _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
    }

    public async Task Consume(ConsumeContext<VerifySignature> context)
    {
        await ExecuteMethodAsync(context, () => _verificationService.VerifySignatureAsync(context.Message));
    }
}
