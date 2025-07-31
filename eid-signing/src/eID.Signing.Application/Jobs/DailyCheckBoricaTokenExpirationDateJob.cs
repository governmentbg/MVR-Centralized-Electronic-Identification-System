using System.Net.Mime;
using System.Text;
using eID.Signing.Contracts.Enums;
using eID.Signing.Service.Database;
using eID.Signing.Service.Extensions;
using eID.Signing.Service.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace eID.Signing.Application.Jobs;

[DisallowConcurrentExecution]
public class DailyCheckBoricaTokenExpirationDateJob : IJob
{
    private readonly ILogger<DailyCheckBoricaTokenExpirationDateJob> _logger;
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly TokenExpirationNotificationOptions _options;

    public DailyCheckBoricaTokenExpirationDateJob(
        ILogger<DailyCheckBoricaTokenExpirationDateJob> logger,
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IOptions<TokenExpirationNotificationOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
        _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
        _options.Validate();
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var currentToken = await _context.AccessTokens.FirstOrDefaultAsync(t => t.Status == AccessTokenStatus.Active);
            if (currentToken is null)
            {
                _logger.LogWarning("No active access token found.");

                await SendEmailAsync("Няма наличен активен access token");
                return;
            }

            var daysLeft = currentToken.ExpirationDate.DayNumber - today.DayNumber;
            _logger.LogInformation("Access token found. Days until expiration: {DaysLeft}", daysLeft);

            // First alert will be 30 days before expiration.
            // In the last 7 days there will be daily notification that the token will expire soon and the service will stop.
            if (daysLeft == 30 || daysLeft == 21 || daysLeft == 14 || daysLeft <= 7)
            {
                if (daysLeft >= 0)
                {
                    var dayText = daysLeft > 1 ? "дни" : "ден";
                    await SendEmailAsync($"Access token-а ще изтече след {daysLeft} { dayText }");
                    return;
                }
                else
                {
                    await SendEmailAsync("Access token-а вече е изтекъл");
                    _logger.LogWarning("Access token has already expired.");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when executing {nameof(DailyCheckBoricaTokenExpirationDateJob)}");
        }
    }

    private async Task SendEmailAsync(string body)
    {
        _logger.LogInformation("Sending email {Email} with subject {Subject}...", _options.Email.MaskEmail(), _options.Subject);

        var sendUrl = "/api/v1/communications/direct-emails/send";
        string requestBody = string.Empty;
        HttpResponseMessage? response = null;
        try
        {
            requestBody = JsonConvert.SerializeObject(new
            {
                Language = "bg",
                _options.Subject,
                Body = body,
                EmailAddress = _options.Email
            });
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, sendUrl)
            {
                Content = new StringContent(requestBody, Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("An error occurred when trying to send email. Response: {Response}, ResponseBody: {ResponseBody}",
                    response.ToString(), await response.Content.ReadAsStringAsync());
                return;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "An error occurred when trying to send email. Request: POST {Url}, Body {Body}, Response {Response}",
                sendUrl, body, response?.ToString());

            return;
        }

        _logger.LogInformation("The email {Email} with subject {Subject} has been sent successfully.", _options.Email.MaskEmail(), _options.Subject);
    }
}
