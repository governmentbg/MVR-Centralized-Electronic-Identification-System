using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service.Database;
using eID.PIVR.Service.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eID.PIVR.Service;

public class IdentityChecksService : BaseService
{
    private readonly ILogger<IdentityChecksService> _logger;
    private readonly ApplicationDbContext _context;

    public IdentityChecksService(ILogger<IdentityChecksService> logger, ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ServiceResult<IEnumerable<IdChangeResult>>> GetIdChangesAsync(GetIdChanges message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetIdChangesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<IdChangeResult>>(validationResult.Errors);
        }

        var changedUIds = _context.IdChanges
            .AsQueryable();

        if (!string.IsNullOrEmpty(message.PersonalId) && message.UidType.HasValue)
        {
            // Get all NewPersonalId or OldPersonalId equal to PersonalId
            changedUIds = changedUIds.Where(d =>
                (d.NewPersonalId == message.PersonalId && d.NewUidType == message.UidType) ||
                (d.OldPersonalId == message.PersonalId && d.OldUidType == message.UidType));
        }

        if (message.CreatedOnFrom.HasValue && message.CreatedOnTo.HasValue)
        {
            var startDate = message.CreatedOnFrom.Value.Date.ToUniversalTime(); // From 00:00:00
            var endDate = message.CreatedOnTo.Value.Date.ToUniversalTime().AddDays(1).AddMilliseconds(-1); // To 23:59:59.999

            changedUIds = changedUIds.Where(d =>
                d.CreatedOn >= startDate && d.CreatedOn <= endDate);
        }

        var result = await changedUIds
            .Cast<IdChangeResult>()
            .ToListAsync();

        return Ok(result.AsEnumerable());
    }

    public async Task<ServiceResult<IEnumerable<StatutChangeResult>>> GetStatutChangesAsync(GetStatutChanges message)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        // Validation
        var validator = new GetStatutChangesValidator();
        var validationResult = await validator.ValidateAsync(message);
        if (!validationResult.IsValid)
        {
            return BadRequest<IEnumerable<StatutChangeResult>>(validationResult.Errors);
        }

        var statutChanges = _context.StatutChanges
            .AsQueryable();

        if (!string.IsNullOrEmpty(message.PersonalId) && message.UidType.HasValue)
        {
            statutChanges = statutChanges.Where(d =>
                (d.PersonalId == message.PersonalId && d.UidType == message.UidType));
        }

        if (message.CreatedOnFrom.HasValue && message.CreatedOnTo.HasValue)
        {
            var startDate = message.CreatedOnFrom.Value.Date.ToUniversalTime(); // From 00:00:00
            var endDate = message.CreatedOnTo.Value.Date.ToUniversalTime().AddDays(1).AddMilliseconds(-1); // To 23:59:59.999

            statutChanges = statutChanges.Where(d =>
                d.CreatedOn >= startDate && d.CreatedOn <= endDate);
        }

        var result = await statutChanges
            .Cast<StatutChangeResult>()
            .ToListAsync();

        return Ok(result.AsEnumerable());
    }
}
