using System.Net;
using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.UnitTests.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace eID.PAN.UnitTests;

[TestFixture]
public class UserNotificationChannelsServiceTests : BaseTest
{
    private ILogger<UserNotificationChannelsService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private UserNotificationChannelsService _sut;

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<UserNotificationChannelsService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();
        _sut = new UserNotificationChannelsService(_logger, _cache, _dbContext);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void GetByFilterAsync_WhenCalledWithNullFilter_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByFilterAsync(null));
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithInvalidFilterAsync_ReturnsBadRequest()
    {
        // Arrange
        var filter = CreateInterface<GetUserNotificationChannelsByFilter>(new
        {
            PageIndex = 0,
            PageSize = 101,
            ChannelName = "M"
        });

        // Act
        var result = await _sut.GetByFilterAsync(filter);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithInvalidMessage_ShouldReturnBadRequestServiceResultAsync()
    {
        // Arrange
        var filter = CreateInterface<GetUserNotificationChannelsByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            PageIndex = 0,
            PageSize = 101
        });
        var expectedErrors = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("PageIndex", "'Page Index' must be greater than or equal to '1'."),
            new KeyValuePair<string, string>("PageSize", "'Page Size' must be less than or equal to '100'.")
        };

        // Act
        var result = await _sut.GetByFilterAsync(filter);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
        CollectionAssert.AreEqual(expectedErrors, result.Errors);
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithValidMessage_ShouldReturnOkAndEmptyServiceResultAsync()
    {
        // Arrange
        var filter = CreateInterface<GetUserNotificationChannelsByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            PageIndex = 1,
            PageSize = 100,
            ChannelName = "M"
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(filter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(1));
        Assert.That(result.TotalItems, Is.EqualTo(0));
        CollectionAssert.IsEmpty(result.Data);
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithValidData_ReturnsPaginatedResultAsync()
    {
        // Arrange
        var registeredSystemId = Guid.NewGuid();
        _dbContext.RegisteredSystems.Add(new RegisteredSystem { Id = registeredSystemId, IsApproved = true });
        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel1", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel2", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel3", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = registeredSystemId, Name = "Channel4", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = registeredSystemId, Name = "Channel5", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "Channel 6", CallbackUrl = "", Description = "", InfoUrl = "" }, // This should be skipped because system isn't approved
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "7", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "8", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "9", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "10", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "11", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = Guid.NewGuid(), Name = "12", CallbackUrl = "", Description = "", InfoUrl = "" }
        };
        _dbContext.NotificationChannels.AddRange(notificationChannels);
        _dbContext.SaveChanges();

        var filter = CreateInterface<GetUserNotificationChannelsByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            PageIndex = 2,
            PageSize = 3,
            ChannelName = "Channel"
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(filter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(2));
        Assert.That(result.TotalItems, Is.EqualTo(5));
        Assert.That(result.Data, Is.EquivalentTo(notificationChannels
            .Where(nc => nc.IsBuiltIn || nc.SystemId == registeredSystemId)
            .Where(rs => rs.Name.Contains(filter.ChannelName))
            .Skip(3)));
    }

    [Test]
    public void GetSelectedAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetSelectedAsync(null));
    }

    [Test]
    public async Task GetSelectedAsync_ValidMessage_ReturnsPaginatedDataAsync()
    {
        // Arrange
        var UserId = Guid.NewGuid();
        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel1", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel2", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel3", CallbackUrl = "", Description = "", InfoUrl = "" }
        };
        _dbContext.NotificationChannels.AddRange(notificationChannels);
        _dbContext.SaveChanges();

        var data = new List<UserNotificationChannel>
            {
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = UserId, NotificationChannelId = notificationChannels[0].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = UserId, NotificationChannelId = notificationChannels[0].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = UserId, NotificationChannelId = notificationChannels[0].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = UserId, NotificationChannelId = notificationChannels[0].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = UserId, NotificationChannelId = notificationChannels[1].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), NotificationChannelId = notificationChannels[1].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), NotificationChannelId = notificationChannels[1].Id },
            };

        _dbContext.UserNotificationChannels.AddRange(data);
        _dbContext.SaveChanges();

        // Act
        var message = CreateInterface<GetUserNotificationChannels>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId,
            PageIndex = 2,
            PageSize = 3
        });
        var serviceResult = await _sut.GetSelectedAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(2));
        Assert.That(result.TotalItems, Is.EqualTo(5));
        var dataShould = data.Where(d => d.UserId == message.UserId)
            .Select(d => d.NotificationChannelId).OrderBy(d => d).Skip(3).Take(2);
        CollectionAssert.AreEquivalent(result.Data, dataShould);
    }

    [Test]
    public void RegisterSelectedAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RegisterSelectedAsync(null));
    }

    [Test]
    public async Task RegisterSelectedAsync_EmptyIds_DeletesAllUserRecordsAsync()
    {
        // Arrange
        var registeredSystemId = Guid.NewGuid();
        _dbContext.RegisteredSystems.Add(new RegisteredSystem { Id = registeredSystemId, IsApproved = true });
        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.SMTP,
                Description = "Вътрешен канал за изпращане на имейли",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/"
            },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel1", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel2", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel3", CallbackUrl = "", Description = "", InfoUrl = "" }
        };
        _dbContext.NotificationChannels.AddRange(notificationChannels);
        _dbContext.SaveChanges();

        var userId2 = Guid.NewGuid();
        var message = CreateInterface<RegisterUserNotificationChannels>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Ids = new HashSet<Guid>(),
            ModifiedBy = "Ivan Ivanov"
        });

        var existingData = new List<UserNotificationChannel>
            {
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = message.UserId, NotificationChannelId = notificationChannels[0].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = message.UserId, NotificationChannelId = notificationChannels[0].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = userId2, NotificationChannelId = notificationChannels[1].Id },
                new UserNotificationChannel { Id = Guid.NewGuid(), UserId = userId2, NotificationChannelId = notificationChannels[1].Id },
            };

        _dbContext.UserNotificationChannels.AddRange(existingData);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterSelectedAsync(message);

        var currentData = _dbContext.UserNotificationChannels.ToList();

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);
        Assert.That(currentData, Has.Count.EqualTo(4));
    }

    [Test]
    public async Task RegisterSelectedAsync_MissingIds_ReturnsBadRequestAsync()
    {
        // Arrange
        var message = CreateInterface<RegisterUserNotificationChannels>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Ids = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            ModifiedBy = "Ivan Ivanov"
        });
        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.SMTP,
                Description = "Вътрешен канал за изпращане на имейли",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/"
            },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel1", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel2", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = Guid.NewGuid(), IsBuiltIn = true, Name = "Channel3", CallbackUrl = "", Description = "", InfoUrl = "" }
        };
        _dbContext.NotificationChannels.AddRange(notificationChannels);
        _dbContext.SaveChanges();


        var existingData = new List<UserNotificationChannel>
        {
            new UserNotificationChannel { Id = Guid.NewGuid(), UserId = message.UserId, NotificationChannelId = notificationChannels[0].Id },
            new UserNotificationChannel { Id = Guid.NewGuid(), UserId = message.UserId, NotificationChannelId = notificationChannels[0].Id }
        };
        _dbContext.UserNotificationChannels.AddRange(existingData);

        // Act
        var serviceResult = await _sut.RegisterSelectedAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
        Assert.That(serviceResult.Errors[0].Value, Does.Contain(string.Join(',', message.Ids)));
    }

    [Test]
    public async Task RegisterSelectedAsync_AddInEmptySet_ReturnsNoContentAsync()
    {
        // Arrange
        var se1 = Guid.NewGuid();
        var se2 = Guid.NewGuid();
        var message = CreateInterface<RegisterUserNotificationChannels>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ModifiedBy = "Ivan Ivanov",
            Ids = new HashSet<Guid> { se1, se2 }
        });

        var existingData = new List<UserNotificationChannel>
        {
        };
        _dbContext.UserNotificationChannels.AddRange(existingData);

        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.SMTP,
                Description = "Вътрешен канал за изпращане на имейли",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/"
            },
            new NotificationChannelApproved { Id = se1, Name = "1", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = se2, Name = "2", CallbackUrl = "", Description = "", InfoUrl = "" }
        };
        _dbContext.NotificationChannels.AddRange(notificationChannels);

        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterSelectedAsync(message);

        // Assert
        var currentData = _dbContext.UserNotificationChannels.ToList();
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);
        Assert.That(currentData, Has.Count.EqualTo(3));
    }

    [Test]
    public async Task RegisterSelectedAsync_AddInExistingSet_ReturnsNoContentAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existNC = Guid.NewGuid();
        var newNC = Guid.NewGuid();
        var deleteNC = Guid.NewGuid();

        var message = CreateInterface<RegisterUserNotificationChannels>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = userId,
            Ids = new HashSet<Guid> { existNC, newNC },
            ModifiedBy = "Ivan Ivanov"
        });

        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved
            {
                Id = Guid.NewGuid(),
                IsBuiltIn = true,
                Name = ConfigurationsConstants.SMTP,
                Description = "Вътрешен канал за изпращане на имейли",
                CallbackUrl = "http://localhost/",
                Price = 0,
                InfoUrl = "http://localhost/"
            },
            new NotificationChannelApproved { Id = existNC, Name = "1", CallbackUrl = "", Description = "", InfoUrl = "" },
            new NotificationChannelApproved { Id = newNC, Name = "2", CallbackUrl = "", Description = "", InfoUrl = ""  },
            new NotificationChannelApproved { Id = deleteNC, Name = "3", CallbackUrl = "", Description = "", InfoUrl = ""  }
        };
        _dbContext.NotificationChannels.AddRange(notificationChannels);

        var existingData = new List<UserNotificationChannel>
        {
            new UserNotificationChannel { Id = Guid.NewGuid(), UserId = userId, NotificationChannelId = existNC},
            new UserNotificationChannel { Id = Guid.NewGuid(), UserId = userId, NotificationChannelId = deleteNC}
        };
        _dbContext.UserNotificationChannels.AddRange(existingData);
        await _dbContext.SaveChangesAsync();

        // Act
        var serviceResult = await _sut.RegisterSelectedAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);

        var currentData = _dbContext.UserNotificationChannels.ToList();
        Assert.That(currentData, Has.Count.EqualTo(3));
        Assert.That(currentData.Where(ee => ee.NotificationChannelId == newNC).Count(), Is.EqualTo(1));
        Assert.That(currentData.Where(ee => ee.NotificationChannelId == existNC).Count(), Is.EqualTo(1));
        Assert.That(currentData.Where(ee => ee.NotificationChannelId == deleteNC).Count(), Is.EqualTo(0));
    }
}
