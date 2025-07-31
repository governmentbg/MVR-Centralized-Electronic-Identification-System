using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Enums;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Extensions;
using eID.PAN.Service.Options;
using eID.PAN.Service.Validators;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Polly;

namespace eID.PAN.Service;
public class CommunicationsService : BaseService
{
    private readonly ILogger<CommunicationsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ISmtpClient _smtpClient;
    private readonly SmtpOptions _smtpOptions;
    private readonly ISmsSender _smsSender;
    private readonly IHttpCallbackSender _httpCallbackSender;
    private readonly IMpozeiCaller _mpozeiCaller;
    private readonly AesOptions _aesOptions;

    public CommunicationsService(
        ILogger<CommunicationsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IOptions<SmtpOptions> smtpOptions,
        ISmtpClient smtpClient,
        IPushNotificationSender pushNotificationSender,
        ISmsSender smsSender,
        IHttpCallbackSender httpCallbackSender,
        IOptions<AesOptions> aesOptions,
        IMpozeiCaller mpozeiCaller)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _smtpClient = smtpClient ?? throw new ArgumentNullException(nameof(smtpClient));
        _smtpOptions = (smtpOptions ?? throw new ArgumentNullException(nameof(smtpOptions))).Value;
        _smtpOptions.Validate();
        _pushNotificationSender = pushNotificationSender ?? throw new ArgumentNullException(nameof(pushNotificationSender));
        _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
        _httpCallbackSender = httpCallbackSender ?? throw new ArgumentNullException(nameof(httpCallbackSender));
        _mpozeiCaller = mpozeiCaller ?? throw new ArgumentNullException(nameof(mpozeiCaller));
        _aesOptions = (aesOptions ?? throw new ArgumentNullException(nameof(aesOptions))).Value;
        _aesOptions.Validate();
    }

    public async Task SendEmailAsync(SendEmail message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendEmailValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Not valid SendEmail message. Validation failed with errors: {Errors}.", validationResult.Errors);
            return;
        }

        var smtpConfigurations = await _cache.GetOrCreateAsync(SmtpConfiguration.AllSmtpConfigurationCacheKey, async () =>
        {
            var list = await _context.SmtpConfigurations.ToListAsync();
            if (list != null)
            {
                await _cache.SetAsync(SmtpConfiguration.AllSmtpConfigurationCacheKey, list, slidingExpireTime: TimeSpan.FromMinutes(10));
            }

            return list;
        });
        if (smtpConfigurations is null)
        {
            _logger.LogWarning("Smtp configurations resulted null");
            return;
        }
        if (!smtpConfigurations.Any())
        {
            _logger.LogWarning("Cannot find any Smtp configurations");
            return;
        }

        MimeMessage email;
        try
        {
            email = await PrepareEmailAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed preparing email");
            return;
        }

        var maxRetryAttempts = 3;
        var pauseBetweenFailures = TimeSpan.FromSeconds(5);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                onRetry: (ex, ts) =>
                {
                    if (_smtpClient.IsConnected)
                    {
                        try
                        {
                            _smtpClient.Disconnect(quit: true);
                        }
                        catch (Exception _ex)
                        {
                            _logger.LogError(_ex, "Failed when trying to disconnect smtp client.");
                        }
                    }
                    _logger.LogError(ex, "Sending email to {UserId} failed. Retrying to send email.", message.UserId);
                });

        await retryPolicy.ExecuteAsync(async () =>
        {
            var randomSmtpServer = ChooseRandomServer(smtpConfigurations);
            await SendEmailAsync(randomSmtpServer, _smtpClient, email);
        });
    }

    public async Task SendDirectEmailAsync(SendDirectEmail message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendDirectEmailValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Not valid SendDirectEmail message. Validation failed with errors: {Errors}.", validationResult.Errors);
            return;
        }

        var smtpConfigurations = await _cache.GetOrCreateAsync(SmtpConfiguration.AllSmtpConfigurationCacheKey, async () =>
        {
            var list = await _context.SmtpConfigurations.ToListAsync();
            if (list != null)
            {
                await _cache.SetAsync(SmtpConfiguration.AllSmtpConfigurationCacheKey, list, slidingExpireTime: TimeSpan.FromMinutes(10));
            }

            return list;
        });
        if (smtpConfigurations is null)
        {
            _logger.LogWarning("Smtp configurations resulted null");
            return;
        }
        if (!smtpConfigurations.Any())
        {
            _logger.LogWarning("Cannot find any Smtp configurations");
            return;
        }

        var email = await PrepareDirectEmailAsync(message);

        var maxRetryAttempts = 3;
        var pauseBetweenFailures = TimeSpan.FromSeconds(5);

        var maskedEmail = string.Format("{0}******{1}", message.EmailAddress[0], message.EmailAddress[(message.EmailAddress.IndexOf('@') - 1)..]);

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures,
                onRetry: (ex, ts) =>
                {
                    if (_smtpClient.IsConnected)
                    {
                        try
                        {
                            _smtpClient.Disconnect(quit: true);
                        }
                        catch (Exception _ex)
                        {
                            _logger.LogError(_ex, "Failed when trying to disconnect smtp client.");
                        }
                    }
                    _logger.LogError(ex, "Sending direct email to {MaskedEmail} failed. Retrying to send email.", maskedEmail);
                });

        _logger.LogInformation("Trying to send email to {MaskedEmail}", maskedEmail);

        await retryPolicy.ExecuteAsync(async () =>
        {
            var randomSmtpServer = ChooseRandomServer(smtpConfigurations);
            await SendEmailAsync(randomSmtpServer, _smtpClient, email);
        });
    }

    private async Task<MimeMessage> PrepareDirectEmailAsync(SendDirectEmail message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
        email.To.Add(MailboxAddress.Parse(message.EmailAddress));
        email.Subject = message.Subject;

        var body = await PrepareDirectEmailBodyAsync(message.Body, message.Language);

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = body,
        };
        return email;
    }

    /// <summary>
    /// Sending localized push notification to all registered user devices.
    /// </summary>
    /// <param name="message">Required. Validated by <see cref="SendPushNotificationValidator"/>SendPushNotificationValidator</see></param>
    /// <returns>At least one notification was successfully sent.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<bool> SendPushNotificationAsync(SendPushNotification message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendPushNotificationValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("({Command}) Validation failed with errors: {Errors}.", nameof(SendPushNotification), validationResult);
            return false;
        }
       
        var userProfile = await _mpozeiCaller.FetchUserProfileByCitizenProfileIdAsync(message.UserId);
        if (userProfile is null)
        {
            _logger.LogWarning("There was a problem while trying to obtain User profile in Mpozei. UserId: {UserId}", message.UserId);
            return false;
        }

        // 1. Fetch user language selection from user profile information.
        if (string.IsNullOrWhiteSpace(userProfile.FirebaseId))
        {
            _logger.LogInformation("{UserId} doesn't have application token that can receive push notification.", message.UserId);
            return false;
        }
        return await _pushNotificationSender.SendPushNotificationAsync(message.UserId, userProfile.FirebaseId, GetLocalizedOrDefaultMessage(message, userProfile.Language));
    }

    public async Task<bool> SendSmsAsync(SendSms message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendSmsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("({Command}) Validation failed with errors: {Errors}.", nameof(SendSms), validationResult);
            return false;
        }

        var userProfile = await _mpozeiCaller.FetchUserProfileByCitizenProfileIdAsync(message.UserId);
        if (userProfile is null)
        {
            _logger.LogWarning("There was a problem while trying to obtain User profile in Mpozei. UserId: {UserId}", message.UserId);
            return false;
        }

        if (string.IsNullOrWhiteSpace(userProfile.PhoneNumber))
        {
            _logger.LogWarning("Phone number for User: {UserId} is not set", message.UserId);
            return false;
        }

        //TODO Remove this hardcoded values when MPOZEI returns this information
        var messageBodyInSelectedLanguage = GetLocalizedOrDefaultMessage(message, userProfile.Language);

        return await _smsSender.SendSmsAsync(message.UserId, userProfile.PhoneNumber, messageBodyInSelectedLanguage);
    }

    public async Task<bool> SendHttpCallbackAsync(SendHttpCallbackAsync message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SendHttpCallbackValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("({Command}) Validation failed with errors: {Errors}.", nameof(SendHttpCallbackAsync), validationResult);
            return false;
        }

        return await _httpCallbackSender.SendHttpCallbackAsync(message.CallbackUrl, message.Body);
    }

    /// <summary>
    /// Looks for the requested language in Translations. Fallsback to "bg" if requested language isn't found.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="userLanguage"></param>
    /// <returns>Localized message</returns>
    private string GetLocalizedOrDefaultMessage(SendPushNotification message, string userLanguage)
    {
        var defaultTranslation = message.Translations.First(t => t.Language == "bg");
        var currentTranslation = message.Translations.FirstOrDefault(t => t.Language == userLanguage) ?? defaultTranslation;
        return currentTranslation.Message;
    }

    /// <summary>
    /// Looks for the requested language in Translations. Fallsback to "bg" if requested language isn't found.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="userLanguage"></param>
    /// <returns>Localized message</returns>
    private string GetLocalizedOrDefaultMessage(SendSms message, string userLanguage)
    {
        var defaultTranslation = message.Translations.First(t => t.Language == "bg");
        var currentTranslation = message.Translations.FirstOrDefault(t => t.Language == userLanguage) ?? defaultTranslation;
        return currentTranslation.Message;
    }
    /// <summary>
    /// Looks for the requested language in Translations. Fallsback to "bg" if requested language isn't found.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="userLanguage"></param>
    /// <returns>Localized message</returns>
    private string GetLocalizedOrDefaultMessage(SendEmail message, string userLanguage)
    {
        var defaultTranslation = message.Translations.First(t => t.Language == "bg");
        var currentTranslation = message.Translations.FirstOrDefault(t => t.Language == userLanguage) ?? defaultTranslation;
        return currentTranslation.Message;
    }

    private async Task SendEmailAsync(SmtpConfiguration smtpConfiguration, ISmtpClient client, MimeMessage email)
    {
        var socketOpt = SecureSocketOptions.None;
        if (smtpConfiguration.SecurityProtocol == SmtpSecurityProtocol.SSL)
        {
            socketOpt = SecureSocketOptions.SslOnConnect;
        }
        else if (smtpConfiguration.SecurityProtocol == SmtpSecurityProtocol.TLS)
        {
            socketOpt = SecureSocketOptions.StartTls;
        }

        var plainPassword = string.Empty;

        using (Aes myAes = Aes.Create())
        {
            plainPassword = AesEncryptDecryptHelper.DecryptPassword(smtpConfiguration.Password, _aesOptions.Key, _aesOptions.Vector);
        }
        if (!client.IsConnected)
        {
            client.Connect(smtpConfiguration.Server, smtpConfiguration.Port, socketOpt);
        }
        
        if (client.Capabilities.HasFlag(SmtpCapabilities.Authentication) && !client.IsAuthenticated)
        {
            client.Authenticate(smtpConfiguration.UserName, plainPassword);
        }
        await client.SendAsync(email);

        var plainEmail = email.To.First().ToString();
        var maskedEmail = string.Format("{0}******{1}", plainEmail[0], plainEmail[(plainEmail.IndexOf('@') - 1)..]);

        _logger.LogInformation("Email successfully sent. Recipient: {MaskedEmail}", maskedEmail);
    }

    private async Task<MimeMessage> PrepareEmailAsync(SendEmail message)
    {
        var userProfile = await _mpozeiCaller.FetchUserProfileByCitizenProfileIdAsync(message.UserId);
        if (userProfile is null)
        {
            _logger.LogWarning("There was a problem while trying to obtain User profile in Mpozei. UserId: {UserId}", message.UserId);
            throw new ArgumentNullException("There was a problem while trying to obtain User profile in Mpozei. Cannot continue with sending email");
        }

        if (string.IsNullOrWhiteSpace(userProfile.Email))
        {
            _logger.LogWarning("Email for User: {UserId} is not set", message.UserId);
            throw new ArgumentNullException(nameof(userProfile.Email));
        }

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_smtpOptions.SenderName, _smtpOptions.SenderEmail));
        email.To.Add(MailboxAddress.Parse(userProfile.Email));

        var emailSubject = userProfile.Language switch
        {
            "en" => "Automatic notification from the Centralized Electronic Identification System (CEIS)",
            _ => "Автоматична нотификация от Централизираната система за електронна идентификация (ЦСЕИ)"
        };
        email.Subject = emailSubject;

        var body = await PrepareEmailBodyAsync(message, userProfile.Language);

        email.Body = new TextPart(TextFormat.Html)
        {
            Text = body,
        };

        return email;
    }

    /// <summary>
    /// Returns email body based on different languages. If there is no template for wanted language, it will fall back to default bulgarian template
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <param name="selectedLanguage"></param>
    private async Task<string> PrepareEmailBodyAsync(SendEmail message, string selectedLanguage)
    {
        var templateText = await ReadFileDataAsync($"email-template-{selectedLanguage}.html", Assembly.GetExecutingAssembly());
        return templateText.Replace("{{ message }}", HttpUtility.HtmlEncode(GetLocalizedOrDefaultMessage(message, selectedLanguage)));
    }


    /// <summary>
    /// Returns email body based on different languages. If there is no template for wanted language, it will fall back to default bulgarian template
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    /// <param name="selectedLanguage"></param>
    private async Task<string> PrepareDirectEmailBodyAsync(string message, string selectedLanguage)
    {
        var templateText = await ReadFileDataAsync($"email-template-{selectedLanguage}.html", Assembly.GetExecutingAssembly());
        return templateText.Replace("{{ message }}", message);
    }

    /// <summary>
    /// Locates the file in the assembly and return its content. 
    /// If the file isn't found it falls back to default "email-template-bg.html" template.
    /// </summary>
    /// <param name="embeddedFileName"></param>
    /// <param name="assembly"></param>
    /// <returns>Empty string is either empty file or file not found.</returns>
    private Task<string> ReadFileDataAsync(string embeddedFileName, Assembly assembly)
    {
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));
        if (!string.IsNullOrWhiteSpace(resourceName))
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEndAsync();
                    }
                }
                else
                {
                    _logger.LogWarning("Template file {RequestedFileName} was not found .", resourceName);
                }
            }
        }
        var defaultTemplateFilename = "email-template-bg.html";
        var defaultResourceName = assembly.GetManifestResourceNames().FirstOrDefault(s => s.EndsWith(defaultTemplateFilename, StringComparison.CurrentCultureIgnoreCase));
        if (string.IsNullOrWhiteSpace(defaultResourceName))
        {
            _logger.LogError("No email template files were found.");
            return Task.FromResult(string.Empty);
        }
        using (var stream = assembly.GetManifestResourceStream(defaultResourceName))
        {
            if (stream != null)
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEndAsync();
                }
            }
        }
        _logger.LogError("Failed reading default email template content.");
        return Task.FromResult(string.Empty);
    }

    /// <summary>
    /// Returns a random Smtp server configuration from a provided list.
    /// </summary>
    /// <param name="servers"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private static SmtpConfiguration ChooseRandomServer(List<SmtpConfiguration> servers)
    {
        var randomIndexInRange = new Random().Next(0, servers.Count);
        return servers[randomIndexInRange];
    }
}
