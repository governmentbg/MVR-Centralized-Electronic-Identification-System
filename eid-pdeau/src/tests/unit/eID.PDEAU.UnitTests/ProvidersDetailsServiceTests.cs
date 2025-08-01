using System.Net;
using eID.PDEAU.Contracts.Commands;
using eID.PDEAU.Contracts.Enums;
using eID.PDEAU.Service;
using eID.PDEAU.Service.Database;
using eID.PDEAU.Service.Entities;
using eID.PDEAU.Service.Options;
using eID.PDEAU.UnitTests.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace eID.PDEAU.UnitTests;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class ProvidersDetailsServiceTests : BaseTest
{
    private ILogger<ProvidersDetailsService> _logger;
    private ApplicationDbContext _dbContext;
    private IDistributedCache _cache;
    private ProvidersDetailsService _sut;

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<ProvidersDetailsService>();

        _dbContext = GetTestDbContext();
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);
        var notificationSender = new Mock<INotificationSender>();
        var notificationEmailsOptions = Options.Create(new NotificationEmailsOptions { DEAUActions = "test@test.com" });

        _sut = new ProvidersDetailsService(_logger, _cache, _dbContext, notificationSender.Object, notificationEmailsOptions);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    [TestCaseSource(nameof(UpdateServiceInvalidDataTestCases))]
    public async Task UpdateServiceAsync_WhenCallWithInvalidData_ShouldReturnBadRequestAsync(UpdateService message, string caseName)
    {
        // Arrange
        // Act
        var result = await _sut.UpdateServiceAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task UpdateServiceAsync_WhenCallWithValidData_ShouldReturnOkRequestAsync()
    {
        // Arrange
        var (provider, service) = await AddProviderDetailsAndServiceTaskAsync();

        var serviceScope = new ServiceScope
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),

            ProviderService = service
        };
        await _dbContext.ServiceScopes.AddAsync(serviceScope);
        await _dbContext.SaveChangesAsync();

        var scopes = new List<string> { "1", "2" };

        var message = CreateInterface<UpdateService>(new
        {
            CorrelationId = Guid.NewGuid(),
            ProviderDetailsId = provider.Details.Id,
            UserId = Guid.NewGuid(),
            ServiceId = service.Id,
            IsEmpowerment = true,
            ServiceScopeNames = scopes,
            RequiredPersonalInformation = new List<CollectablePersonalInformation> { CollectablePersonalInformation.Uid },
            MinimumLevelOfAssurance = LevelOfAssurance.High
        });

        // Act
        var result = await _sut.UpdateServiceAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            var serviceScopes = _dbContext.ServiceScopes.Select(d => d.Name).ToList();

            Assert.That(serviceScopes.OrderBy(s => s), Is.EqualTo(scopes.OrderBy(s => s)));
        });
    }

    [Test]
    [TestCaseSource(nameof(GetScopesByProviderDetailsIdInvalidDataTestCases))]
    public async Task GetScopesByProviderId_WhenCallWithInvalidData_ShouldReturnBadRequestAsync(GetAvailableScopesByProviderId message, string caseName)
    {
        // Arrange
        // Act
        var result = await _sut.GetAvailableScopesByProviderIdAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task GetScopesByProviderId_WhenCallWithValidData_ShouldReturnOkRequestAsync()
    {
        // Arrange
        var (providerDetails, service) = await AddProviderDetailsAndServiceTaskAsync();

        var scopes = new List<string> { "1", "2" };

        var serviceScope1 = new ServiceScope
        {
            Id = Guid.NewGuid(),
            Name = scopes[0],
            ProviderService = service
        };
        await _dbContext.ServiceScopes.AddAsync(serviceScope1);
        var serviceScope2 = new ServiceScope
        {
            Id = Guid.NewGuid(),
            Name = scopes[1],
            ProviderService = service
        };
        await _dbContext.ServiceScopes.AddAsync(serviceScope2);

        await _dbContext.SaveChangesAsync();

        var message = CreateInterface<GetAvailableScopesByProviderId>(new
        {
            CorrelationId = Guid.NewGuid(),
            ProviderId = providerDetails.Id,
        });

        // Act
        var result = await _sut.GetAvailableScopesByProviderIdAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            var serviceScopes = _dbContext.ServiceScopes
                .ToArray()
                .Where(ss => ss.ServiceId == service.Id)
                .Select(d => d.Name)
                .ToList();

            Assert.That(serviceScopes.OrderBy(s => s), Is.EqualTo(scopes.OrderBy(s => s)));
        });
    }

    private async Task<(Provider, ProviderService)> AddProviderDetailsAndServiceTaskAsync()
    {
        var providerDetails = new ProviderDetails
        {
            Id = Guid.NewGuid(),
            IdentificationNumber = Guid.NewGuid().ToString(),
            Name = "Provider Details Name",
            Status = ProviderDetailsStatus.Active,
            IsDeleted = false,
            SyncedFromOnlineRegistry = true,
            Address = "Address",
            Headquarters = "Headquarters address"
        };

        var section = new ProviderSection
        {
            Id = Guid.NewGuid(),
            ProviderDetailsId = providerDetails.Id,
            ProviderDetails = providerDetails,
            Name = "Section Name",
            IsDeleted = false,
            SyncedFromOnlineRegistry = true
        };

        var service = new ProviderService
        {
            Id = Guid.NewGuid(),
            ServiceNumber = 1234567,
            Name = "Service Name",
            IsEmpowerment = true,
            SyncedFromOnlineRegistry = true,
            IsDeleted = false,
            ProviderDetails = providerDetails,
            ProviderSection = section,
            Status = ProviderServiceStatus.Approved,
            CreatedOn = DateTime.UtcNow
        };
        await _dbContext.ProvidersDetailsServices.AddAsync(service);

        await _dbContext.SaveChangesAsync();
        var provider = new Provider
        {
            Id = Guid.NewGuid(),
            Number = "ПДЕАУ00001/16.01.2025",
            ExternalNumber = "123456",
            Name = "Test Provider",
            Bulstat = "Bulstat",
            Headquarters = "HQ",
            Address = "Address",
            Email = "email",
            Phone = "phone",
            IssuerName = "Test Issuer",
            IssuerUid = "8888888888",
            IssuerUidType = IdentifierType.EGN,
            Details = providerDetails,
            DetailsId = providerDetails.Id,
            GeneralInformation = "GeneralInformation",
            XMLRepresentation = "XMLRepresentation",
        };
        var office = new ProviderOffice { Id = Guid.NewGuid(), Name = "Central", Address = "Middle", Lat = 69, Lon = 420, Provider = provider, ProviderId = provider.Id };
        await _dbContext.ProvidersOffices.AddAsync(office);
        await _dbContext.Providers.AddAsync(provider);
        await _dbContext.SaveChangesAsync();

        return (provider, service);
    }

    private static readonly object[] UpdateServiceInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "1", "2" }
            }),
            "No CorrelationId"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.Empty,
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "1", "2" }
            }),
            "Empty CorrelationId"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                //ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "1", "2" }
            }),
            "No ProviderDetailsId"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.Empty,
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "1", "2" }
            }),
            "Empty ProviderDetailsId"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                //ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "1", "2" }
            }),
            "No ServiceId"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.Empty,
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "1", "2" }
            }),
            "Empty ServiceId"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                //ServiceScopeNames = null
            }),
            "No ServiceScopeNames"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] {}
            }),
            "Empty ServiceScopeNames"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "", "2" }
            }),
            "Empty field in ServiceScopeNames"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "", "", "" }
            }),
            "All fields are Empty in ServiceScopeNames"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { "111", "222", "123", "123" }
            }),
            "Fields with the same name in ServiceScopeNames"
        },
        new object[]
        {
            CreateInterface<UpdateService>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderDetailsId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                IsEmpowerment = true,
                ServiceScopeNames = new string[] { new(Enumerable.Repeat('1', Contracts.Constants.ServiceScopeNameMaxLength + 1).ToArray()), "222", "333"}
            }),
            "ServiceScopeName is longer than expected"
        },
    };

    private static readonly object[] GetScopesByProviderDetailsIdInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<GetAvailableScopesByProviderId>(new
            {
                //CorrelationId = Guid.NewGuid(),
                ProviderId = Guid.NewGuid()
            }),
            "No CorrelationId"
        },
        new object[]
        {
            CreateInterface<GetAvailableScopesByProviderId>(new
            {
                CorrelationId = Guid.Empty,
                ProviderId = Guid.NewGuid()
            }),
            "CorrelationId is empty"
        },
        new object[]
        {
            CreateInterface<GetAvailableScopesByProviderId>(new
            {
                CorrelationId = Guid.NewGuid(),
                //ProviderId = Guid.NewGuid()
            }),
            "No ProviderId"
        },
        new object[]
        {
            CreateInterface<GetAvailableScopesByProviderId>(new
            {
                CorrelationId = Guid.NewGuid(),
                ProviderId = Guid.Empty
            }),
            "ProviderId is empty"
        },
    };
}
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
