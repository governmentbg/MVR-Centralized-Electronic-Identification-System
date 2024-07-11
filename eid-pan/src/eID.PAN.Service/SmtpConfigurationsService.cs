using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Results;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Extensions;
using eID.PAN.Service.Options;
using eID.PAN.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eID.PAN.Service;

public class SmtpConfigurationsService : BaseService
{
    private readonly ILogger<SmtpConfigurationsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly AesOptions _aesOptions;

    public SmtpConfigurationsService(
        ILogger<SmtpConfigurationsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        IOptions<AesOptions> aesOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _aesOptions = (aesOptions ?? throw new ArgumentNullException(nameof(aesOptions))).Value;
        _aesOptions.Validate();
    }

    public async Task<ServiceResult<Guid>> CreateAsync(CreateSmtpConfiguration message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new CreateSmtpConfigurationValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        var existingSmtpConfiguration = await _context.SmtpConfigurations.AnyAsync(c => c.Server == message.Server);
        if (existingSmtpConfiguration)
        {
            _logger.LogInformation("{Server} already exists.", message.Server);
            return Conflict<Guid>(nameof(message.Server), message.Server);
        }

        // Action
        var smtpConfiguration = new SmtpConfiguration
        {
            Id = Guid.NewGuid(),
            Server = message.Server.Trim(),
            Port = message.Port,
            SecurityProtocol = message.SecurityProtocol,
            UserName = message.UserName.Trim(),
            Password = AesEncryptDecryptHelper.EncryptPassword(message.Password.Trim(), _aesOptions.Key, _aesOptions.Vector),
            CreatedOn = DateTime.UtcNow,
            CreatedBy = message.UserId
        };

        await _context.SmtpConfigurations.AddAsync(smtpConfiguration);
        await _context.SaveChangesAsync();
        _cache.Remove(SmtpConfiguration.AllSmtpConfigurationCacheKey);
        _logger.LogInformation("Smtp configuration created. Id: {Id}", smtpConfiguration.Id);
        // Result
        return Ok(smtpConfiguration.Id);
    }

    public async Task<ServiceResult<IPaginatedData<SmtpConfigurationResult>>> GetByFilterAsync(GetSmtpConfigurationsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetSmtpConfigurationsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<SmtpConfigurationResult>>(validationResult.Errors);
        }

        // Action
        var smtpConfigurations = _context.SmtpConfigurations.AsQueryable();

        // Pagination
        // Execute query
        var result = await PaginatedData<SmtpConfigurationResult>.CreateAsync(smtpConfigurations.OrderBy(s => s.Server), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<SmtpConfigurationResult>> GetByIdAsync(GetSmtpConfigurationById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetSmtpConfigurationByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<SmtpConfigurationResult>(validationResult.Errors);
        }

        // Action
        var cacheKey = SmtpConfiguration.BuildSmtpConfigurationCacheKey(message.Id);
        SmtpConfigurationResult? configuration = await _cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var configuration = await _context.SmtpConfigurations.FirstOrDefaultAsync(c => c.Id == message.Id);
            if (configuration != null)
            {
                await _cache.SetAsync(cacheKey, configuration, slidingExpireTime: TimeSpan.FromMinutes(10));
            }

            return configuration;
        });

        if (configuration is null)
        {
            return NotFound<SmtpConfigurationResult>(nameof(message.Id), message.Id);
        }

        // Result
        return Ok(configuration);
    }

    public async Task<ServiceResult> UpdateAsync(UpdateSmtpConfiguration message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new UpdateSmtpConfigurationValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var smtpConfiguration = await _context.SmtpConfigurations.FirstOrDefaultAsync(c => c.Id == message.Id);
        if (smtpConfiguration is null)
        {
            _logger.LogInformation("Unable to find Smtp configuration with Id: {Id}", message.Id);
            return NotFound(nameof(message.Id), message.Id);
        }

        // Action
        if (!string.IsNullOrWhiteSpace(message.Server))
        {
            smtpConfiguration.Server = message.Server.Trim();
        }

        if (message.Port.HasValue && message.Port.Value != 0)
        {
            smtpConfiguration.Port = message.Port.Value;
        }

        if (!string.IsNullOrWhiteSpace(message.UserName))
        {
            smtpConfiguration.UserName = message.UserName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(message.Password))
        {
            smtpConfiguration.Password = AesEncryptDecryptHelper.EncryptPassword(message.Password.Trim(), _aesOptions.Key, _aesOptions.Vector);
        }

        smtpConfiguration.SecurityProtocol = message.SecurityProtocol;
        smtpConfiguration.ModifiedBy = message.UserId;
        smtpConfiguration.ModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Invalidate the cache
        var key = SmtpConfiguration.BuildSmtpConfigurationCacheKey(message.Id);
        _cache.Remove(key);
        _cache.Remove(SmtpConfiguration.AllSmtpConfigurationCacheKey);

        _logger.LogInformation("Smtp configuration updated. Id: {Id}", smtpConfiguration.Id);

        // Result
        return NoContent();
    }

    public async Task<ServiceResult> DeleteAsync(DeleteSmtpConfiguration message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DeleteSmtpConfigurationValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var smtpConfiguration = await _context.SmtpConfigurations.FirstOrDefaultAsync(c => c.Id == message.Id);
        if (smtpConfiguration is null)
        {
            _logger.LogInformation("Unable to find Smtp configuration with Id: {Id}", message.Id);
            return NotFound(nameof(message.Id), message.Id);
        }

        _context.SmtpConfigurations.Remove(smtpConfiguration);
        await _context.SaveChangesAsync();

        // Invalidate the cache
        var key = SmtpConfiguration.BuildSmtpConfigurationCacheKey(message.Id);
        _cache.Remove(key);
        _cache.Remove(SmtpConfiguration.AllSmtpConfigurationCacheKey);

        _logger.LogInformation("Smtp configuration deleted. Id: {Id}", smtpConfiguration.Id);

        // Result
        return NoContent();
    }
}
