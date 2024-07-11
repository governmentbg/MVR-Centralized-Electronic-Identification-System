using eID.PJS.AuditLogging;
using eID.RO.API.Public.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eID.RO.API.Public.Controllers;

public class DeauController : BaseV1Controller
{
    public DeauController(IConfiguration configuration, ILogger<DeauController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// This endpoint will validate Deau and search for a Empowerments based on filter
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("empowerments", Name = nameof(GetEmpowermentsByDeauAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<EmpowermentStatementResult>))]
    public async Task<IActionResult> GetEmpowermentsByDeauAsync(
        [FromServices] IRequestClient<GetEmpowermentsByDeau> client,
        [FromBody] GetEmpowermentsByDeauRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.OnBehalfOf,
                    request.AuthorizerUid,
                    request.AuthorizerUidType,
                    request.EmpoweredUid,
                    request.EmpoweredUidType,
                    SupplierId = GetSupplierId(),
                    RequesterUid = GetUid(),
                    request.ServiceId,
                    request.VolumeOfRepresentation,
                    request.StatusOn,
                    request.PageSize,
                    request.PageIndex
                }, cancellationToken));

        if (serviceResult.Result?.Data.Any() == true)
        {
            foreach (var eID in serviceResult.Result.Data.Select(r => r.CreatedBy).Distinct())
            {
                AddAuditLog(LogEventCode.GetEmpowermentsByDeau, eID);
            }
        }

        return Result(serviceResult);
    }

    /// <summary>
    /// Deny both Active and Unconfirmed empowerments
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("deny-empowerment", Name = nameof(DenyEmpowermentByDeauAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DenyEmpowermentByDeauAsync(
        [FromServices] IRequestClient<DenyEmpowermentByDeau> client,
        [FromBody] DenyEmpowermentByDeauRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    AdministrationId = GetSupplierId(),
                    request.EmpowermentId,
                    request.DenialReasonComment
                }, cancellationToken));

        AddAuditLog(LogEventCode.DenyEmpowermentByDeau, payload: new SortedDictionary<string, object>
            {
                { nameof(request.EmpowermentId), request.EmpowermentId },
                { "AdministratorId", GetUserId() },
                { nameof(DenyEmpowermentByDeau.AdministrationId), GetSupplierId() ?? "Unable to obtain AdministrationId" }
            });

        return Result(serviceResult);
    }

    /// <summary>
    /// Approve Unconfirmed empowerment
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("approve-empowerment", Name = nameof(ApproveEmpowermentByDeauAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ApproveEmpowermentByDeauAsync(
        [FromServices] IRequestClient<ApproveEmpowermentByDeau> client,
        [FromBody] ApproveEmpowermentByDeauRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    AdministrationId = GetSupplierId(),
                    request.EmpowermentId
                }, cancellationToken));

        AddAuditLog(LogEventCode.ApproveEmpowermentByDeau, payload: new SortedDictionary<string, object>
            {
                { nameof(request.EmpowermentId), request.EmpowermentId },
                { "AdministratorId", GetUserId() },
                { nameof(ApproveEmpowermentByDeau.AdministrationId), GetSupplierId() ?? "Unable to obtain AdministrationId" }
            });

        return Result(serviceResult);
    }
}
