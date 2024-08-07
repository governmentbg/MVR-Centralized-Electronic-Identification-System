﻿using eID.PJS.AuditLogging;
using eID.PAN.API.Requests;
using eID.PAN.Contracts;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace eID.PAN.API.Controllers;

public class SmtpConfigurationsController : BaseV1Controller
{
    public SmtpConfigurationsController(
        IConfiguration configuration,
        ILogger<SmtpConfigurationsController> logger,
        AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {

    }

    /// <summary>
    /// Create Smtp configuration
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> CreateSmtpConfigurationAsync(
        [FromServices] IRequestClient<CreateSmtpConfiguration> client,
        CancellationToken cancellationToken,
        [FromBody] CreateSmtpConfigurationRequest request)
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
                    request.Server,
                    request.Port,
                    request.SecurityProtocol,
                    request.UserName,
                    request.Password,
                    UserId = GetUserId(),
                }, cancellationToken));

        AddAuditLog(LogEventCode.CreateSmtpConfiguration);

        return Result(serviceResult);
    }

    /// <summary>
    /// Get all Smtp configurations
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<SmtpConfigurationResult>))]
    public async Task<IActionResult> GetAllSmtpConfigurationsAsync(
        [FromServices] IRequestClient<GetSmtpConfigurationsByFilter> client,
        CancellationToken cancellationToken,
        [FromQuery] int pageSize = 10,
        [FromQuery] int pageIndex = 1)
    {
        var request = new GetSmtpConfigurationsByFilterRequest
        {
            PageIndex = pageIndex,
            PageSize = pageSize,
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<SmtpConfigurationResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.PageSize,
                    request.PageIndex
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetSmtpConfigurationsByFilter);

        return Result(serviceResult);
    }

    /// <summary>
    /// Get Smtp configuration by Id
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SmtpConfigurationResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> GetSmtpConfigurationByIdAsync(
        [FromServices] IRequestClient<GetSmtpConfigurationById> client,
        CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        var request = new GetSmtpConfigurationByIdRequest
        {
            Id = id
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<SmtpConfigurationResult>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        AddAuditLog(LogEventCode.GetSmtpConfigurationById);

        return Result(serviceResult);
    }

    /// <summary>
    /// Update Smtp configuration
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> UpdateSmtpConfigurationAsync(
        [FromServices] IRequestClient<UpdateSmtpConfiguration> client,
        CancellationToken cancellationToken,
        [FromRoute] string id,
        [FromBody] UpdateSmtpConfigurationRequest request)
    {
        request.Id = id;

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
                    request.Server,
                    request.Port,
                    request.SecurityProtocol,
                    request.UserName,
                    request.Password,
                    UserId = GetUserId(),
                }, cancellationToken));

        AddAuditLog(LogEventCode.UpdateSmtpConfiguration);

        return Result(serviceResult);
    }

    /// <summary>
    /// Delete Smtp configuration
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> DeleteSmtpConfigurationAsync(
        [FromServices] IRequestClient<DeleteSmtpConfiguration> client,
        CancellationToken cancellationToken,
        [FromRoute] string id)
    {
        var request = new DeleteSmtpConfigurationRequest
        {
            Id = id,
        };

        if (!request.IsValid())
        {
            return BadRequest(request);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult>(
                new
                {
                    CorrelationId = RequestId,
                    request.Id
                }, cancellationToken));

        AddAuditLog(LogEventCode.DeleteSmtpConfiguration);

        return Result(serviceResult);
    }
}
