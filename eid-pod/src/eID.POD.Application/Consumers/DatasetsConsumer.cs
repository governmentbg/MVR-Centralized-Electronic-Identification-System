using eID.POD.Contracts.Commands;
using eID.POD.Service;
using MassTransit;

namespace eID.POD.Application.Consumers;

public class DatasetsConsumer : BaseConsumer,
     IConsumer<CreateDataset>,
     IConsumer<GetAllDatasets>,
     IConsumer<UpdateDataset>,
     IConsumer<DeleteDataset>,
     IConsumer<ManualUploadDataset>
{
    private readonly DatasetsService _datasetsService;

    public DatasetsConsumer(
        ILogger<DatasetsConsumer> logger,
        DatasetsService datasetsService) : base(logger)
    {
        _datasetsService = datasetsService ?? throw new ArgumentNullException(nameof(datasetsService));
    }

    public async Task Consume(ConsumeContext<CreateDataset> context)
    {
        await ExecuteMethodAsync(context, () => _datasetsService.CreateDatasetAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<GetAllDatasets> context)
    {
        await ExecuteMethodAsync(context, () => _datasetsService.GetAllDatasetsAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<UpdateDataset> context)
    {
        await ExecuteMethodAsync(context, () => _datasetsService.UpdateDatasetAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<DeleteDataset> context)
    {
        await ExecuteMethodAsync(context, () => _datasetsService.DeleteDatasetAsync(context.Message));
    }

    public async Task Consume(ConsumeContext<ManualUploadDataset> context)
    {
        await ExecuteMethodAsync(context, () => _datasetsService.ManualUploadDatasetAsync(context.Message));
    }
}
