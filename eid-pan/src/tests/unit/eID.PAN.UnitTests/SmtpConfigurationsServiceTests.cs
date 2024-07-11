using System.Net;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Enums;
using eID.PAN.Service;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Options;
using eID.PAN.UnitTests.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace eID.PAN.UnitTests;

public class SmtpConfigurationsServiceTests : BaseTest
{
    private ILogger<SmtpConfigurationsService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private SmtpConfigurationsService _sut;

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<SmtpConfigurationsService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        var aesOptionsMock = new Mock<IOptions<AesOptions>>();
        aesOptionsMock.Setup(x => x.Value).Returns(() => new AesOptions { Key = "MyTestkey1234567", Vector = "MyTestVector1234" });

        _dbContext = GetTestDbContext();
        _sut = new SmtpConfigurationsService(_logger, _cache, _dbContext, aesOptionsMock.Object);
    }

    [Test]
    public void GetByIdAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByIdAsync(null));
    }

    [Test]
    public async Task GetByIdAsync_WhenCalledWithEmptyKey_ReturnsBadRequestAsync()
    {
        // Arrange
        var getSmtpConfigurationById = CreateInterface<GetSmtpConfigurationById>(new
        {
            Id = Guid.Empty
        });

        // Act
        var result = await _sut.GetByIdAsync(getSmtpConfigurationById);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByIdAsync_WhenCalledWithNonExistingKey_ShouldReturnNotFoundAsync()
    {
        var getSmtpConfigurationById = CreateInterface<GetSmtpConfigurationById>(new
        {
            Id = Guid.NewGuid(),
        });

        // Act
        var result = await _sut.GetByIdAsync(getSmtpConfigurationById);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetByKeyAsync_WhenCalledWithExistingKey_ShouldReturnOkAsync()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        await SeedTestSmtpConfigurationsAsync(id1, id2, id3);

        var getSmtpConfigurationById = CreateInterface<GetSmtpConfigurationById>(new
        {
            Id = id1
        });

        // Act
        var result = await _sut.GetByIdAsync(getSmtpConfigurationById);

        // Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(id1, Is.EqualTo(result?.Result?.Id));
            Assert.That(_cache.Get($"eID:PAN:SmtpConfiguration:{id1}"), Is.Not.Null);
        });
    }

    [Test]
    public void UpdateAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateAsync(null));
    }

    [Test]
    public async Task UpdateAsync_WhenCalledWithEmptyUserId_ReturnsBadRequestAsync()
    {
        // Arrange
        var updateSmtpConfiguration = CreateInterface<UpdateSmtpConfiguration>(new
        {
            Id = Guid.NewGuid(),
            Server = "UpdatedServer",
            Port = 1234,
            SecurityProtocol = SmtpSecurityProtocol.SSL,
            UserName = "UpdatedUserName",
            Password = "UpdatedPassword",
            UserId = string.Empty
        });

        // Act
        var result = await _sut.UpdateAsync(updateSmtpConfiguration);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateAsync_WhenCalledWithNegativePort_ReturnsBadRequestAsync()
    {
        // Arrange
        var updateSmtpConfiguration = CreateInterface<UpdateSmtpConfiguration>(new
        {
            Id = Guid.NewGuid(),
            Server = "UpdatedServer",
            Port = -1234,
            SecurityProtocol = SmtpSecurityProtocol.SSL,
            UserName = "UpdatedUserName",
            Password = "UpdatedPassword",
            UserId = "UpdatedUser"
        });

        // Act
        var result = await _sut.UpdateAsync(updateSmtpConfiguration);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateAsync_WhenCalledWithNonExistingKey_ShouldReturnNotFoundAsync()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        await SeedTestSmtpConfigurationsAsync(id1, id2, id3);

        var updateSmtpConfiguration = CreateInterface<UpdateSmtpConfiguration>(new
        {
            Id = Guid.NewGuid(),
            Server = "UpdatedServer",
            Port = 1234,
            SecurityProtocol = SmtpSecurityProtocol.SSL,
            UserName = "UpdatedUserName",
            Password = "UpdatedPassword",
            UserId = "UpdatedUser"
        });

        // Act
        var result = await _sut.UpdateAsync(updateSmtpConfiguration);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateAsync_WhenCalledWithValidData_ShouldReturnNoContentAndClearCacheAsync()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();

        await SeedTestSmtpConfigurationsAsync(id1, id2, id3);

        var updateSmtpConfiguration = CreateInterface<UpdateSmtpConfiguration>(new
        {
            Id = id1,
            Server = "UpdatedServer",
            Port = 1234,
            SecurityProtocol = SmtpSecurityProtocol.SSL,
            UserName = "UpdatedUserName",
            Password = "UpdatedPassword",
            UserId = "UpdatedUser"
        });

        var getSmtpConfigurationById = CreateInterface<GetSmtpConfigurationById>(new
        {
            Id = id1
        });

        // Getting the record by Id in order to populate the cache
        await _sut.GetByIdAsync(getSmtpConfigurationById);

        // Act
        var result = await _sut.UpdateAsync(updateSmtpConfiguration);
        Assert.That(_cache.Get($"eID:PAN:SmtpConfiguration:{id1}"), Is.Null);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NoContent);

        var resultAfterUpdate = await _sut.GetByIdAsync(getSmtpConfigurationById);
        CheckServiceResult(resultAfterUpdate, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(id1, Is.EqualTo(resultAfterUpdate?.Result?.Id));
            Assert.That(updateSmtpConfiguration.Server, Is.EqualTo(resultAfterUpdate?.Result?.Server));
            Assert.That(updateSmtpConfiguration.Port, Is.EqualTo(resultAfterUpdate?.Result?.Port));
            Assert.That(updateSmtpConfiguration.UserName, Is.EqualTo(resultAfterUpdate?.Result?.UserName));
        });
    }

    private async Task SeedTestSmtpConfigurationsAsync(params Guid[] ids)
    {
        var smtpConfigurations = new List<SmtpConfiguration>();

        var counter = 1;
        foreach (var id in ids)
        {
            smtpConfigurations.Add(new SmtpConfiguration
            {
                Id = id,
                Server = $"TestServer{counter}",
                Port = counter,
                UserName = $"TestUserName{counter}",
                Password = $"TestPassword{counter}",
                CreatedOn = DateTime.UtcNow,
                CreatedBy = $"TestCreatedBy{counter}",
                ModifiedOn = null,
                ModifiedBy = null
            });

            counter++;
        }

        await _dbContext.SmtpConfigurations.AddRangeAsync(smtpConfigurations);
        await _dbContext.SaveChangesAsync();
    }
}
