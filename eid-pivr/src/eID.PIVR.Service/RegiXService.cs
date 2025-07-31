using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Validators;
using Microsoft.Extensions.Logging;

namespace eID.PIVR.Service;

public class RegiXService : BaseService, IRegiXService
{
    private readonly ILogger<RegiXService> _logger;
    private readonly IRegiXCaller _regix;

    public RegiXService(ILogger<RegiXService> logger, IRegiXCaller regix)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _regix = regix ?? throw new ArgumentNullException(nameof(regix));
    }

    public async Task<ServiceResult<RegixSearchResultDTO>> SearchAsync(RegiXSearchCommand message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegiXSearchCommandValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<RegixSearchResultDTO>(validationResult.Errors);
        }

        var response = await _regix.SearchAsync(message);
        if (response is null)
        {
            _logger.LogWarning("Regix caller returned null.");
            return new ServiceResult<RegixSearchResultDTO>
            {
                StatusCode = System.Net.HttpStatusCode.InternalServerError,
                Error = "Something went wrong"
            };
        }
        if (response.HasFailed)
        {
            return new ServiceResult<RegixSearchResultDTO>
            {
                StatusCode = System.Net.HttpStatusCode.BadGateway,
                Error = response.Error,
                Result = response
            };
        }
        return Ok(response);
    }
}

public interface IRegiXService
{
    Task<ServiceResult<RegixSearchResultDTO>> SearchAsync(RegiXSearchCommand message);
}
