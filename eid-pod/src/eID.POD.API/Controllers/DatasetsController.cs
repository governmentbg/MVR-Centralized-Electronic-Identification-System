using eID.PJS.AuditLogging;
using eID.POD.API.Requests;
using eID.POD.Contracts;
using eID.POD.Contracts.Commands;
using eID.POD.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.POD.API.Controllers;

public class DatasetsController : BaseV1Controller
{
    public DatasetsController(
        IConfiguration configuration,
        ILogger<DatasetsController> logger,
        AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {

    }

    /// <summary>
    /// Create dataset in eID and OpenData portal and links them.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Id of created dataset in eID</returns>
    [HttpPost(Name = nameof(CreateDatasetAsync))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ServiceResult<Guid>))]
    public async Task<IActionResult> CreateDatasetAsync(
        [FromServices] IRequestClient<CreateDataset> client,
        [FromBody] CreateDatasetRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var userId = GetUserId();

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    request.DatasetName,
                    request.CronPeriod,
                    request.DataSource,
                    request.IsActive,
                    CreatedBy = userId,
                }, cancellationToken));

        AddAuditLog(LogEventCode.CreateDataset, userId);

        return Result(serviceResult);
    }

    /// <summary>
    /// Get all datasets
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>List of all datasets</returns>
    [HttpGet(Name = nameof(GetAllDatasetsAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DatasetResult>))]
    public async Task<IActionResult> GetAllDatasetsAsync(
        [FromServices] IRequestClient<GetAllDatasets> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<DatasetResult>>>(
                new
                {
                    CorrelationId = RequestId,
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetAllDatasets, GetUserId());

        return Result(serviceResult);
    }

    /// <summary>
    /// Update dataset and schedule/reschedule job depending on IsActive flag.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPut("{id}", Name = nameof(UpdateDatasetAsync))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateDatasetAsync(
        [FromServices] IRequestClient<UpdateDataset> client,
        [FromRoute] Guid id,
        [FromBody] UpdateDatasetRequest request,
        CancellationToken cancellationToken)
    {
        request.Id = id;

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var userId = GetUserId();

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                    request.DatasetName,
                    request.CronPeriod,
                    request.DataSource,
                    request.IsActive,
                    LastModifiedBy = userId
                }, cancellationToken));

        AddAuditLog(LogEventCode.UpdateDataset, userId);

        return Result(serviceResult);
    }

    /// <summary>
    /// Deletes dataset and removes any scheduled jobs for the given dataset.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpDelete("{id}", Name = nameof(DeleteDatasetAsync))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteDatasetAsync(
        [FromServices] IRequestClient<DeleteDataset> client,
        [FromRoute] DeleteDatasetRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var userId = GetUserId();

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                    LastModifiedBy = userId
                }, cancellationToken));

        AddAuditLog(LogEventCode.DeleteDataset, userId);

        return Result(serviceResult);
    }

    /// <summary>
    /// Manually uploads a dataset
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("{id}/upload", Name = nameof(ManualUploadDatasetAsync))]
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(ServiceResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ManualUploadDatasetAsync(
        [FromServices] IRequestClient<ManualUploadDataset> client,
        [FromRoute] ManualUploadDatasetRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id,
                }, cancellationToken));

        AddAuditLog(LogEventCode.ManualUploadDataset, GetUserId());

        return Result(serviceResult);
    }
}
