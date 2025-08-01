using eID.PDEAU.Contracts.Enums;
using eID.PJS.AuditLogging;
using Microsoft.AspNetCore.Mvc;

namespace eID.PDEAU.API.Public.Admin.Controllers;

public class NomenclaturesController : BaseV1Controller
{
    public NomenclaturesController(
            IConfiguration configuration,
            ILogger<ProvidersController> logger,
            AuditLogger auditLogger) : base(configuration, logger, auditLogger)
    {
    }

    /// <summary>
    /// Get list of supported personal information type
    /// </summary>
    [HttpGet("personalinformation")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CollectablePersonalInformation>))]
    public IActionResult GetSupportedPersonalInformation()
    {
        var result = Enum.GetValues<CollectablePersonalInformation>().Where(d => d != CollectablePersonalInformation.None);

        return Ok(result);
    }
}
