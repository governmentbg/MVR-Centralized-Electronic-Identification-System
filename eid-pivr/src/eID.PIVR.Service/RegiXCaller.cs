using System.Text;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Extensions;
using eID.PIVR.Service.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eID.PIVR.Service;

public class RegiXCaller : IRegiXCaller
{
    private readonly ILogger<RegiXCaller> _logger;
    private readonly IDistributedCache _cache;
    private readonly HttpClient _httpClient;
    private readonly ExternalRegistersCacheOptions _cacheOptions;

    public RegiXCaller(ILogger<RegiXCaller> logger,
                       IDistributedCache cache,
                       HttpClient httpClient,
                       IOptions<ExternalRegistersCacheOptions> cacheOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _cacheOptions = (cacheOptions ?? throw new ArgumentNullException(nameof(cacheOptions))).Value;
    }

    public async Task<RegixSearchResultDTO> SearchAsync(RegiXSearchCommand message)
    {
        if (message is null)
        {
            _logger.LogWarning("RegiXSearch called with null message.");
            return new FailedRegiXResult
            {
                Error = "Malformed request."
            };
        }


        string cacheKey = string.Empty;
        try
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(message.Command));

                cacheKey = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", string.Empty);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed during cacheKey generation. Terminating RegiX query operation.");
            return new FailedRegiXResult
            {
                Error = "Something went wrong."
            };
        }
        if (string.IsNullOrWhiteSpace(cacheKey))
        {
            _logger.LogError("CacheKey remained empty. Terminating RegiX query operation.");
            return new FailedRegiXResult
            {
                Error = "Something went wrong."
            };
        }
        RegixSearchResultDTO? response = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var response = await QueryRegiXAsync(message, cacheKey);
            if (response is null)
            {
                return new FailedRegiXResult
                {
                    Error = "Malformed response."
                };
            }
            if (response.GetType() != typeof(FailedRegiXResult))
            {
                await _cache.SetAsync(cacheKey, response, _cacheOptions.ExpireAfterInHours);
            }

            return response;
        });

        if (response is null)
        {
            return new FailedRegiXResult
            {
                Error = "Malformed response."
            };
        }
        return response;
    }

    private async Task<RegixSearchResultDTO> QueryRegiXAsync(RegiXSearchCommand message, string cacheKey)
    {
        var stringContent = new StringContent(message.Command, Encoding.UTF8, "application/json");
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsync("search", stringContent);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            await _cache.RemoveAsync(cacheKey);
            // Cancellation could be caused by Socket timeout
            _logger.LogInformation("Failed making RegiX request. Exception Message: {ExceptionMessage}", ex.Message);
            return new FailedRegiXResult { Error = "Failed getting RegiX response." };
        }

        try
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RegixSearchResultDTO>(body) ?? new FailedRegiXResult { Error = "Null RegiX response." };
        }
        catch (Exception ex)
        {
            await _cache.RemoveAsync(cacheKey);
            _logger.LogInformation("Failed parsing RegiX response. Exception Message: {ExceptionMessage}", ex.Message);
            return new FailedRegiXResult { Error = "Failed parsing response." };
        }
    }
}

internal class FailedRegiXResult : RegixSearchResultDTO
{
    public new bool HasFailed { get; set; } = true;
    public new string? Error { get; set; } = "Unexpected error";
}
public interface IRegiXCaller
{
    Task<RegixSearchResultDTO> SearchAsync(RegiXSearchCommand message);
}
