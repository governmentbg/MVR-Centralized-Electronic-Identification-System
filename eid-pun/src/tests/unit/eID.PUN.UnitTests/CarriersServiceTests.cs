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

    private const string _testSerialNumber1 = "123-123";
    private const string _testSerialNumber2 = "test-serial-123";
    private Guid _testEId1 = Guid.NewGuid();
    private Guid _testEId2 = Guid.NewGuid();

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
        var registerCarrier = CreateInterface<RegisterCarrier>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = serialNumber,
            Type = type,
            CertificateId = certId,
            EId = eId,
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
        });
        _publishEndpoint.Verify(pe =>
            pe.Publish<NotifyEIds>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
    }

    #region GetCarriers
    [Test]
    public void GetByFilterAsync_CalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByFilterAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(GetCarriersByFilterInvalidDataTestCases))]
    public async Task GetByFilterAsync_WhenCallWithInvalidData_ShouldReturnBadRequestAsync(GetCarriersByFilter message, string caseName)
    {
        // Arrange
        // Act
        var result = await _sut.GetByFilterAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task GetByKeyAsync_CalledWithNonExistingValues_ShouldReturnEmptyListAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = "fake-sn",
            EId = Guid.NewGuid()
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(getCarriersByFilter);

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
    public async Task GetByFilterAsync_CalledWithExistingSerialNumber_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = _testSerialNumber1
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(getCarriersByFilter);

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
    public async Task GetByFilterAsync_CalledWithExistingEId_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            EId = _testEId2
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(getCarriersByFilter);

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
    public async Task GetByFilterAsync_CalledWithExistingSerialNumberAndEId_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = _testSerialNumber1,
            EId = _testEId2
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(getCarriersByFilter);

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

    [Test]
    public async Task GetByFilterAsync_CalledWithExistingSerialNumberAndType_ShouldReturnOkAsync()
    {
        // Arrange
        await SeedTestCarriersAsync();

        var getCarriersByFilter = CreateInterface<GetCarriersByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            SerialNumber = _testSerialNumber1,
            EId = _testEId2,
            Type = "MobileApp"
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(getCarriersByFilter);

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
            },
            new Carrier
            {
                Id = Guid.NewGuid(),
                SerialNumber = _testSerialNumber1,
                Type = "MobileApp",
                CertificateId = Guid.NewGuid(),
                EId = _testEId2,
            },
            new Carrier
            {
                Id = Guid.NewGuid(),
                SerialNumber = _testSerialNumber2,
                Type = "BgIdCard",
                CertificateId = Guid.NewGuid(),
                EId = _testEId2,
            },
        };

        await _dbContext.Carriers.AddRangeAsync(dbCarriers);
        await _dbContext.SaveChangesAsync();
    }

    private static readonly object[] GetCarriersByFilterInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<GetCarriersByFilter>(new
            {
                //CorrelationId = Guid.NewGuid(),
                SerialNumber = "123-123",
                EId = Guid.NewGuid(),
                CertificateId = Guid.NewGuid(),
                Type = "Type",
            }),
            "No CorrelationId"
        },
        new object[]
        {
            CreateInterface<GetCarriersByFilter>(new
            {
                CorrelationId = Guid.Empty,
                SerialNumber = "123-123",
                EId = Guid.NewGuid(),
                CertificateId = Guid.NewGuid(),
                Type = "Type",
            }),
            "Empty CorrelationId"
        },
        new object[]
        {
            CreateInterface<GetCarriersByFilter>(new
            {
                CorrelationId = Guid.NewGuid(),
                //SerialNumber = "123-123",
                //EId = Guid.NewGuid(),
                //CertificateId = Guid.NewGuid(),
                //Type = "Type",
            }),
            "No Type"
        },
        new object[]
        {
            CreateInterface<GetCarriersByFilter>(new
            {
                CorrelationId = Guid.NewGuid(),
                //SerialNumber = "123-123",
                //EId = Guid.NewGuid(),
                //CertificateId = Guid.NewGuid(),
                Type = string.Empty,
            }),
            "Empty Type"
        }
    };
}
