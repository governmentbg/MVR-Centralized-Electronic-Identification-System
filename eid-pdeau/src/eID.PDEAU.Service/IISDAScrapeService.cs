using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AdmServicesService = IISDA.AdmServicesService;
using BatchInfoService = IISDA.BatchInfoService;

namespace eID.PDEAU.Service;

public class IISDAScrapeService
{
    private readonly ILogger<IISDAScrapeService> _logger;
    private readonly ApplicationDbContext _context;

    public IISDAScrapeService(
        ILogger<IISDAScrapeService> logger,
        ApplicationDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task ScrapeAsync()
    {
        var deletedBatches = 0;
        var addedBatches = 0;
        var updatedBatches = 0;
        var deletedServices = 0;
        var addedServices = 0;
        var updatedServices = 0;

        try
        {
            _logger.LogInformation("Starting IISDA BatchInfoService SearchBatchesIdentificationInfo...");
            var remoteBatches = await GetRemoteActiveBatchesAsync();
            _logger.LogInformation("{BatchIdCount} IISDA BatchInfoService.SearchBatchesIdentificationInfo have been received", remoteBatches.Count);

            var dbBatches = await _context.ProvidersDetails.Where(b => b.SyncedFromOnlineRegistry).ToArrayAsync();
            // All SyncedFromOnlineRegistry contains IdentificationNumber
            var dbBatchesMap = dbBatches.ToDictionary(k => k.GetIdentificationNumber(), v => v);
            _logger.LogInformation("{BatchIdCount} Current IISDA Batches were get", dbBatchesMap.Count);

            var currentSections = _context.ProvidersDetailsSections.Where(s => s.SyncedFromOnlineRegistry).ToList();

            // Mark dbBatches that are no longer available in the remote as deleted
            var missingBatches = dbBatchesMap.Values
                .Where(dbBatch => !dbBatch.IsDeleted && !remoteBatches.ContainsKey(dbBatch.GetIdentificationNumber()));
            foreach (var missingBatch in missingBatches)
            {
                missingBatch.IsDeleted = true;
                deletedBatches++;
            }

            // Add IdentificationNumber that dont exist in the db as new batches
            var newBatches = remoteBatches
                .Where(remoteBatch => !dbBatchesMap.ContainsKey(remoteBatch.Key));

            foreach (var newBatch in newBatches)
            {
                _context.ProvidersDetails.Add(
                    new ProviderDetails
                    {
                        Id = Guid.NewGuid(),
                        IdentificationNumber = newBatch.Key,
                        Name = newBatch.Value.Name,
                        IsDeleted = false,
                        SyncedFromOnlineRegistry = true,
                        Status = ProviderDetailsStatus.Deactivated,
                        UIC = newBatch.Value.UIC,
                        Address = newBatch.Value.Address,
                        Headquarters = newBatch.Value.Headquarters,
                        WebSiteUrl = newBatch.Value.WebSiteUrl,
                        WorkingTimeStart = newBatch.Value.WorkingTimeStart,
                        WorkingTimeEnd = newBatch.Value.WorkingTimeEnd
                    });

                addedBatches++;
            }
            _context.SaveChanges();

            // Apply updates over batches that were previously scraped
            var existingBatches =
                    dbBatchesMap.Values.Where(dbBatch => remoteBatches.ContainsKey(dbBatch.GetIdentificationNumber()));
            foreach (var dbBatch in existingBatches)
            {
                // It's safe to access this index, due to the check in Where clause
                var remoteBatch = remoteBatches[dbBatch.GetIdentificationNumber()];

                // IsDeleted (soft delete flag) can be raised only during scrape, when it's no longer available in the remote.
                // When returning or the name or UIC are being updated the flag will be set to false
                if (!dbBatch.Name.Equals(remoteBatch.Name, StringComparison.CurrentCultureIgnoreCase)
                        || !dbBatch.UIC.Equals(remoteBatch.UIC, StringComparison.CurrentCultureIgnoreCase)
                        || !dbBatch.Address.Equals(remoteBatch.Address, StringComparison.CurrentCultureIgnoreCase)
                        || !dbBatch.Headquarters.Equals(remoteBatch.Headquarters, StringComparison.CurrentCultureIgnoreCase)
                        || !dbBatch.WebSiteUrl.Equals(remoteBatch.WebSiteUrl, StringComparison.CurrentCultureIgnoreCase)
                        || !dbBatch.WorkingTimeStart.Equals(remoteBatch.WorkingTimeStart, StringComparison.CurrentCultureIgnoreCase)
                        || !dbBatch.WorkingTimeEnd.Equals(remoteBatch.WorkingTimeEnd, StringComparison.CurrentCultureIgnoreCase)
                        || dbBatch.IsDeleted)
                {
                    dbBatch.Name = remoteBatch.Name;
                    dbBatch.UIC = remoteBatch.UIC;
                    dbBatch.IsDeleted = false;
                    dbBatch.Address = remoteBatch.Address;
                    dbBatch.Headquarters = remoteBatch.Headquarters;
                    dbBatch.WebSiteUrl = remoteBatch.WebSiteUrl;
                    dbBatch.WorkingTimeStart = remoteBatch.WorkingTimeStart;
                    dbBatch.WorkingTimeEnd = remoteBatch.WorkingTimeEnd;
                    updatedBatches++;
                }
                // Update services for batches that are marked as Active from an administrator
                if (dbBatch.Status == ProviderDetailsStatus.Active)
                {
                    var services = await ProcessServicesByIdentificationNumberAsync(dbBatch.Id, dbBatch.GetIdentificationNumber(), currentSections);
                    var servicesMap = services.ToDictionary(k => k.ServiceNumber, v => v);
                    var currentServices = await _context.ProvidersDetailsServices.Where(s => s.ProviderDetailsId == dbBatch.Id && s.SyncedFromOnlineRegistry).ToArrayAsync();
                    var currentServicesMap = currentServices.ToDictionary(k => k.ServiceNumber, v => v);
                    (int deleted, int added, int updated) = ProcessServices(servicesMap, currentServicesMap);

                    deletedServices += deleted;
                    addedServices += added;
                    updatedServices += updated;
                }
                _context.SaveChanges();
            }

            _logger.LogInformation("Scrape IISDA finished successfully. " +
                "DeletedBatches {DeletedBatches}, AddedBatches {AddedBatches}, UpdatedBatches {UpdatedBatches}, " +
                "DeletedServices {DeletedServices}, AddedServices {AddedServices}, UpdatedServices {UpdatedServices}",
                deletedBatches, addedBatches, updatedBatches,
                deletedServices, addedServices, updatedServices);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during Scraping IISDA. " +
                "DeletedBatches {DeletedBatches}, AddedBatches {AddedBatches}, UpdatedBatches {UpdatedBatches}, " +
                "DeletedServices {DeletedServices}, AddedServices {AddedServices}, UpdatedServices {UpdatedServices}",
                deletedBatches, addedBatches, updatedBatches,
                deletedServices, addedServices, updatedServices);

            throw;
        }
    }

    private (int deletedServices, int addedServices, int updatedServices) ProcessServices(
        Dictionary<long, ProviderService> remoteServicesMap, Dictionary<long, ProviderService> dbServicesMap)
    {
        var deletedSevices = 0;
        var addedSevices = 0;
        var updatedSevices = 0;

        // Raising soft delete flag for services that are no longer listed by the remote
        var deletedServices = dbServicesMap.Values
            .Where(dbService => !dbService.IsDeleted && !remoteServicesMap.ContainsKey(dbService.ServiceNumber));
        foreach (var dbService in deletedServices)
        {
            dbService.IsDeleted = true;
            deletedSevices++;
        }

        // Add new services
        var newServices = remoteServicesMap.Values.Where(s => !dbServicesMap.ContainsKey(s.ServiceNumber));
        foreach (var service in newServices)
        {
            _context.ProvidersDetailsServices.Add(service);
            addedSevices++;
        }

        // Update existing services
        var existingServices
            = dbServicesMap.Values.Where(cs => remoteServicesMap.ContainsKey(cs.ServiceNumber));
        foreach (var dbService in existingServices)
        {
            var remoteService = remoteServicesMap[dbService.ServiceNumber];
            if (dbService.ServiceNumber != remoteService.ServiceNumber
                || !dbService.Name.Equals(remoteService.Name, StringComparison.CurrentCultureIgnoreCase)
                || !string.Equals(dbService.Description, remoteService.Description, StringComparison.CurrentCultureIgnoreCase)
                || dbService.IsDeleted
                || dbService.PaymentInfoNormalCost != remoteService.PaymentInfoNormalCost
                || dbService.ProviderSectionId != remoteService.ProviderSectionId)
            {
                dbService.ServiceNumber = remoteService.ServiceNumber;
                dbService.Name = remoteService.Name;
                dbService.Description = remoteService.Description;
                dbService.IsDeleted = false;
                dbService.PaymentInfoNormalCost = remoteService.PaymentInfoNormalCost;
                dbService.ProviderSectionId = remoteService.ProviderSectionId;
                updatedSevices++;
            }
        }

        return (deletedSevices, addedSevices, updatedSevices);
    }

    private async Task<Dictionary<string, BatchData>> GetRemoteActiveBatchesAsync()
    {
        Dictionary<string, BatchData> result;
        using (var client = new BatchInfoService.BatchInfoServiceClient(BatchInfoService.BatchInfoServiceClient.EndpointConfiguration.WSHttpBinding_IBatchInfoService))
        {
            // Only Active Batches will be got
            var batchResponse = await client.SearchBatchesIdentificationInfoAsync("", "", null, null, BatchInfoService.BatchStatusEnum.Active, null, null);

            // The batches with UIC will be processed
            result = batchResponse.SearchBatchesIdentificationInfoResult
                .Where(d => !string.IsNullOrWhiteSpace(d.UIC))
                .ToDictionary(
                    k => k.IdentificationNumber,
                    v => new BatchData { Name = v.Name, UIC = v.UIC });

            _logger.LogDebug("IISDA {Count} batches information will be processed", result.Count);

            var skip = 0;
            const int takeCount = 50;
            var processed = 0;
            // Add address information
            while (true)
            {
                var identificationNumbers = result.Skip(skip).Take(takeCount).Select(d => d.Key).ToArray();
                if (identificationNumbers.Length == 0)
                {
                    break;
                }

                // Process
                var batchDetailedInfoResponse = await client.GetBatchDetailedInfoAsync(identificationNumbers, null, null);
                _logger.LogDebug("Got {BatchDetailedInfoResponse} BatchDetailedInfo form {IdentificationNumbers} requested Ids", batchDetailedInfoResponse.GetBatchDetailedInfoResult.Length, identificationNumbers.Length);
                // There are missing information
                if (batchDetailedInfoResponse.GetBatchDetailedInfoResult.Length != identificationNumbers.Length)
                {
                    var missingIdentificationNumbers = identificationNumbers.Except(batchDetailedInfoResponse.GetBatchDetailedInfoResult.Select(d => d.IdentificationNumber));

                    _logger.LogWarning("Missing BatchDetailedInfo for IdentificationNumbers: {MissingIdentificationNumbers}", string.Join(",", missingIdentificationNumbers));
                }
                processed += identificationNumbers.Length;
                _logger.LogInformation("Processed {InfoCount}/{BatchCount} IISDA BatchDetailedInfo", processed, result.Count);

                foreach (var batchDetailedInfo in batchDetailedInfoResponse.GetBatchDetailedInfoResult)
                {
                    if (result.TryGetValue(batchDetailedInfo.IdentificationNumber, out var value))
                    {
                        value.Address = batchDetailedInfo.Administration.Address.AddressText;
                        var adr = batchDetailedInfo.Administration.Address.EkatteAddress;

                        var settlementName = adr.SettlementName;
                        if (!string.IsNullOrWhiteSpace(batchDetailedInfo.Administration.Address.PostCode))
                        {
                            settlementName = $"{batchDetailedInfo.Administration.Address.PostCode} {settlementName}";
                        }
                        var headquartersAddressValues = new string[] { settlementName, adr.DistrictName, adr.MunicipalityName }.Where(addr => !string.IsNullOrWhiteSpace(addr));
                        value.Headquarters = string.Join(", ", headquartersAddressValues);
                        value.WebSiteUrl = batchDetailedInfo.Administration.CorrespondenceData.WebSiteUrl;
                        value.WorkingTimeStart = batchDetailedInfo.Administration.WorkingTime.StartTime;
                        value.WorkingTimeEnd = batchDetailedInfo.Administration.WorkingTime.EndTime;
                    }
                }

                skip += takeCount;
            }
        }

        return result;
    }

    private async Task<ProviderService[]> ProcessServicesByIdentificationNumberAsync(Guid iisdaBatchId, string identificationNumber,
        List<ProviderSection> currentSections)
    {
        _logger.LogDebug("Requesting AdmServicesService_SearchAdmServicesAsync({IdentificationNumber})", identificationNumber);

        AdmServicesService.SearchAdmServicesResponse response;
        using (var client = new AdmServicesService.AdmServicesServiceClient(AdmServicesService.AdmServicesServiceClient.EndpointConfiguration.BasicHttpsBinding_IAdmServicesService))
        {
            response = await client.SearchAdmServicesAsync(identificationNumber, "", null, null);
        }

        _logger.LogDebug("AdmServicesService_SearchAdmServicesAsync({IdentificationNumber}) received ({Length}) results",
            identificationNumber, response.SearchAdmServicesResult.Length);

        // Process sections
        var sections = response.SearchAdmServicesResult.Select(sr => sr.AdmServiceData.SectionName).Distinct()
            .Where(s => !string.IsNullOrEmpty(s)).ToArray();

        // Add new sections
        sections.Where(s => !currentSections.Any(cs => cs.Name.Equals(s, StringComparison.InvariantCultureIgnoreCase)))
            .ToList()
            .ForEach(section =>
            {
                var entity = new ProviderSection
                {
                    Id = Guid.NewGuid(),
                    ProviderDetailsId = iisdaBatchId,
                    Name = section,
                    SyncedFromOnlineRegistry = true,
                    IsDeleted = false
                };
                // Mutate currentSections list
                currentSections.Add(entity);
                _context.ProvidersDetailsSections.Add(entity);
            });

        return response.SearchAdmServicesResult.Select(sr => new ProviderService
        {
            Id = Guid.NewGuid(),
            ServiceNumber = sr.AdmServiceData.ServiceNumber,
            Name = sr.AdmServiceData.Name,
            Description = sr.AdmServiceData.Description,
            PaymentInfoNormalCost = sr.AdmServiceBatchInfo?.PaymentInfo?.NormalCost,
            IsEmpowerment = false,
            SyncedFromOnlineRegistry = true,
            IsDeleted = false,
            ProviderDetailsId = iisdaBatchId,
            ProviderSectionId = currentSections.First(sc => sc.Name.Equals(sr.AdmServiceData.SectionName, StringComparison.CurrentCultureIgnoreCase)).Id,
            CreatedOn = DateTime.UtcNow,
            Status = ProviderServiceStatus.Approved
        }).ToArray();
    }

    private class BatchData
    {
        public string Name { get; set; } = string.Empty;
        public string UIC { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Headquarters { get; set; } = string.Empty;
        public string WebSiteUrl { get; set; } = string.Empty;
        public string WorkingTimeStart { get; set; } = string.Empty;
        public string WorkingTimeEnd { get; set; } = string.Empty;
    }
}
