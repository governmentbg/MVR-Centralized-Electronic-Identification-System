using System.Net;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Service;
using eID.PIVR.Service.Database;
using eID.PIVR.Service.Entities;
using eID.PIVR.Service.Options;
using eID.PIVR.UnitTests.Generic;
using MassTransit.Internals.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace eID.PIVR.UnitTests;

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
public class DateOfDeathServiceTests : BaseTest
{
    private ILogger<DateOfDeathService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private DateOfDeathService _sut;

    private const string DuplicatePersonalId = "8201019050";
    private const string TestPersonalId2 = "9001019050";
    private const string TestPFN = "1234123455";
    private DateTime _today;
    private DateTime _yesterday;

    [SetUp]
    public void Init()
    {
        _today = DateTime.Now;
        _yesterday = _today.AddDays(-1);

        _logger = new NullLogger<DateOfDeathService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        var cacheOptions = Options.Create(new ExternalRegistersCacheOptions());

        _dbContext = GetTestDbContext();
        _sut = new DateOfDeathService(_logger, _cache, _dbContext, cacheOptions);
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
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByPersonalIdAsync(null));
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithNullPersonalId_ReturnsBadRequestAsync()
    {
        // Arrange
        var getDateOfDeathByQuery = CreateInterface<GetDateOfDeath>(new
        {
            CorrelationId = Guid.NewGuid(),
        });

        // Act
        var result = await _sut.GetByPersonalIdAsync(getDateOfDeathByQuery);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithEmptyPersonalId_ReturnsBadRequestAsync()
    {
        // Arrange
        var getDateOfDeathByQuery = CreateInterface<GetDateOfDeath>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = ""
        });

        // Act
        var result = await _sut.GetByPersonalIdAsync(getDateOfDeathByQuery);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithNonExistingPersonalId_ShouldReturnResultWithNullAsync()
    {
        // Arrange
        await SeedTestDatesOfDeathAsync();

        var getDateOfDeathByQuery = CreateInterface<GetDateOfDeath>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = "1122334455",
            UidType = Contracts.Enums.UidType.EGN
        });

        // Act
        var serviceResult = await _sut.GetByPersonalIdAsync(getDateOfDeathByQuery);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Date, Is.Null);
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithExistingPersonalIdAndNullDateInDb_ShouldReturnResultWithNullAsync()
    {
        // Arrange
        await SeedTestDatesOfDeathAsync();

        var getDateOfDeathByQuery = CreateInterface<GetDateOfDeath>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = TestPersonalId2,
            UidType = Contracts.Enums.UidType.LNCh
        });

        // Act
        var serviceResult = await _sut.GetByPersonalIdAsync(getDateOfDeathByQuery);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Date, Is.Null);
    }

    [Test]
    public async Task GetByPersonalIdAsync_CalledWithExistingPersonalId_ShouldReturnLatestResultDateAsync()
    {
        // Arrange
        await SeedTestDatesOfDeathAsync();

        var getDateOfDeathByQuery = CreateInterface<GetDateOfDeath>(new
        {
            CorrelationId = Guid.NewGuid(),
            PersonalId = DuplicatePersonalId,
            UidType = Contracts.Enums.UidType.EGN
        });

        // Act
        var serviceResult = await _sut.GetByPersonalIdAsync(getDateOfDeathByQuery);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Date, Is.Not.Null);
        Assert.That(result.Date, Is.EqualTo(_yesterday));
    }

    [Test]
    public void GetDeceasedByPeriodAsync_WhenCalledWithNull_ThrowsArgumentNullException()
    {
        //Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetDeceasedByPeriodAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(_getDeceasedByPeriodCommandInvalidDataTestCases))]
    public async Task GetDeceasedByPeriodAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync(GetDeceasedByPeriod command, string caseName)
    {
        // Act
        var result = await _sut.GetDeceasedByPeriodAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task GetDeceasedByPeriodAsync_WhenCalledWithValidData_ShouldReturnOkWithEmptyDataAsync()
    {
        // Arrange
        var data = await SeedTestDatesOfDeathAsync();

        var minCreatedOn = data.Min(d => d.CreatedOn);

        var command = new GetDeceasedByPeriodCommand
        {
            CorrelationId = Guid.NewGuid(),
            From = minCreatedOn.AddDays(-2),
            To = minCreatedOn.AddDays(-1)
        };

        // Act
        var result = await _sut.GetDeceasedByPeriodAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var dataResult = result.Result;
        Assert.That(dataResult, Is.Empty);
    }

    [Test]
    public async Task GetDeceasedByPeriodAsync_WhenCalledWithValidData_ShouldReturnOkWithOneLastResultAsync()
    {
        // Arrange
        var data = await SeedTestDatesOfDeathAsync();

        var minCreatedOn = data.Min(d => d.CreatedOn);

        var command = new GetDeceasedByPeriodCommand
        {
            CorrelationId = Guid.NewGuid(),
            From = minCreatedOn.AddDays(-1),
            To = minCreatedOn
        };

        // Act
        var result = await _sut.GetDeceasedByPeriodAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var dataResult = result.Result;
        Assert.That(dataResult, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(dataResult.Count(), Is.EqualTo(1));
            Assert.That(dataResult.First(), Is.EqualTo(data.Last()));
        });
    }

    [Test]
    public async Task GetDeceasedByPeriodAsync_WhenCalledWithValidData_ShouldReturnOkWithLastOfTwoDuplicatesLastResultsAsync()
    {
        // Arrange
        var data = await SeedTestDatesOfDeathAsync();

        var maxCreatedOn = data.Max(d => d.CreatedOn);

        var command = new GetDeceasedByPeriodCommand
        {
            CorrelationId = Guid.NewGuid(),
            From = maxCreatedOn.AddDays(-1),
            To = maxCreatedOn
        };

        // Act
        var result = await _sut.GetDeceasedByPeriodAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var dataResult = result.Result;
        Assert.That(dataResult, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(dataResult.Count(), Is.EqualTo(1));
            Assert.That(dataResult.First(), Is.EqualTo(data[0]));
        });
    }

    [Test]
    public async Task GetDeceasedByPeriodAsync_WhenCalledWithValidData_ShouldReturnOkWithAllResultsAsync()
    {
        // Arrange
        var data = await SeedTestDatesOfDeathAsync();
        var dataWithoutNull = data.Where(d => d.Date != null).DistinctBy(d => d.PersonalId);

        var minCreatedOn = data.Min(d => d.CreatedOn);
        var maxCreatedOn = data.Max(d => d.CreatedOn);

        var command = new GetDeceasedByPeriodCommand
        {
            CorrelationId = Guid.NewGuid(),
            From = minCreatedOn,
            To = maxCreatedOn
        };

        // Act
        var result = await _sut.GetDeceasedByPeriodAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var dataResult = result.Result;
        Assert.That(dataResult, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(dataResult.Count(), Is.EqualTo(dataWithoutNull.Count()));
            Assert.That(dataResult, Is.EqualTo(dataWithoutNull));
        });
    }

    private static readonly object[] _getDeceasedByPeriodCommandInvalidDataTestCases =
    {
        new object[]
        {
            new GetDeceasedByPeriodCommand
            {
                CorrelationId = Guid.Empty,
            },
            "CorrelationId is Empty"
        },
        new object[]
        {
            new GetDeceasedByPeriodCommand
            {
                CorrelationId = Guid.NewGuid(),
                From = default
            },
            "From is default"
        },
        new object[]
        {
            new GetDeceasedByPeriodCommand
            {
                CorrelationId = Guid.NewGuid(),
                To = default
            },
            "To is default"
        },
        new object[]
        {
            new GetDeceasedByPeriodCommand
            {
                CorrelationId = Guid.NewGuid(),
                From = DateTime.UtcNow,
                To = default
            },
            "To is default"
        },
        new object[]
        {
            new GetDeceasedByPeriodCommand
            {
                CorrelationId = Guid.NewGuid(),
                From = default,
                To = DateTime.UtcNow
            },
            "From is default"
        },
        new object[]
        {
            new GetDeceasedByPeriodCommand
            {
                CorrelationId = Guid.NewGuid(),
                From = DateTime.UtcNow.AddMinutes(1),
                To = DateTime.UtcNow
            },
            "From is greater than To"
        }
    };

    private async Task<List<DateOfDeath>> SeedTestDatesOfDeathAsync()
    {
        var dbDatesOfDeath = new List<DateOfDeath>
        {
            new DateOfDeath
            {
                Id = 1,
                PersonalId = DuplicatePersonalId,
                Date = _yesterday,
                CreatedOn = _today,
                UidType = Contracts.Enums.UidType.EGN
            },
            new DateOfDeath
            {
                Id = 2,
                PersonalId = DuplicatePersonalId,
                Date = _yesterday.AddDays(-2),
                CreatedOn = _yesterday,
                UidType = Contracts.Enums.UidType.EGN
            },
            new DateOfDeath
            {
                Id = 3,
                PersonalId = TestPersonalId2,
                Date = null,
                CreatedOn = _yesterday.AddDays(-1),
                UidType = Contracts.Enums.UidType.LNCh
            },
            new DateOfDeath
            {
                Id = 4,
                PersonalId = TestPFN,
                Date = _yesterday,
                CreatedOn = _yesterday.AddDays(-2),
                UidType = Contracts.Enums.UidType.EGN
            }
        };

        await _dbContext.DatesOfDeath.AddRangeAsync(dbDatesOfDeath);
        await _dbContext.SaveChangesAsync();

        return dbDatesOfDeath;
    }

    private class GetDeceasedByPeriodCommand : GetDeceasedByPeriod
    {
        public Guid CorrelationId { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
