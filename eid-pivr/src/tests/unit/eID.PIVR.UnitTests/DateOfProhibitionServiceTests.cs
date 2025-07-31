using System.Net;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Enums;
using eID.PIVR.Service;
using eID.PIVR.Service.Database;
using eID.PIVR.Service.Entities;
using eID.PIVR.Service.Options;
using eID.PIVR.UnitTests.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace eID.PIVR.UnitTests;

public class DateOfProhibitionServiceTests : BaseTest
{
    private ILogger<DateOfProhibitionService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private DateOfProhibitionService _sut;

    private const string _testPersonalId1 = "8201019050";
    private const string _testPersonalId2 = "9001019050";
    private const string _testPFN = "1234123455";
    private DateTime _yesterday = DateTime.Now.AddDays(-1);

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<DateOfProhibitionService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        var cacheOptions = Options.Create(new ExternalRegistersCacheOptions());

        _dbContext = GetTestDbContext();
        _sut = new DateOfProhibitionService(_logger, _cache, _dbContext, cacheOptions);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void GetByPersonalIdAsync_CalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByPersonalIdAsync(null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithNullPersonalId_ReturnsBadRequestAsync()
    {
        // Arrange
        var getDateOfProhibition = CreateInterface<GetDateOfProhibition>(new
        {
            CorrelationId = Guid.NewGuid(),
        });

        // Act
        var result = await _sut.GetByPersonalIdAsync(getDateOfProhibition);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithEmptyPersonalId_ReturnsBadRequestAsync()
    {
        // Arrange
        var getDateOfProhibition = CreateInterface<GetDateOfProhibition>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = ""
        });

        // Act
        var result = await _sut.GetByPersonalIdAsync(getDateOfProhibition);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithNonExistingPersonalId_ShouldReturnResultWithNullAsync()
    {
        // Arrange
        await SeedTestDateOfProhibitionAsync();

        var getDateOfProhibition = CreateInterface<GetDateOfProhibition>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = "1122334455",
            UidType = UidType.EGN
        });

        // Act
        var serviceResult = await _sut.GetByPersonalIdAsync(getDateOfProhibition);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Date, Is.Null);
        Assert.That(result.TypeOfProhibition, Is.EqualTo(ProhibitionType.None));
        Assert.That(result.DescriptionOfProhibition, Is.EqualTo(""));
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithExistingPersonalIdAndNullDateInDb_ShouldReturnResultWithNullAsync()
    {
        // Arrange
        await SeedTestDateOfProhibitionAsync();

        var getDateOfProhibition = CreateInterface<GetDateOfProhibition>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = _testPersonalId2,
            UidType = UidType.LNCh
        });

        // Act
        var serviceResult = await _sut.GetByPersonalIdAsync(getDateOfProhibition);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Date, Is.Null);
        Assert.That(result.TypeOfProhibition, Is.EqualTo(ProhibitionType.Partial));
        Assert.That(result.DescriptionOfProhibition, Is.EqualTo("TestTest"));
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithExistingPersonalId_ShouldReturnLatestResultDateAsync()
    {
        // Arrange
        await SeedTestDateOfProhibitionAsync();

        var getDateOfProhibition = CreateInterface<GetDateOfProhibition>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = _testPersonalId1,
            UidType = UidType.EGN
        });

        // Act
        var serviceResult = await _sut.GetByPersonalIdAsync(getDateOfProhibition);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Date, Is.Not.Null);
        Assert.That(result.Date, Is.EqualTo(_yesterday));
        Assert.That(result.TypeOfProhibition, Is.EqualTo(ProhibitionType.Full));
        Assert.That(result.DescriptionOfProhibition, Is.EqualTo("Test"));
    }

    private async Task SeedTestDateOfProhibitionAsync()
    {
        var dbDatesOfProhibition = new List<DateOfProhibition>
        {
            new DateOfProhibition
            {
                Id = 1,
                PersonalId = _testPersonalId1,
                Date = _yesterday,
                CreatedOn = DateTime.Today,
                UidType = Contracts.Enums.UidType.EGN,
                TypeOfProhibition = Contracts.Enums.ProhibitionType.Full,
                DescriptionOfProhibition = "Test"
            },
            new DateOfProhibition
            {
                Id = 2,
                PersonalId = _testPersonalId1,
                Date = _yesterday.AddDays(-2),
                CreatedOn = DateTime.Today.AddDays(-1),
                UidType = Contracts.Enums.UidType.EGN,
                TypeOfProhibition = Contracts.Enums.ProhibitionType.Full,
                DescriptionOfProhibition = "Test"
            },
            new DateOfProhibition
            {
                Id = 3,
                PersonalId = _testPersonalId2,
                Date = null,
                CreatedOn = DateTime.Today,
                UidType = Contracts.Enums.UidType.LNCh,
                TypeOfProhibition = Contracts.Enums.ProhibitionType.Partial,
                DescriptionOfProhibition = "TestTest"
            },
            new DateOfProhibition
            {
                Id = 4,
                PersonalId = _testPFN,
                Date = _yesterday,
                CreatedOn = DateTime.Today
            },
        };

        await _dbContext.DatesOfProhibition.AddRangeAsync(dbDatesOfProhibition);
        await _dbContext.SaveChangesAsync();
    }
}
