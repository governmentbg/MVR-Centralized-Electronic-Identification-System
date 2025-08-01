using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Contracts.Results;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Entities;
using eID.PDEAU.Service.Extensions;
using eID.PDEAU.Service.Options;
using eID.PDEAU.Service.Requests;
using eID.PDEAU.Service.Validators;
using FluentValidation;
using MassTransit.Util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace eID.PDEAU.Service;

public class ProvidersDetailsService : BaseService
{
    private readonly ILogger<ProvidersDetailsService> _logger;
    private readonly IDistributedCache _cache;
    private readonly ApplicationDbContext _context;
    private readonly INotificationSender _notificationSender;
    private readonly NotificationEmailsOptions _notificationEmailsOptions;

    public ProvidersDetailsService(
        ILogger<ProvidersDetailsService> logger,
        IDistributedCache cache,
        ApplicationDbContext context,
        INotificationSender notificationSender,
        IOptions<NotificationEmailsOptions> notificationEmailsOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
        _notificationEmailsOptions = (notificationEmailsOptions ?? throw new ArgumentNullException(nameof(notificationEmailsOptions))).Value;
        _notificationEmailsOptions.IsValid();
    }

    public async Task<ServiceResult<IPaginatedData<ProviderDetailsResult>>> GetProviderDetailsByFilterAsync(GetProviderDetailsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderDetailsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<ProviderDetailsResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        // Default query should return details with at last one active service eligible for empowerment
        var providerDetails = _context.Providers.Where(p =>
                p.Status == ProviderStatus.Active
                && p.Details != null
                && p.Details.Status == message.Status
        );

        // if message.IncludeDeleted returns all
        if (!message.IncludeDeleted)
        {
            providerDetails = providerDetails.Where(s => s.Details.IsDeleted == false);
        }

        if (message.IncludeEmpowermentOnly)
        {
            providerDetails = providerDetails
                                    .Where(b => b.Details.ProviderServices
                                                                    .Any(s =>
                                                                            s.IsEmpowerment
                                                                            && s.MinimumLevelOfAssurance != LevelOfAssurance.None
                                                                            && s.RequiredPersonalInformation != null
                                                                            && s.IsDeleted == false));
        }

        if (message.IncludeWithServicesOnly)
        {
            providerDetails = providerDetails.Where(b => b.Details.ProviderServices.Any(s => s.IsDeleted == false));
        }

        if (!string.IsNullOrWhiteSpace(message.Name))
        {
            providerDetails = providerDetails.Where(s => s.Details.Name.ToUpper().Contains(message.Name.ToUpper()));
        }

        // Pagination
        // Execute queryw
        var result = await GetPaginatedAsync<ProviderDetailsResult>(providerDetails.Select(s => s.Details).OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<IEnumerable<ProviderDetailsResult>>> GetAvailableProviderDetailsByFilterAsync(GetAvailableProviderDetailsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetAvailableProviderDetailsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<ProviderDetailsResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var providerDetails = _context.ProvidersDetails
            .Where(b => b.Status == ProviderDetailsStatus.Deactivated && b.IsDeleted == false && b.SyncedFromOnlineRegistry)
            .OrderBy(s => s.Name)
            .Cast<ProviderDetailsResult>();

        // Pagination
        // Execute queryw
        var result = await providerDetails.ToListAsync();

        // Result
        return Ok(result.AsEnumerable());
    }

    public async Task<ServiceResult<ProviderDetailsResult>> GetProviderDetailsByIdAsync(GetProviderDetailsById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetProviderDetailsByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<ProviderDetailsResult>(validationResult.Errors);
        }

        // Action
        // Execute query
        var result = await _context.ProvidersDetails
            .FirstOrDefaultAsync(b => b.Id == message.Id);

        if (result == null)
        {
            return NotFound<ProviderDetailsResult>(nameof(message.Id), message.Id);
        }

        // Result
        return Ok((ProviderDetailsResult)result);
    }

    public async Task<ServiceResult> SetProviderDetailsStatusAsync(SetProviderDetailsStatus message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new SetProviderDetailsStatusValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        // Action
        // Execute query
        var details = await _context.ProvidersDetails
            .FirstOrDefaultAsync(b => b.Id == message.Id);

        if (details == null)
        {
            return NotFound(nameof(message.Id), message.Id);
        }

        details.Status = message.Status switch
        {
            ProviderDetailsStatusType.Deactivate => ProviderDetailsStatus.Deactivated,
            ProviderDetailsStatusType.Activate => ProviderDetailsStatus.Active,
            _ => throw new ArgumentException($"{nameof(ProviderDetailsStatusType)}: {message.Status}")
        };
        await _context.SaveChangesAsync();

        // Result
        return NoContent();
    }

    public async Task<ServiceResult<IPaginatedData<SectionResult>>> GetSectionsByFilterAsync(GetSectionsByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetSectionsByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<SectionResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        var sections = _context.ProvidersDetailsSections
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(message.Name))
        {
            sections = sections
                .Where(s => s.Name.ToUpper().Contains(message.Name.ToUpper()));
        }

        // if message.IncludeDeleted returns all
        if (!message.IncludeDeleted)
        {
            sections = sections
                .Where(s => s.IsDeleted == false);
        }

        // Pagination
        // Execute query
        var result = await GetPaginatedAsync<SectionResult>(sections.OrderBy(s => s.Name), message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<SectionResult>> GetSectionByIdAsync(GetSectionById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetSectionByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<SectionResult>(validationResult.Errors);
        }

        // Action
        // Execute query
        var result = await _context.ProvidersDetailsSections
            .FirstOrDefaultAsync(b => b.Id == message.Id);

        if (result == null)
        {
            return NotFound<SectionResult>(nameof(message.Id), message.Id);
        }

        // Result
        return Ok((SectionResult)result);
    }

    public async Task<ServiceResult<IPaginatedData<ProviderServiceResult>>> GetServicesByFilterAsync(GetServicesByFilter message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetServicesByFilterValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IPaginatedData<ProviderServiceResult>>(validationResult.Errors);
        }

        // Action
        // Prepare query
        // Default query returns services that has at least one service scope (обем на представителна власт)
        var services = _context.ProvidersDetailsServices
            .Include(pds => pds.ProviderDetails.Providers)
            .AsQueryable();
        if (message.IncludeApprovedOnly)
        {
            services = services.Where(s => s.Status == ProviderServiceStatus.Approved);
        }

        if (!message.IncludeInactive)
        {
            services = services.Where(s => s.IsActive);
        }

        if (!message.IncludeWithoutScope)
        {
            services = services.Where(s => s.ServiceScopes.Any());
        }

        if (message.ServiceNumber.HasValue)
        {
            services = services
                .Where(s => s.ServiceNumber == message.ServiceNumber.Value);
        }

        if (!string.IsNullOrWhiteSpace(message.Name))
        {
            services = services
                .Where(s => s.Name.ToUpper().Contains(message.Name.ToUpper()));
        }

        if (!string.IsNullOrWhiteSpace(message.Description))
        {
            services = services
                .Where(s => s.Description != null && s.Description.ToUpper().Contains(message.Description.ToUpper()));
        }

        if (!string.IsNullOrWhiteSpace(message.FindServiceNumberAndName))
        {
            services = services
                .Where(s => s.Name.ToUpper().Contains(message.FindServiceNumberAndName.ToUpper()) ||
                (s.ServiceNumber.ToString().Contains(message.FindServiceNumberAndName)));
        }

        // if !message.IncludeEmpowerment returns all
        if (message.IncludeEmpowermentOnly)
        {
            services = services
                .Where(s => s.IsEmpowerment == true
                            && s.MinimumLevelOfAssurance != LevelOfAssurance.None
                            && s.RequiredPersonalInformation != null);
        }

        if (message.ProviderId.HasValue)
        {
            var provider = await _context.Providers.FirstOrDefaultAsync(pr => pr.Id == message.ProviderId);
            if (provider is null)
            {
                return NotFound<IPaginatedData<ProviderServiceResult>>(nameof(message.ProviderId), message.ProviderId);
            }

            services = services
                .Where(s => s.ProviderDetailsId == provider.DetailsId);
        }

        if (message.ProviderSectionId.HasValue)
        {
            services = services
                .Where(s => s.ProviderSectionId == message.ProviderSectionId.Value);
        }

        // if message.IncludeDeleted returns all
        if (!message.IncludeDeleted)
        {
            services = services
                .Where(s => s.IsDeleted == false);
        }

        if (message.IsPLSRole)
        {
            services = services.Where(s => s.ProviderDetails.Providers.Any(p => p.Type == ProviderType.PrivateLawSubject));
        }

        if (message.SortBy.HasValue)
        {
            if (message.SortDirection == SortDirection.Desc)
            {
                services = message.SortBy switch
                {
                    ProviderServicesSortBy.ServiceNumber => services.OrderByDescending(s => s.ServiceNumber),
                    ProviderServicesSortBy.Name => services.OrderByDescending(s => s.Name),
                    ProviderServicesSortBy.IsEmpowerment => services.OrderByDescending(s => s.IsEmpowerment),
                    ProviderServicesSortBy.MinimumLevelOfAssurance => services.OrderByDescending(s => s.MinimumLevelOfAssurance),
                    _ => services.OrderByDescending(s => s.ServiceNumber),
                };
            }
            else
            {
                services = message.SortBy switch
                {
                    ProviderServicesSortBy.ServiceNumber => services.OrderBy(s => s.ServiceNumber),
                    ProviderServicesSortBy.Name => services.OrderBy(s => s.Name),
                    ProviderServicesSortBy.IsEmpowerment => services.OrderBy(s => s.IsEmpowerment),
                    ProviderServicesSortBy.MinimumLevelOfAssurance => services.OrderBy(s => s.MinimumLevelOfAssurance),
                    _ => services.OrderBy(s => s.ServiceNumber),
                };
            }
        }
        else
        {
            services = services.OrderBy(s => s.ServiceNumber);
        }

        // Pagination
        // Execute query
        var result = await GetPaginatedAsync<ProviderServiceResult>(services, message.PageIndex, message.PageSize);

        // Result
        return Ok(result);
    }

    public async Task<ServiceResult<ProviderServiceResult>> GetServiceByIdAsync(GetServiceById message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetServiceByIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<ProviderServiceResult>(validationResult.Errors);
        }

        // Action
        // Execute query
        var result = await _context.ProvidersDetailsServices
            .FirstOrDefaultAsync(b => b.Id == message.Id);

        if (result == null)
        {
            return NotFound<ProviderServiceResult>(nameof(message.Id), message.Id);
        }

        // Result
        return Ok((ProviderServiceResult)result);
    }

    public async Task<ServiceResult<IEnumerable<ServiceScopeResult>>> GetAllServiceScopesAsync(GetAllServiceScopes message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetAllServiceScopesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<ServiceScopeResult>>(validationResult.Errors);
        }

        // Action
        // Call from public admin
        if (message.ProviderId != Guid.Empty)
        {
            var serviceBelongsToProvider = await _context.ProvidersDetailsServices
                .AnyAsync(pds => pds.Id == message.ServiceId &&
                    pds.ProviderDetails.Providers.Any(p => p.Id == message.ProviderId));

            if (!serviceBelongsToProvider)
            {
                return Forbidden<IEnumerable<ServiceScopeResult>>("You are not allowed to access this service.");
            }
        }

        // Prepare query
        var serviceScopes = _context.ServiceScopes
            .Where(s => s.ProviderServiceId == message.ServiceId)
            .AsQueryable();

        if (message.IsPLSRole)
        {
            serviceScopes = serviceScopes.Where(ss => ss.ProviderService.ProviderDetails.Providers.Any(p => p.Type == ProviderType.PrivateLawSubject));
        }

        // Pagination
        // Execute query
        var result = await serviceScopes
            .OrderBy(s => s.Name)
            .OfType<ServiceScopeResult>()
            .ToListAsync();

        // Result
        return Ok(result.AsEnumerable());
    }

    public async Task<ServiceResult<Guid>> UpdateServiceAsync(UpdateService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new UpdateServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        if (message.ProviderId != Guid.Empty)
        {
            var detailsBelongToCurrentProvider = await _context.Providers.Include(pr => pr.Details)
                .AnyAsync(pr => pr.Id == message.ProviderId && pr.DetailsId == message.ProviderDetailsId);

            if (!detailsBelongToCurrentProvider)
            {
                _logger.LogError("User {UserId} tried to update Service {ServiceId} for Details {DetailsId} of Provider {ProviderId}.", message.UserId, message.ServiceId, message.ProviderDetailsId, message.ProviderId);
                return Forbidden<Guid>("You are not allowed to update this service.");
            }
        }

        var service = await _context.ProvidersDetailsServices
            .Include(s => s.ServiceScopes)
            .FirstOrDefaultAsync(ss => ss.Id == message.ServiceId && ss.ProviderDetailsId == message.ProviderDetailsId);

        if (service is null)
        {
            _logger.LogWarning("Service {ServiceId} not found.", message.ServiceId);
            return NotFound<Guid>(nameof(message.ServiceId), message.ServiceId);
        }
        if (service.Status != ProviderServiceStatus.Approved)
        {
            _logger.LogWarning("Service {ServiceId} is {Status} and cannot be updated.", message.ServiceId, service.Status);
            return ConflictMessage<Guid>(nameof(service.Status), $"Only {ProviderServiceStatus.Approved} services can be updated.");
        }

        if (service.IsEmpowerment != message.IsEmpowerment)
        {
            service.IsEmpowerment = message.IsEmpowerment;
        }

        service.LastModifiedOn = DateTime.UtcNow;
        service.MinimumLevelOfAssurance = message.MinimumLevelOfAssurance;
        service.RequiredPersonalInformation = message.RequiredPersonalInformation.ToHashSet();

        // Update scopes
        var databaseScopeNamesSet = service.ServiceScopes.Select(f => f.Name.ToUpper()).ToList().ToHashSet();
        var messageScopeNamesSet = message.ServiceScopeNames.Select(f => f.ToUpper()).ToHashSet();

        // Remove every scope except common scopes
        var scopeNamesForDeletionSet = databaseScopeNamesSet.Except(messageScopeNamesSet).ToHashSet();
        var scopesToRemove = service.ServiceScopes.Where(sc => scopeNamesForDeletionSet.Contains(sc.Name.ToUpper()));
        foreach (var scopeToRemove in scopesToRemove)
        {
            service.ServiceScopes.Remove(scopeToRemove);
        }

        // Add new
        var newScopeNamesSet = messageScopeNamesSet.Except(databaseScopeNamesSet).ToHashSet();
        var scopesToAdd = message.ServiceScopeNames
                            .Where(name => newScopeNamesSet.Contains(name.ToUpper()))
                            .Select(name => new ServiceScope
                            {
                                Name = name,
                                CreatedOn = DateTime.UtcNow,
                                CreatedBy = message.UserId
                            });
        foreach (var scopeToAdd in scopesToAdd)
        {
            service.ServiceScopes.Add(scopeToAdd);
        }

        await _context.SaveChangesAsync();

        // Invalidate cache
        var key = CacheKeyHelper.GetProviderServices(message.ProviderId);
        await _cache.RemoveAsync(key);

        _logger.LogInformation("Service {ServiceId} updated. IsEmpowerment: {IsEmpowerment}, ServiceScopes: {ServiceScopes}", message.ServiceId, message.IsEmpowerment, message.ServiceScopeNames);

        // Result
        return Ok(service.Id);
    }

    public async Task<ServiceResult<Guid>> RegisterServiceAsync(RegisterService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new RegisterServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<Guid>(validationResult.Errors);
        }

        if (message.ProviderId != Guid.Empty)
        {
            var detailsBelongToCurrentProvider = await _context.Providers.Include(pr => pr.Details)
                .AnyAsync(pr => pr.Id == message.ProviderId && pr.DetailsId == message.ProviderDetailsId);

            if (!detailsBelongToCurrentProvider)
            {
                _logger.LogError("User {UserId} tried to register Service {ServiceName} for Details {DetailsId} of Provider {ProviderId}.", message.UserId, message.Name, message.ProviderDetailsId, message.ProviderId);
                return Forbidden<Guid>("You are not allowed to register services for this provider.");
            }
        }

        var providerDetail = await _context.ProvidersDetails
            .Include(pd => pd.Providers)
            .FirstOrDefaultAsync(pd => pd.Id == message.ProviderDetailsId);
        if (providerDetail is null)
        {
            _logger.LogWarning("Provider details {ProviderDetailsId} not found.", message.ProviderDetailsId);
            return NotFound<Guid>(nameof(message.ProviderDetailsId), message.ProviderDetailsId);
        }

        // We're complying to IISDA's structure and having a section is required.
        // We create a new section in order to satisfy that constraint.
        var providerDetailSection = await _context.ProvidersDetailsSections.FirstOrDefaultAsync(pd => pd.ProviderDetailsId == message.ProviderDetailsId);
        if (providerDetailSection is null)
        {
            _logger.LogInformation("Creating default section for Provider details id {ProviderDetailsId}", message.ProviderDetailsId);
            providerDetailSection = new ProviderSection
            {
                Id = Guid.NewGuid(),
                Name = providerDetail.Name,
                ProviderDetailsId = providerDetail.Id
            };
        }

        var service = new ProviderService
        {
            Id = Guid.NewGuid(),
            ProviderDetails = providerDetail,
            ProviderDetailsId = message.ProviderDetailsId,
            ProviderSection = providerDetailSection,
            ProviderSectionId = providerDetailSection.Id,
            Name = message.Name,
            Description = message.Description,
            PaymentInfoNormalCost = message.PaymentInfoNormalCost,
            RequiredPersonalInformation = message.RequiredPersonalInformation.ToHashSet(),
            MinimumLevelOfAssurance = message.MinimumLevelOfAssurance,
            IsEmpowerment = message.IsEmpowerment,
            CreatedOn = DateTime.UtcNow,
            Status = ProviderServiceStatus.InReview
        };

        // Add new
        var messageScopeNamesSet = message.ServiceScopeNames.Select(f => f.ToUpper()).ToHashSet();
        var scopesToAdd = messageScopeNamesSet
                            .Select(name => new ServiceScope
                            {
                                ProviderService = service,
                                ProviderServiceId = service.Id,
                                Name = name,
                                CreatedOn = DateTime.UtcNow,
                                CreatedBy = message.UserId
                            });

        _context.ServiceScopes.AddRange(scopesToAdd);

        await _context.SaveChangesAsync();

        // Invalidate cache
        var key = CacheKeyHelper.GetProviderServices(message.ProviderId);
        await _cache.RemoveAsync(key);

        SendEmail(message.CorrelationId, _notificationEmailsOptions.DEAUActions, "Заявена услуга от ДЕАУ", "bg",
            $"ДЕАУ {providerDetail.Provider.Name} е заявил услуга, която трябва да бъде одобрена или отхвърлена.",
            "Failed to send MoI register service email to {MoIEmail}", _notificationEmailsOptions.DEAUActions.MaskEmail());

        _logger.LogInformation("Service {ServiceName} registered for provider details {ProviderDetailsId}.", message.Name, message.ProviderDetailsId);

        // Result
        return Created(service.Id);
    }

    public async Task<ServiceResult<IEnumerable<string>>> GetDefaultServiceScopesAsync()
    {
        var result = await _context.DefaultServiceScopes.Select(d => d.Name).ToArrayAsync();

        return Ok(result.AsEnumerable());
    }

    public async Task<ServiceResult<IEnumerable<string>>> GetAvailableScopesByProviderIdAsync(GetAvailableScopesByProviderId message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetAvailableScopesByProviderIdValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<string>>(validationResult.Errors);
        }
        var provider = await _context.Providers.Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == message.ProviderId);
        if (provider is null)
        {
            return NotFound<IEnumerable<string>>(nameof(message.ProviderId), message.ProviderId);
        }
        var result = await _context
                        .ServiceScopes
                        .Where(scope => scope.ProviderService.ProviderDetailsId == provider.DetailsId)
                        .Select(scope => scope.Name)
                        .Distinct()
                        .OrderBy(name => name)
                        .ToListAsync();

        return Ok(result.AsEnumerable());
    }
    public async Task<ServiceResult<ProviderDetailsResult>> GetCurrentProviderDetailsAsync(GetCurrentProviderDetails message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetCurrentProviderDetailsValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<ProviderDetailsResult>(validationResult.Errors);
        }
        var provider = await _context.Providers.Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == message.ProviderId);
        if (provider is null)
        {
            return NotFound<ProviderDetailsResult>(nameof(message.ProviderId), message.ProviderId);
        }
        if (provider.Details is null)
        {
            return NotFound<ProviderDetailsResult>(nameof(provider.Details), $"Provider details not found for provider {message.ProviderId}");
        }

        return Ok(provider.Details as ProviderDetailsResult);
    }

    public async Task<ServiceResult> ActivateServiceAsync(ActivateService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ActivateServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var provider = await _context.Providers
            .Include(p => p.Users.Where(u => u.IsDeleted == false))
            .FirstOrDefaultAsync(p => p.Id == message.ProviderId);
        if (provider is null)
        {
            _logger.LogWarning("Provider {ProviderId} not found when trying to activate service {ServiceId}.", message.ProviderId, message.ServiceId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }
        var userIsAdmin = provider.Users.Any(u => u.Uid == message.Uid && u.UidType == message.UidType && u.IsAdministrator);
        if (!userIsAdmin)
        {
            _logger.LogWarning("User {UserId} tried to deactivate service {ServiceId} for provider {ProviderId}", message.UserId, message.ServiceId, message.ProviderId);
            return Forbidden("You don't have the permission to deactivate the service.");
        }
        var service = await _context.ProvidersDetailsServices.FirstOrDefaultAsync(s => s.Id == message.ServiceId);
        if (service is null)
        {
            _logger.LogWarning("Provider service {ServiceId} not found.", message.ServiceId);
            return NotFound(nameof(message.ServiceId), $"Provider service {message.ServiceId} not found");
        }
        service.IsActive = true;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Provider service {ServiceId} activated.", message.ServiceId);
        return NoContent();
    }

    public async Task<ServiceResult> DeactivateServiceAsync(DeactivateService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DeactivateServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var provider = await _context.Providers
            .Include(p => p.Users.Where(u => u.IsDeleted == false))
            .FirstOrDefaultAsync(p => p.Id == message.ProviderId);
        if (provider is null)
        {
            _logger.LogWarning("Provider {ProviderId} not found when trying to deactivate service {ServiceId}.", message.ProviderId, message.ServiceId);
            return NotFound(nameof(message.ProviderId), message.ProviderId);
        }

        var userIsAdmin = provider.Users.Any(u => u.Uid == message.Uid && u.UidType == message.UidType && u.IsAdministrator);
        if (!userIsAdmin)
        {
            _logger.LogWarning("User {UserId} tried to deactivate service {ServiceId} for provider {ProviderId}", message.UserId, message.ServiceId, message.ProviderId);
            return Forbidden("You don't have the permission to deactivate the service.");
        }
        var service = await _context.ProvidersDetailsServices.FirstOrDefaultAsync(s => s.Id == message.ServiceId);
        if (service is null)
        {
            _logger.LogWarning("Provider service {ServiceId} not found.", message.ServiceId);
            return NotFound(nameof(message.ServiceId), $"Provider service {message.ServiceId} not found");
        }
        service.IsActive = false;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Provider service {ServiceId} deactivated.", message.ServiceId);
        return NoContent();
    }

    public async Task<ServiceResult> ApproveServiceAsync(ApproveService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new ApproveServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var service = await _context.ProvidersDetailsServices.FirstOrDefaultAsync(s => s.Id == message.ServiceId);
        if (service is null)
        {
            _logger.LogWarning("Provider service {ServiceId} not found.", message.ServiceId);
            return NotFound(nameof(message.ServiceId), $"Provider service {message.ServiceId} not found");
        }
        if (service.Status != ProviderServiceStatus.InReview)
        {
            _logger.LogWarning("Service {ServiceId} is {Status} and cannot be approved.", message.ServiceId, service.Status);
            return ConflictMessage<Guid>(nameof(service.Status), $"Only {ProviderServiceStatus.InReview} services can be updated.");
        }

        var providers = _context.Providers
                .Include(p => p.Users.Where(u => u.IsDeleted == false))
                .AsQueryable();

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        var provider = await providers.FirstOrDefaultAsync(p => p.DetailsId == service.ProviderDetailsId);
        if (provider is null)
        {
            _logger.LogWarning("Provider not found when trying to approve service {ServiceId}.", message.ServiceId);
            return NotFound(nameof(message.ServiceId), $"Provider referencing service {message.ServiceId} not found");
        }

        service.Status = ProviderServiceStatus.Approved;
        service.DenialReason = string.Empty;
        service.ReviewerFullName = message.ReviewerFullName;
        service.LastModifiedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        SendEmail(message.CorrelationId, provider.Email, "Ваша услуга беше одобрена", "bg",
            $"Услугата {service.Name} беше одобрена.",
            "Failed to send provider issuer approved service email to {ProviderEmail}",
                provider.Email.MaskEmail());

        if (!provider.Users.Any())
        {
            _logger.LogWarning("Administrator for ProviderId {ProviderId} is not found", provider.Id);
        }
        else
        {
            var adminEmail = provider.Users.First().Email;

            SendEmail(message.CorrelationId, adminEmail, "Ваша услуга беше одобрена", "bg",
                $"Услугата {service.Name} беше одобрена.",
                "Failed to send provider admin approved service email. ProviderId {ProviderId} admin email {AdminEmail}",
                    provider.Id, adminEmail.MaskEmail());
        }

        _logger.LogInformation("Provider service {ServiceId} approved.", message.ServiceId);
        return NoContent();
    }

    public async Task<ServiceResult> DenyServiceAsync(DenyService message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new DenyServiceValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var service = await _context.ProvidersDetailsServices.FirstOrDefaultAsync(s => s.Id == message.ServiceId);
        if (service is null)
        {
            _logger.LogWarning("Provider service {ServiceId} not found.", message.ServiceId);
            return NotFound(nameof(message.ServiceId), $"Provider service {message.ServiceId} not found");
        }
        if (service.Status == ProviderServiceStatus.Denied)
        {
            _logger.LogWarning("Service {ServiceId} is already {Status}.", message.ServiceId, service.Status);
            return ConflictMessage<Guid>(nameof(service.Status), $"Service already {ProviderServiceStatus.Denied}.");
        }

        var providers = _context.Providers
                .Include(p => p.Users.Where(u => u.IsDeleted == false))
                .AsQueryable();

        if (message.IsPLSRole)
        {
            providers = providers.Where(p => p.Type == ProviderType.PrivateLawSubject);
        }

        var provider = await providers
                .FirstOrDefaultAsync(p => p.DetailsId == service.ProviderDetailsId);
        if (provider is null)
        {
            _logger.LogWarning("Provider not found when trying to approve service {ServiceId}.", message.ServiceId);
            return NotFound(nameof(message.ServiceId), $"Provider referencing service {message.ServiceId} not found");
        }

        service.Status = ProviderServiceStatus.Denied;
        service.DenialReason = message.DenialReason;
        service.ReviewerFullName = message.ReviewerFullName;
        service.LastModifiedOn = DateTime.UtcNow;
        service.DeniedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        SendEmail(message.CorrelationId, provider.Email, "Ваша услуга беше отказана", "bg",
            $"Услугата {service.Name} беше отказана с причина: \"{service.DenialReason}\".",
            "Failed to send provider issuer denied email Uid: {Uid} UidType: {UidType}",
                provider.IssuerUid.Mask(), provider.IssuerUidType);

        if (!provider.Users.Any())
        {
            _logger.LogWarning("Administrator for ProviderId {ProviderId} is not found", provider.Id);
        }
        else
        {
            var adminEmail = provider.Users.First().Email;
            SendEmail(message.CorrelationId, adminEmail, "Ваша услуга беше отказана", "bg",
                $"Услугата {service.Name} беше отказана с причина: \"{service.DenialReason}\".",
                "Failed to send provider admin denied email for ProviderId {ProviderId} admin email {AdminEmail}",
                    provider.Id, adminEmail.MaskEmail());
        }
        _logger.LogInformation("Provider service {ServiceId} denied.", message.ServiceId);
        return NoContent();
    }

    private void SendEmail(Guid correlationId, string email, string subject, string language, string body, string? message, params object?[] args)
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        // When there's an issue with email sending, waiting could timeout the http request.
        Task.Run(async () =>
        {
            var confirmationEmailSend = await _notificationSender.SendEmailAsync(new SendEmailRequest
            {
                CorrelationId = correlationId,
                Email = email,
                Subject = subject,
                Language = language,
                Body = body
            });

            if (!confirmationEmailSend)
            {
                _logger.LogWarning(message, args);
            }
        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
}
