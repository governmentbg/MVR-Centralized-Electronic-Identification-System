using eID.RO.Contracts.Commands;
using eID.RO.Service;
using MassTransit;

namespace eID.RO.Application.Consumers;

public class EmpowermentConsumer : BaseConsumer,
    IConsumer<AddEmpowermentStatement>,
    IConsumer<GetEmpowermentsToMeByFilter>,
    IConsumer<GetEmpowermentsFromMeByFilter>,
    IConsumer<ChangeEmpowermentStatus>,
    IConsumer<GetEmpowermentWithdrawReasons>,
    IConsumer<WithdrawEmpowerment>,
    IConsumer<ChangeEmpowermentWithdrawalStatus>,
    IConsumer<GetEmpowermentDisagreementReasons>,
    IConsumer<DisagreeEmpowerment>,
    IConsumer<GetEmpowermentsByDeau>,
    IConsumer<DenyEmpowermentByDeau>,
    IConsumer<ApproveEmpowermentByDeau>,
    IConsumer<SignEmpowerment>,
    IConsumer<GetEmpowermentsByEik>
{
    private readonly EmpowermentsService _empowermentsService;

    public EmpowermentConsumer(
        ILogger<EmpowermentConsumer> logger,
        EmpowermentsService empowermentsService) : base(logger)
    {
        _empowermentsService = empowermentsService ?? throw new ArgumentNullException(nameof(empowermentsService));
    }

    public async Task Consume(ConsumeContext<AddEmpowermentStatement> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.CreateStatementAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetEmpowermentsToMeByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.GetEmpowermentsToMeByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ChangeEmpowermentStatus> context)
    {
        await ExecuteMethodWithoutResponseAsync(context, () => _empowermentsService.ChangeEmpowermentStatusAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetEmpowermentsFromMeByFilter> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.GetEmpowermentsFromMeByFilterAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetEmpowermentWithdrawReasons> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.GetEmpowermentWithdrawReasonsAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<WithdrawEmpowerment> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.WithdrawEmpowermentAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ChangeEmpowermentWithdrawalStatus> context)
    {
        await ExecuteMethodWithoutResponseAsync(context, () => _empowermentsService.ChangeEmpowermentsWithdrawStatusAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetEmpowermentDisagreementReasons> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.GetEmpowermentDisagreementReasonsAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<DisagreeEmpowerment> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.DisagreeEmpowermentAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetEmpowermentsByDeau> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.GetEmpowermentsByDeauAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<DenyEmpowermentByDeau> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.DenyEmpowermentByDeauAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ApproveEmpowermentByDeau> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.ApproveEmpowermentByDeauAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<SignEmpowerment> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.SignEmpowermentAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetEmpowermentsByEik> context)
    {
        await ExecuteMethodAsync(context, () => _empowermentsService.GetEmpowermentsByEikAsync(context.Message));
    }
}
