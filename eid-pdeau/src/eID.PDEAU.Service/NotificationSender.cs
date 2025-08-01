using System.Net.Mime;
using System.Text;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.Extensions;
using eID.PDEAU.Service.Options;
using eID.PDEAU.Service.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace eID.PDEAU.Service;

public class NotificationSender : BaseService, INotificationSender
{
    private readonly ILogger<NotificationSender> _logger;
    private readonly ApplicationUrls _applicationUrls;
    private readonly HttpClient _httpClient;

    public NotificationSender(
        ILogger<NotificationSender> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<ApplicationUrls> applicationUrls)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _httpClient = httpClientFactory.CreateClient("PAN");
        _applicationUrls = (applicationUrls ?? throw new ArgumentNullException(nameof(applicationUrls))).Value;
        _applicationUrls.Validate();
    }

    public async Task<ServiceResult<bool>> SendConfirmationEmailAsync(SendConfirmationEmailRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validation
        var validator = new SendConfirmationEmailRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{CommandName} validation failed. {Errors}", nameof(SendConfirmationEmailRequest), validationResult);
            return BadRequest<bool>(validationResult.Errors);
        }

        var httpSendNotificationBody = BuildSendConfirmationEmailHttpBody(request);

        _logger.LogInformation("Attempting to send confirmation email...");

        var result = await SendEmailAsync(request.CorrelationId, httpSendNotificationBody);
        if (!result)
        {
            return UnhandledException<bool>();
        }

        _logger.LogInformation("Send confirmation email request completed successfully.");
        return Ok(true);
    }

    public async Task<bool> SendEmailAsync(SendEmailRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Validation
        var validator = new SendEmailRequestValidator();
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{RequestName} validation failed. {Errors}", nameof(SendEmailRequest), validationResult);
            return false;
        }

        _logger.LogInformation("Sending email {Email} with subject {Subject}...", request.Email.MaskEmail(), request.Subject);

        var sendUrl = "/api/v1/communications/direct-emails/send";
        HttpResponseMessage? response = null;
        try
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, sendUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    request.Language,
                    request.Subject,
                    request.Body,
                    EmailAddress = request.Email
                }), Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            requestMessage.Headers.TryAddWithoutValidation(Contracts.Constants.HeaderNames.RequestId, request.CorrelationId.ToString());
            response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("An error occurred when trying to send email. Response: {Response}, ResponseBody: {ResponseBody}",
                    response.ToString(), await response.Content.ReadAsStringAsync());
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "An error occurred when trying to send email. Request: POST {Url}, Body {Body}, Response {Response}",
                sendUrl, request.Body, response?.ToString());

            return false;
        }

        _logger.LogInformation("The email {Email} with subject {Subject} has been sent successfully.", request.Email.MaskEmail(), request.Subject);

        return true;
    }

    private async Task<bool> SendEmailAsync(Guid correlationId, object body)
    {
        var sendUrl = "/api/v1/notifications/send";

        HttpResponseMessage? response = null;
        try
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, sendUrl)
            {
                Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, MediaTypeNames.Application.Json)
            };
            requestMessage.Headers.TryAddWithoutValidation(Contracts.Constants.HeaderNames.RequestId, correlationId.ToString());
            response = await _httpClient.SendAsync(requestMessage);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Event registration failed response: {Response}, responseBody: {ResponseBody}", 
                    response.ToString(), await response.Content.ReadAsStringAsync());
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "An error occurred when trying to send confirmation email. Request: POST {Url}, Body {Body}, Response {Response}",
                sendUrl, body, response?.ToString());

            return false;
        }

        return true;
    }

    private object BuildSendConfirmationEmailHttpBody(SendConfirmationEmailRequest request)
    {
        var confirmationLink = $"{_applicationUrls.PdeauHostUrl}/api/v1/providers/administrator-promotions/{request.AdministratorPromotionId}";

        // Mapping the translations to the expected format
        var trasnlations = Events.Events.AdminPromotionConfirmationEmail.Translations
            .Select(t => new SendNotificationTranslation
            {
                Language = t.Language,
                Message = t.Description.Replace("{{ ConfirmationLink }}", confirmationLink),
            });

        var body = new
        {
            EventCode = Events.Events.AdminPromotionConfirmationEmail.Code,
            request.Uid,
            request.UidType,
            Translations = trasnlations
        };

        return body;
    }

    private class SendNotificationTranslation
    {
        public string Language { get; set; }
        public string Message { get; set; }
    }
}

public interface INotificationSender
{
    Task<bool> SendEmailAsync(SendEmailRequest request);
    Task<ServiceResult<bool>> SendConfirmationEmailAsync(SendConfirmationEmailRequest request);
}
