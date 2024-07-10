using System.Net;
using eID.PUN.Contracts.Commands;
using eID.PUN.Contracts.Results;
using eID.PUN.Service;
using eID.PUN.Service.Database;
using eID.PUN.Service.Entities;
using eID.PUN.UnitTests.Generic;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace eID.PUN.UnitTests;

public class CarriersServiceTests : BaseTest
{
    private ILogger<CarriersService> _logger;
    private ApplicationDbContext _dbContext;
    private CarriersService _sut;
    private Mock<IPublishEndpoint> _publishEndpoint;

    private const string _testUserName = "Test User";
    private const string _testSerialNumber1 = "123-123";
    private const string _testSerialNumber2 = "test-serial-123";
    private Guid _testEId1 = Guid.NewGuid();
    private Guid _testEId2 = Guid.NewGuid();
    private Guid _testUserId = Guid.NewGuid();

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<CarriersService>();
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _dbContext = GetTestDbContext();
        _sut = new CarriersService(_logger, _publishEndpoint.Object, _dbContext);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void RegisterAsync_CalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RegisterAsync(null));
    }

    [Test]
    public async Task RegisterAsync_WithValidData_StoresCorrectlyAndReturnsGuidAsync()
    {
        // Arrange
        var serialNumber = "111-222-333";
        var type = "BgIdCard";
        var certId = Guid.NewGuid();
        var eId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var registerCarrier = CreateInterface<RegisterCarrier>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = serialNumber,
            Type = type,
            CertificateId = certId,
            EId = eId,
            UserId = userId,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.RegisterAsync(registerCarrier);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));
        var dbRecord = _dbContext.Carriers.FirstOrDefault(c => c.Id == result);
        Assert.Multiple(() =>
        {
            Assert.That(dbRecord, Is.Not.Null);
            Assert.That(dbRecord?.SerialNumber, Is.EqualTo(serialNumber));
            Assert.That(dbRecord?.Type, Is.EqualTo(type));
            Assert.That(dbRecord?.CertificateId, Is.EqualTo(certId));
            Assert.That(dbRecord?.EId, Is.EqualTo(eId));
            Assert.That(dbRecord?.UserId, Is.EqualTo(userId));
            Assert.That(dbRecord?.ModifiedBy, Is.EqualTo(_testUserName));
        });
        _publishEndpoint.Verify(pe =>
            pe.Publish<NotifyEIds>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }

    #region GetCarriers
    [Test]
    public void GetByAsync_CalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByAsync(null));
    }

    [Test]
    public async Task GetByAsync_CalledWithEmptyFilter_ReturnsBadRequestAsync()
    {
        // Arrange
        var getCarriersByFilter = CreateInterface<GetCarriersBy>(new
        {
            CorrelationId = Guid.NewGuid()
        });

        // Act
        var result = await _sut.GetByAsync(getCarriersByFilter);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByKeyAsync_CalledWithNonExistingValues_ShouldReturnEmptyListAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersBy>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = "fake-sn",
            EId = Guid.NewGuid()
        });

        // Act
        var serviceResult = await _sut.GetByAsync(getCarriersByFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        CollectionAssert.IsEmpty(result);
        _publishEndpoint.Verify(pe =>
            pe.Publish<NotifyEIds>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GetByAsync_CalledWithExistingSerialNumber_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersBy>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = _testSerialNumber1
        });

        // Act
        var serviceResult = await _sut.GetByAsync(getCarriersByFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(c => c.SerialNumber), Is.All.EqualTo(_testSerialNumber1));
        _publishEndpoint.Verify(pe =>
            pe.Publish<NotifyEIds>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }

    [Test]
    public async Task GetByAsync_CalledWithExistingEId_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersBy>(new
        {
            CorrelationId = Guid.NewGuid(),
            EId = _testEId2
        });

        // Act
        var serviceResult = await _sut.GetByAsync(getCarriersByFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        Assert.That(result.Select(c => c.EId), Is.All.EqualTo(_testEId2));
        _publishEndpoint.Verify(pe =>
            pe.Publish<NotifyEIds>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }

    [Test]
    public async Task GetByAsync_CalledWithExistingSerialNumberAndEId_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersBy>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = _testSerialNumber1,
            EId = _testEId2
        });

        // Act
        var serviceResult = await _sut.GetByAsync(getCarriersByFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(1));
        Assert.That(result.Select(c => c.SerialNumber), Is.All.EqualTo(_testSerialNumber1));
        Assert.That(result.Select(c => c.EId), Is.All.EqualTo(_testEId2));
        _publishEndpoint.Verify(pe =>
            pe.Publish<NotifyEIds>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }
    #endregion

    private async Task SeedTestCarriersAsync()
    {
        var dbCarriers = new List<Carrier>
        {
            new Carrier
            {
                Id = Guid.NewGuid(),
                SerialNumber = _testSerialNumber1,
                Type = "BgIdCard",
                CertificateId = Guid.NewGuid(),
                EId = _testEId1,
                UserId = _testUserId,
                ModifiedBy = _testUserName,
            },
            new Carrier
            {
                Id = Guid.NewGuid(),
                SerialNumber = _testSerialNumber1,
                Type = "MobileApp",
                CertificateId = Guid.NewGuid(),
                EId = _testEId2,
                UserId = _testUserId,
                ModifiedBy = _testUserName,
            },
            new Carrier
            {
                Id = Guid.NewGuid(),
                SerialNumber = _testSerialNumber2,
                Type = "BgIdCard",
                CertificateId = Guid.NewGuid(),
                EId = _testEId2,
                UserId = _testUserId,
                ModifiedBy = _testUserName,
            },
        };

        await _dbContext.Carriers.AddRangeAsync(dbCarriers);
        await _dbContext.SaveChangesAsync();
    }
}
