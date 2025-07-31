using System.Net;
using eID.Signing.Contracts.Results;
using MassTransit;

namespace eID.Signing.Application.Consumers
{
    public class BaseConsumer
    {
        protected ILogger Logger { get; private set; }

        public BaseConsumer(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteMethodAsync<TCmd, TResult>(ConsumeContext<TCmd> context, Func<Task<ServiceResult<TResult>>> action)
            where TCmd : class, CorrelatedBy<Guid>
        {
            using (Logger.BeginScope("{CorrelationId}", context.Message.CorrelationId))
            {
                Logger.LogInformation("Received message {MessageName}", typeof(TCmd).Name);

                try
                {
                    ServiceResult<TResult> result = await action();

                    await context.RespondAsync(result);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Exception occurred when execute '{nameof(ExecuteMethodAsync)}'");
                    await context.RespondAsync(new ServiceResult<TResult> { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" });
                }
            }
        }

        public async Task ExecuteMethodAsync<TCmd>(ConsumeContext<TCmd> context, Func<Task<ServiceResult>> action)
            where TCmd : class, CorrelatedBy<Guid>
        {
            using (Logger.BeginScope("{CorrelationId}", context.Message.CorrelationId))
            {
                Logger.LogInformation("Received message {MessageName}", typeof(TCmd).Name);
                try
                {
                    ServiceResult result = await action();

                    await context.RespondAsync(result);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Exception occurred when execute '{nameof(ExecuteMethodAsync)}'");
                    await context.RespondAsync(new ServiceResult { StatusCode = HttpStatusCode.InternalServerError, Error = "Unhandled exception" });
                }
            }
        }
    }
}
