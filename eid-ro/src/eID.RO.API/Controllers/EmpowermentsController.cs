using System.Net.Mime;
using System.Text;
using eID.PJS.AuditLogging;
using eID.RO.API.Authorization;
using eID.RO.API.Requests;
using eID.RO.Contracts;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Results;
using iText.Html2pdf;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Filespec;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace eID.RO.API.Controllers;

//[ApiExplorerSettings(IgnoreApi = true)] // Uncomment when create SDK
[RoleAuthorization(UserRoles.AppRUAndCISAdmins)]
public class EmpowermentsController : BaseV1Controller
{
    /// <summary>
    /// Crate an instance of <see cref="EmpowermentsController"/>
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="auditLogger"></param>
    public EmpowermentsController(IConfiguration configuration, ILogger<EmpowermentsController> logger, AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Query empowerment statements by provided filter
    /// </summary>
    /// <param name="client"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="request"></param>
    /// <returns>Returns a list of all empowerment statements that match the filter criteria</returns>
    [HttpPost(Name = nameof(GetEmpowermentsByFilterAsync))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IPaginatedData<EmpowermentStatementWithSignaturesResult>))]
    public async Task<IActionResult> GetEmpowermentsByFilterAsync(
        [FromServices] IRequestClient<GetEmpowermentsByFilter> client,
        GetEmpowermentsByFilterRequest request,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.GET_EMPOWERMENTS_BY_ADMINISTRATOR;
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, request },
            { AuditLoggingKeys.RequesterUid, GetUid() },
            { AuditLoggingKeys.RequesterUidType, GetUidType().ToString() }
        };
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload);

        if (!request.IsValid())
        {
            return BadRequestWithAuditLog(request, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<IPaginatedData<EmpowermentStatementWithSignaturesResult>>>(
                new
                {
                    CorrelationId = RequestId,
                    request.Number,
                    request.Status,
                    request.Authorizer,
                    request.CreatedOnFrom,
                    request.CreatedOnTo,
                    request.ProviderName,
                    request.ServiceName,
                    request.ValidToDate,
                    request.ShowOnlyNoExpiryDate,
                    request.SortBy,
                    request.SortDirection,
                    request.EmpoweredUids,
                    request.OnBehalfOf,
                    request.EmpowermentUid,
                    request.PageIndex,
                    request.PageSize
                }, cancellationToken));

        return Result(serviceResult, (errorMessage, suffix, statusCode) =>
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                eventPayload.Add("Reason", errorMessage);
            }
            if (statusCode is not null)
            {
                eventPayload.Add("ResponseStatusCode", statusCode);
            }
            if (serviceResult.Result?.Data.Any() == true)
            {
                var currentData = serviceResult.Result.Data.ToDictionary(
                    k => new { EmpowermentId = k.Id, EID = k.CreatedBy, k.ProviderName }, // Common information for each audit log event
                    v => v.AuthorizerUids.Union(v.EmpoweredUids).ToHashSet(new UidResultComparer()) // Distinct list of all authorizers and empowered people data
                );
                foreach (var record in currentData)
                {
                    foreach (var currentUser in record.Value)
                    {
                        var currentPayload = new SortedDictionary<string, object>(eventPayload)
                        {
                            [AuditLoggingKeys.TargetUid] = currentUser.Uid,
                            [AuditLoggingKeys.TargetUidType] = currentUser.UidType.ToString(),
                            [AuditLoggingKeys.TargetName] = currentUser.Name,
                            [nameof(record.Key.ProviderName)] = record.Key.ProviderName ?? "Unable to obtain ProviderName",
                            [nameof(record.Key.EmpowermentId)] = record.Key.EmpowermentId
                        };
                        AddAuditLog(logEventCode, targetUserId: record.Key.EID, suffix: suffix, payload: currentPayload);
                    }
                }
            }
            else
            {
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            }
        });
    }

    /// <summary>
    /// Generate and sign PDF for requested empowerment
    /// </summary>
    /// <param name="client"></param>
    /// <param name="empowermentId"></param>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a pdf file</returns>
    /// 
    [HttpPost("{empowermentId}/export", Name = nameof(HtmlToPdfAsync))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HtmlToPdfAsync(
        [FromServices] IRequestClient<GetEmpowermentById> client,
        [FromRoute] Guid empowermentId,
        [FromBody] ExportEmpowermentPayload data,
        CancellationToken cancellationToken)
    {
        var logEventCode = LogEventCode.EXPORT_EMPOWERMENT;
        var eventPayload = new SortedDictionary<string, object>
        {
            { AuditLoggingKeys.Request, data },
            { AuditLoggingKeys.RequesterUid, GetUid() },
            { AuditLoggingKeys.RequesterUidType, GetUidType().ToString() },
            { "EmpowermentId", empowermentId },
            { AuditLoggingKeys.RequesterName, GetUserFullName() }
        };
        HttpContext.Items["Payload"] = eventPayload;
        HttpContext.Items[nameof(LogEventCode)] = logEventCode;
        AddAuditLog(logEventCode, suffix: LogEventLifecycle.REQUEST, payload: eventPayload);

        if (empowermentId == Guid.Empty)
        {
            var msd = new ModelStateDictionary();
            msd.AddModelError("EmpowermentId", "EmpowermentId is required");
            eventPayload.Add("Reason", "EmpowermentId is required");
            eventPayload.Add("ResponseStatusCode", System.Net.HttpStatusCode.BadRequest);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return ValidationProblem(msd);
        }
        if (!data.IsValid())
        {
            return BadRequestWithAuditLog(data, logEventCode, eventPayload);
        }

        var serviceResult = await GetResponseAsync(() =>
            client.GetResponse<ServiceResult<EmpowermentStatementWithSignaturesResult>>(
                new
                {
                    CorrelationId = RequestId,
                    EmpowermentId = empowermentId
                }, cancellationToken));

        if (serviceResult.StatusCode != System.Net.HttpStatusCode.OK)
        {
            return Result(serviceResult, (errorMessage, suffix, statusCode) =>
            {
                if (!string.IsNullOrWhiteSpace(errorMessage))
                {
                    eventPayload.Add("Reason", errorMessage);
                }
                if (statusCode is not null)
                {
                    eventPayload.Add("ResponseStatusCode", statusCode);
                }
                AddAuditLog(logEventCode, suffix: suffix, payload: eventPayload);
            });
        }
        var empowerment = serviceResult.Result;
        if (empowerment is null)
        {
            Logger.LogError("Empowerment {EmpowermentId} was null.", empowermentId);
            eventPayload.Add("Reason", $"Empowerment {empowermentId} is null.");
            eventPayload.Add("ResponseStatusCode", System.Net.HttpStatusCode.NotFound);
            AddAuditLog(logEventCode, suffix: LogEventLifecycle.FAIL, payload: eventPayload);
            return NotFound();
        }
        byte[] pdfBytes;

        using (MemoryStream ms = new MemoryStream())
        {
            using PdfWriter writer = new PdfWriter(ms);
            using PdfDocument pdfDoc = new PdfDocument(writer);
            PdfFileSpec xmlSpec = PdfFileSpec.CreateEmbeddedFileSpec(
                pdfDoc,
                Encoding.UTF8.GetBytes(empowerment.XMLRepresentation),
                "XmlRepresentation.xml",
                null
            );
            pdfDoc.AddFileAttachment("XmlRepresentation.xml", xmlSpec);
            foreach (var signature in empowerment.EmpowermentSignatures)
            {
                try
                {
                    PdfFileSpec signatureSpec = PdfFileSpec.CreateEmbeddedFileSpec(
                        pdfDoc,
                        Convert.FromBase64String(signature.Signature),
                        $"DetachedSignature-{signature.SignerUid}.sig",
                        null
                    );
                    pdfDoc.AddFileAttachment($"DetachedSignature-{signature.SignerUid}", signatureSpec);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception during signatures iteration.");
                    continue;
                }
            }
            HtmlConverter.ConvertToPdf(data.Html, pdfDoc, new ConverterProperties { });
            pdfBytes = ms.ToArray();
        }

        foreach (var currentUser in empowerment.AuthorizerUids.Union(empowerment.EmpoweredUids).ToHashSet(new UidResultComparer())) // Distinct list of all authorizers and empowered people data
        {
            var currentPayload = new SortedDictionary<string, object>(eventPayload)
            {
                [AuditLoggingKeys.TargetUid] = currentUser.Uid,
                [AuditLoggingKeys.TargetUidType] = currentUser.UidType.ToString(),
                [AuditLoggingKeys.TargetName] = currentUser.Name,
                [nameof(empowerment.ProviderName)] = empowerment?.ProviderName ?? "Unable to obtain ProviderName"
            };
            AddAuditLog(logEventCode, targetUserId: empowerment?.CreatedBy, suffix: LogEventLifecycle.SUCCESS, payload: currentPayload);
        }

        return new FileStreamResult(new MemoryStream(pdfBytes), MediaTypeNames.Application.Pdf)
        {
            FileDownloadName = $"{empowermentId}_{DateTime.UtcNow:dd-MM-yyyy}"
        };

    }
}
