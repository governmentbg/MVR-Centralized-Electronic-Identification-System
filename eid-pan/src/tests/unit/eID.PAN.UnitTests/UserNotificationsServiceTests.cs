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
using SystemEvent = eID.PAN.Service.Entities.SystemEvent;

namespace eID.PAN.UnitTests;

[TestFixture]
public class UserNotificationsServiceTests : BaseTest
{
    private ILogger<UserNotificationsService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private UserNotificationsService _sut;

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<UserNotificationsService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();
        _sut = new UserNotificationsService(_logger, _cache, _dbContext);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    #region Get from NotificationsServiceShould
    [Test]
    public void GetByFilterAsync_WhenCalledWithNullFilter_ThrowsArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByFilterAsync(null));
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithInvalidFilterAsync()
    {
        // Arrange
        var notificationFilter = CreateInterface<GetSystemsAndNotificationsByFilter>(new
        {
            PageIndex = 0,
            PageSize = 11,
            SystemName = "System"
        });

        // Act
        var result = await _sut.GetByFilterAsync(notificationFilter);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithInvalidMessage_ShouldReturnBadRequestServiceResultAsync()
    {
        // Arrange
        var notificationFilter = CreateInterface<GetSystemsAndNotificationsByFilter>(new
        {
            PageIndex = 0,
            PageSize = 51
        });
        var expectedErrors = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("PageIndex", "'Page Index' must be greater than or equal to '1'."),
            new KeyValuePair<string, string>("PageSize", "'Page Size' must be less than or equal to '50'.")
        };

        // Act
        var result = await _sut.GetByFilterAsync(notificationFilter);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
        CollectionAssert.AreEqual(expectedErrors, result.Errors);
    }

    [Test]
    public async Task GetByFilterAsync_WhenCalledWithValidMessage_ShouldReturnOkAndEmptyServiceResultAsync()
    {
        // Arrange
        var notificationFilter = CreateInterface<GetSystemsAndNotificationsByFilter>(new
        {
            PageIndex = 1,
            PageSize = 10,
            SystemName = "Test"
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(notificationFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(1));
        Assert.That(result.TotalItems, Is.EqualTo(0));
        CollectionAssert.IsEmpty(result.Data);
    }

    [Test]
    public async Task GetNotificationsByFilterAsync_WhenCalledWithValidData_ReturnsPaginatedNotificationsAsync()
    {
        // Arrange
        var registeredSystems = new List<RegisteredSystem>
        {
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System1", IsApproved = true, IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System2", IsApproved = true, IsDeleted = true },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System3", IsApproved = true, IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System4", IsApproved = true, IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System5", IsApproved = true, IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "6", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "7", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "8", IsDeleted = true },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "9", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "10", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "11", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "12", IsDeleted = true }
        };
        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        var notificationFilter = CreateInterface<GetSystemsAndNotificationsByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            PageIndex = 2,
            PageSize = 3,
            SystemName = "System"
        });

        // Act
        var serviceResult = await _sut.GetByFilterAsync(notificationFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(2));
        Assert.That(result.TotalItems, Is.EqualTo(4));
        Assert.That(result.Data, Is.EquivalentTo(registeredSystems
            .Where(rs => !rs.IsDeleted && rs.Name.Contains(notificationFilter.SystemName))
            .Skip(3)));
    }
    #endregion

    [Test]
    public void GetDeactivatedAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetDeactivatedAsync(null));
    }

    [Test]
    public async Task GetDeactivatedAsync_ValidMessage_ReturnsPaginatedDataAsync()
    {
        // Arrange
        var registeredSystemId = Guid.NewGuid();
        var registeredSystem = GetRegisteredSystems(registeredSystemId, "DemoSystem")[0];
        _dbContext.RegisteredSystems.Add(registeredSystem);
        _dbContext.SaveChanges();


        var message = CreateInterface<GetDeactivatedUserNotifications>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            PageIndex = 2,
            PageSize = 3
        });

        var data = new List<DeactivatedUserEvent>
            {
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id }
            };

        _dbContext.DeactivatedUserEvents.AddRange(data);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.GetDeactivatedAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(2));
        Assert.That(result.TotalItems, Is.EqualTo(5));
        var dataShould = data.Where(d => d.UserId == message.UserId)
            .Select(d => d.SystemEventId).OrderBy(d => d).Skip(3).Take(2);
        CollectionAssert.AreEquivalent(result.Data, dataShould);
    }

    [Test]
    public void RegisterDeactivatedEventsAsync_NullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RegisterDeactivatedEventsAsync(null));
    }

    [Test]
    public async Task RegisterDeactivatedEventsAsync_EmptyIds_DeletesAllUserRecordsAsync()
    {
        // Arrange
        var userId2 = Guid.NewGuid();
        var registeredSystemId = Guid.NewGuid();
        var registeredSystem = GetRegisteredSystems(registeredSystemId, "DemoSystem")[0];
        _dbContext.RegisteredSystems.Add(registeredSystem);
        _dbContext.SaveChanges();
        var message = CreateInterface<RegisterDeactivatedEvents>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ModifiedBy = "Ivan Ivanov",
            Ids = new HashSet<Guid>()
        });

        var existingEvents = new List<DeactivatedUserEvent>
            {
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = userId2, SystemEventId = registeredSystem.Events.FirstOrDefault().Id },
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = userId2, SystemEventId = registeredSystem.Events.FirstOrDefault().Id }
            };

        _dbContext.DeactivatedUserEvents.AddRange(existingEvents);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterDeactivatedEventsAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);
        var currentEvents = _dbContext.DeactivatedUserEvents.ToList();
        Assert.That(currentEvents, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task RegisterDeactivatedEventsAsync_MissingIds_ReturnsBadRequestAsync()
    {
        // Arrange
        var registeredSystemId = Guid.NewGuid();
        var registeredSystem = GetRegisteredSystems(registeredSystemId, "DemoSystem")[0];
        _dbContext.RegisteredSystems.Add(registeredSystem);
        _dbContext.SaveChanges();
        var message = CreateInterface<RegisterDeactivatedEvents>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ModifiedBy = "Ivan Ivanov",
            Ids = new HashSet<Guid> { Guid.NewGuid(), Guid.NewGuid() }
        });

        var existingEvents = new List<DeactivatedUserEvent>
            {
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId , SystemEventId = registeredSystem.Events.FirstOrDefault().Id},
                new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = message.UserId, SystemEventId = registeredSystem.Events.FirstOrDefault().Id }
            };

        var systemEvents = new List<SystemEvent>
            {
                new SystemEvent { Id = Guid.NewGuid() },
                new SystemEvent { Id = Guid.NewGuid() }
            };

        _dbContext.DeactivatedUserEvents.AddRange(existingEvents);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterDeactivatedEventsAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
        Assert.That(serviceResult.Errors[0].Value, Does.Contain(string.Join(',', message.Ids)));
    }

    [Test]
    public async Task RegisterDeactivatedEventsAsync_AddInEmptySet_ReturnsNoContentAsync()
    {
        // Arrange
        var se1 = Guid.NewGuid();
        var se2 = Guid.NewGuid();
        var message = CreateInterface<RegisterDeactivatedEvents>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ModifiedBy = "Ivan Ivanov",
            Ids = new HashSet<Guid> { se1, se2 }
        });

        var regSys = new RegisteredSystem() 
        { 
            Id = Guid.NewGuid(),
            IsApproved = true 
        };

        var systemEvents = new List<SystemEvent>
        {
            new SystemEvent { Id = se1 , RegisteredSystem = regSys},
            new SystemEvent { Id = se2 , RegisteredSystem = regSys}
        };

        _dbContext.SystemEvents.AddRange(systemEvents);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterDeactivatedEventsAsync(message);
        var existingEvents = _dbContext.DeactivatedUserEvents.ToList();

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);
        Assert.That(existingEvents, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task RegisterDeactivatedEventsAsync_AddInExistingSet_ReturnsNoContentAsync()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existSE = Guid.NewGuid();
        var newSE = Guid.NewGuid();
        var deleteSE = Guid.NewGuid();

        var message = CreateInterface<RegisterDeactivatedEvents>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = userId,
            ModifiedBy = "Ivan Ivanov",
            Ids = new HashSet<Guid> { existSE, newSE }
        });

        var regSys = new RegisteredSystem()
        {
            Id = Guid.NewGuid(),
            IsApproved = true
        };

        var systemEvents = new List<SystemEvent>
        {
            new SystemEvent { Id = existSE, RegisteredSystem = regSys },
            new SystemEvent { Id = newSE, RegisteredSystem = regSys },
            new SystemEvent { Id = deleteSE, RegisteredSystem = regSys }
        };

        var existingEvents = new List<DeactivatedUserEvent>
        {
            new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = userId, SystemEventId = existSE},
            new DeactivatedUserEvent { Id = Guid.NewGuid(), UserId = userId, SystemEventId = deleteSE}
        };
        _dbContext.DeactivatedUserEvents.AddRange(existingEvents);
        _dbContext.SystemEvents.AddRange(systemEvents);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterDeactivatedEventsAsync(message);
        var currentEvents = _dbContext.DeactivatedUserEvents.ToList();

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);

        Assert.That(currentEvents, Has.Count.EqualTo(2));
        Assert.That(currentEvents.Where(ee => ee.SystemEventId == newSE).Count(), Is.EqualTo(1));
        Assert.That(currentEvents.Where(ee => ee.SystemEventId == existSE).Count(), Is.EqualTo(1));
        Assert.That(currentEvents.Where(ee => ee.SystemEventId == deleteSE).Count(), Is.EqualTo(0));
    }


    private static List<RegisteredSystem> GetRegisteredSystems(Guid id, string name)
    {
        return new List<RegisteredSystem>
        {
            new RegisteredSystem {
                Id = id,
                Name = name,
                IsDeleted = false,
                IsApproved = false,
                Events = new List<Service.Entities.SystemEvent>
                {
                    new Service.Entities.SystemEvent
                    {
                        Id = Guid.NewGuid(),
                        Code = Guid.NewGuid().ToString(),
                        IsDeleted = false,
                        IsMandatory = true,
                        Translations = new List<Service.Entities.Translation>
                        {
                            new Service.Entities.Translation
                            {
                                Language = "bg",
                                ShortDescription = "TestShortDescription",
                                Description = "TestDescription"
                            },
                            new Service.Entities.Translation
                            {
                                Language = "en",
                                ShortDescription = "TestShortDescription",
                                Description = "TestDescription"
                            }
                        }
                    }
                }
            }
        };
    }
}
