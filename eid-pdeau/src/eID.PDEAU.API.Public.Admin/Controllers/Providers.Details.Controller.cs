using eID.PDEAU.API.Public.Admin.Authorization;
using eID.PDEAU.API.Public.Admin.Requests;
using eID.PDEAU.Contracts;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PDEAU.API.Public.Admin.Controllers;

/// <summary>
/// Support provider details
/// </summary>
[RoleAuthorization(UserRoles.ApplicationAdministrator + UserRoles.Delimiter + UserRoles.ExternalSystemAdministrator)]
public partial class ProvidersController : BaseV1Controller
{
    /// <summary>
    /// Get all scopes per service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="serviceId"></param>
    /// <returns></returns>
    [HttpGet("services/{serviceId}/scope")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ServiceScopeResult>))]
    public async Task<IActionResult> GetAllServiceScopesAsync(
        [FromServices] IRequestClient<GetAllServiceScopes> client,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var request = new GetAllServiceScopesRequest
        {
            ServiceId = serviceId,
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<ServiceScopeResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.ServiceId,
                    ProviderId
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Update Service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="serviceId"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPut("services/{serviceId}")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    public async Task<IActionResult> UpdateServiceAsync(
        [FromServices] IRequestClient<UpdateService> client,
        [FromRoute] Guid serviceId,
        [FromBody] UpdateServicePayload payload,
        CancellationToken cancellationToken)
    {
        var request = new UpdateServiceRequest
        {
            ProviderDetailsId = payload.ProviderDetailsId,
            ServiceId = serviceId,
            IsEmpowerment = payload.IsEmpowerment,
            ServiceScopeNames = payload.ServiceScopeNames,
            UserId = payload.UserId,
            MinimumLevelOfAssurance = payload.MinimumLevelOfAssurance,
            RequiredPersonalInformation = payload.RequiredPersonalInformation
        };

        var logEventCode = LogEventCode.UPDATE_SERVICE;
        var eventPayload = BeginAuditLog(logEventCode, request,
            (nameof(UpdateServiceRequest.ProviderDetailsId), request.ProviderDetailsId),
            (nameof(UpdateServiceRequest.ServiceId), request.ServiceId),
            (nameof(UpdateServiceRequest.UserId), request.UserId)
        );

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        _ = Guid.TryParse(GetProviderId(), out var providerId);
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = providerId,
                    request.ProviderDetailsId,
                    request.UserId,
                    request.ServiceId,
                    request.IsEmpowerment,
                    request.ServiceScopeNames,
                    request.RequiredPersonalInformation,
                    request.MinimumLevelOfAssurance
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Register service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="payload"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("services")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Guid))]
    public async Task<IActionResult> RegisterServiceAsync(
        [FromServices] IRequestClient<RegisterService> client,
        [FromBody] RegisterServicePayload payload,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(GetProviderId(), out var providerId);
        _ = Guid.TryParse(GetUserId(), out var userId);
        var request = new RegisterServiceRequest
        {
            ProviderId = providerId,
            ProviderDetailsId = payload.ProviderDetailsId,
            IsEmpowerment = payload.IsEmpowerment,
            UserId = userId,
            Description = payload.Description,
            Name = payload.Name,
            PaymentInfoNormalCost = payload.PaymentInfoNormalCost,
            MinimumLevelOfAssurance = payload.MinimumLevelOfAssurance,
            RequiredPersonalInformation = payload.RequiredPersonalInformation,
            ServiceScopeNames = payload.ServiceScopeNames,
        };

        var logEventCode = LogEventCode.REGISTER_SERVICE;
        var eventPayload = BeginAuditLog(logEventCode,
                                         request,
                                         (nameof(RegisterServiceRequest.UserId), request.UserId),
                                         (nameof(RegisterServiceRequest.ProviderId), request.ProviderId),
                                         (nameof(RegisterServiceRequest.ProviderDetailsId), request.ProviderDetailsId));

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<Guid>>(
                new
                {
                    CorrelationId = RequestId,
                    request.ProviderId,
                    request.ProviderDetailsId,
                    request.IsEmpowerment,
                    request.UserId,
                    request.Description,
                    request.Name,
                    request.PaymentInfoNormalCost,
                    request.MinimumLevelOfAssurance,
                    request.RequiredPersonalInformation,
                    request.ServiceScopeNames
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }

    /// <summary>
    /// Get default service scopes
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("services/scope/defaults")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> GetDefaultServiceScopesAsync(
        [FromServices] IRequestClient<GetDefaultServiceScopes> client,
        CancellationToken cancellationToken)
    {
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<string>>>(
                new
                {
                    CorrelationId = RequestId
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Get collection of names already used by your provider
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("services/scope/available")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<string>))]
    public async Task<IActionResult> GetAvailableScopesByProviderIdAsync(
        [FromServices] IRequestClient<GetAvailableScopesByProviderId> client,
        CancellationToken cancellationToken)
    {
        _ = Guid.TryParse(GetProviderId(), out var providerId);
        var request = new GetByIdRequest { Id = providerId };
        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IEnumerable<string>>>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = request.Id
                }, cancellationToken));

        return Result(serviceResult);
    }

    /// <summary>
    /// Activate a service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="serviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("services/{serviceId}/activate")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateServiceAsync(
        [FromServices] IRequestClient<ActivateService> client,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.ACTIVATE_SERVICE;
        _ = Guid.TryParse(GetProviderId(), out var providerId);
        var eventPayload = BeginAuditLog(logEventCode, ("ProviderId", providerId), ("ServiceId", serviceId));
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = providerId,
                    ServiceId = serviceId,
                    UserId = GetUserId(),
                    Uid = GetUid(),
                    UidType = GetUidType()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
    /// <summary>
    /// Deactivate a service
    /// </summary>
    /// <param name="client"></param>
    /// <param name="serviceId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("services/{serviceId}/deactivate")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateServiceAsync(
        [FromServices] IRequestClient<DeactivateService> client,
        [FromRoute] Guid serviceId,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.DEACTIVATE_SERVICE;
        _ = Guid.TryParse(GetProviderId(), out var providerId);
        var eventPayload = BeginAuditLog(logEventCode, ("ProviderId", providerId), ("ServiceId", serviceId));
        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    ProviderId = providerId,
                    ServiceId = serviceId,
                    UserId = GetUserId(),
                    Uid = GetUid(),
                    UidType = GetUidType()
                }, cancellationToken));

        return ResultWithAuditLog(serviceResult, logEventCode, eventPayload);
    }
}
