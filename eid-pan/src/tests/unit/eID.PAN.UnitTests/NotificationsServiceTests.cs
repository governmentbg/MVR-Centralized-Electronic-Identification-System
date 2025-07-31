using System.Net;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Enums;
using eID.PAN.Service;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Options;
using eID.PAN.UnitTests.Generic;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace eID.PAN.UnitTests;

[TestFixture]
public class NotificationsServiceTests : BaseTest
{
    private Mock<ILogger<NotificationsService>> _mocklogger;
    private Mock<IPublishEndpoint> _mockPublishEndpoint;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private NotificationsService _sut;

    [SetUp]
    public void Init()
    {
        _mocklogger = new Mock<ILogger<NotificationsService>>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();

        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        // Prepare HttpClient
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        var client = new HttpClient(_mockHttpMessageHandler.Object);
        mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);
        var mockConfiguration = new Mock<IConfiguration>();
        var keyCloakOptions = new KeycloakOptions();
        var mockMpozeiCaller = new Mock<IMpozeiCaller>();
        mockMpozeiCaller.Setup(x => x.FetchUserProfileAsync(It.IsAny<string>(), It.IsAny<IdentifierType>())).ReturnsAsync(() => new MpozeiUserProfile { EidentityId = Guid.NewGuid().ToString() });
        mockMpozeiCaller.Setup(x => x.FetchUserProfileAsync(It.IsAny<Guid>())).ReturnsAsync(() => new MpozeiUserProfile { EidentityId = Guid.NewGuid().ToString() });
        mockMpozeiCaller.Setup(x => x.FetchUserProfileByCitizenProfileIdAsync(It.IsAny<Guid>())).ReturnsAsync(() => new MpozeiUserProfile { EidentityId = Guid.NewGuid().ToString() });
        // TODO: Add _configuration.GetValue<int>("NotificationContentMaxLength") = 100
        mockConfiguration.SetupGet(m => m[It.Is<string>(s => s == "NotificationContentMaxLength")]).Returns("100");

        _sut = new NotificationsService(_mocklogger.Object, _cache, _dbContext, _mockPublishEndpoint.Object, mockHttpClientFactory.Object, mockConfiguration.Object, mockMpozeiCaller.Object);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void GetNotificationsByFilterAsync_WhenCalledWithNullFilter_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetByFilterAsync(null));
    }

    [Test]
    public async Task GetNotificationsByFilterAsync_WhenCalledWithInvalidFilterAsync()
    {
        // Arrange
        var notificationFilter = CreateInterface<GetNotificationsByFilter>(new
        {
            PageIndex = 0,
            PageSize = 11,
            SystemName = "System",
            IncludeDeleted = false
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
        var notificationFilter = CreateInterface<GetNotificationsByFilter>(new
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
        var notificationFilter = CreateInterface<GetNotificationsByFilter>(new
        {
            PageIndex = 1,
            PageSize = 10,
            SystemName = "Test",
            IncludeDeleted = false
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
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System1", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System2", IsDeleted = true },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System3", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System4", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "System5", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "6", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "7", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "8", IsDeleted = true },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "9", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "10", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "11", IsDeleted = false },
            new RegisteredSystem { Id = Guid.NewGuid(), Name = "12", IsDeleted = true }
        };
        var notificationFilter = CreateInterface<GetNotificationsByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            PageIndex = 2,
            PageSize = 3,
            SystemName = "System",
            IncludeDeleted = true
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.GetByFilterAsync(notificationFilter);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;
        Assert.That(result.PageIndex, Is.EqualTo(2));
        Assert.That(result.TotalItems, Is.EqualTo(5));
        Assert.That(result.Data, Is.EquivalentTo(registeredSystems.Skip(3).Take(2)));
    }

    [Test]
    public void GetSystemByIdAsync_WithNullMessage_ThrowsArgumentNullExceptionAsync()
    {
        // Arrange, Act + Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetSystemByIdAsync(null));
    }

    [Test]
    public async Task GetSystemByIdAsync_WithInvalidId_ReturnsNotFoundResultAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var registeredSystems = new List<RegisteredSystem>
        {
            new RegisteredSystem { Id = systemId, Name = "System1", IsDeleted = false },
        };

        var message = CreateInterface<GetSystemById>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = id
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        // Act
        var result = await _sut.GetSystemByIdAsync(message);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);

        Assert.IsNull(result.Error);
        Assert.IsNotNull(result.Errors);
        Assert.AreEqual(1, result.Errors.Count);
        Assert.AreEqual(nameof(message.Id), result.Errors[0].Key);
        Assert.AreEqual($"'{message.Id}' not found", result.Errors[0].Value);
    }

    [Test]
    public async Task GetSystemByIdAsync_WithValidId_ReturnsRegisteredSystemResultAsync()
    {
        // Arrange
        var id = Guid.NewGuid();

        var system = new RegisteredSystem { Id = id, Name = "System1", IsDeleted = false };
        var registeredSystems = new List<RegisteredSystem>
        {
            system
        };

        var message = CreateInterface<GetSystemById>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = id
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.GetSystemByIdAsync(message);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.AreEqual(system, result);
    }

    [Test]
    public async Task RegisterSystem_Valid_AddForFirstTimeAsync()
    {
        // Arrange
        var registeredSystems = new List<RegisteredSystem> { };

        var rsTranslationBg = CreateInterface<Contracts.Commands.RegisteredSystemTranslation>(new
        {
            Language = "bg",
            Name = "TestDescription"
        });

        var rsTranslationEn = CreateInterface<Contracts.Commands.RegisteredSystemTranslation>(new
        {
            Language = "en",
            Name = "TestDescription"
        });

        var translationBg = CreateInterface<Contracts.Commands.Translation>(new
        {
            Language = "bg",
            ShortDescription = "TestShortDescription",
            Description = "TestDescription"
        });
        var translationEn = CreateInterface<Contracts.Commands.Translation>(new
        {
            Language = "en",
            ShortDescription = "TestShortDescription",
            Description = "TestDescription"
        });

        var systemEvent1 = CreateInterface<Contracts.Commands.SystemEvent>(new
        {
            Code = "TestCode1",
            IsMandatory = true,
            Translations = new List<Contracts.Commands.Translation>
            {
                translationBg,
                translationEn,
            }
        });

        var systemEvent2 = CreateInterface<Contracts.Commands.SystemEvent>(new
        {
            Code = "TestCode2",
            IsMandatory = true,
            Translations = new List<Contracts.Commands.Translation>
            { translationBg, translationEn }
        });

        var registerSystem = CreateInterface<RegisterSystem>(new
        {
            SystemName = "TestSystem",
            ModifiedBy = "TestUser",
            Translations = new List<Contracts.Commands.RegisteredSystemTranslation>
            { rsTranslationBg, rsTranslationEn },
            Events = new List<Contracts.Commands.SystemEvent>
            { systemEvent1, systemEvent2 }
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterSystemAsync(registerSystem);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
    }

    [Test]
    public async Task RegisterSystem_Valid_AddForNextTimeAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(id, name);

        var rsTranslationBg = CreateInterface<Contracts.Commands.RegisteredSystemTranslation>(new
        {
            Language = "bg",
            Name = "TestDescription"
        });

        var rsTranslationEn = CreateInterface<Contracts.Commands.RegisteredSystemTranslation>(new
        {
            Language = "en",
            Name = "TestDescription"
        });

        var translationBg = CreateInterface<Contracts.Commands.Translation>(new
        {
            Language = "bg",
            ShortDescription = "TestShortDescription",
            Description = "TestDescription"
        });
        var translationEn = CreateInterface<Contracts.Commands.Translation>(new
        {
            Language = "en",
            ShortDescription = "TestShortDescription",
            Description = "TestDescription"
        });

        var systemEvent1 = CreateInterface<Contracts.Commands.SystemEvent>(new
        {
            Code = "TestCode1",
            IsMandatory = true,
            Translations = new List<Contracts.Commands.Translation>
            {
                translationBg,
                translationEn,
            }
        });

        var systemEvent2 = CreateInterface<Contracts.Commands.SystemEvent>(new
        {
            Code = "TestCode2",
            IsMandatory = true,
            Translations = new List<Contracts.Commands.Translation>
            { translationBg, translationEn }
        });

        var registerSystem = CreateInterface<RegisterSystem>(new
        {
            SystemName = name,
            ModifiedBy = "TestUser",
            Translations = new List<Contracts.Commands.RegisteredSystemTranslation>
            { rsTranslationBg, rsTranslationEn },
            Events = new List<Contracts.Commands.SystemEvent>
            { systemEvent1, systemEvent2 }
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SystemEvents.AddRange(registeredSystems[0].Events.ToList());
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterSystemAsync(registerSystem);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.EqualTo(id));
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

    [Test]
    public async Task RegisterSystem_InvalidRequest_ReturnsBadRequestResponseAsync()
    {
        // Arrange
        var registeredSystems = new List<RegisteredSystem> { };

        var rsTranslationBg = CreateInterface<Contracts.Commands.RegisteredSystemTranslation>(new
        {
            Language = "bg",
            Name = "TestDescription"
        });

        var translationBg = CreateInterface<Contracts.Commands.Translation>(new
        {
            Language = "bg",
            ShortDescription = "TestShortDescription",
            Description = "TestDescription"
        });

        var systemEvent1 = CreateInterface<Contracts.Commands.SystemEvent>(new
        {
            Code = "TestCode1",
            IsMandatory = true,
            Translations = new List<Contracts.Commands.Translation>
            { translationBg }
        });

        var registerSystem = CreateInterface<RegisterSystem>(new
        {
            SystemName = "",
            ModifiedBy = "",
            Translations = new List<Contracts.Commands.RegisteredSystemTranslation>
            { rsTranslationBg },
            Events = new List<Contracts.Commands.SystemEvent>
            { systemEvent1 }
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.RegisterSystemAsync(registerSystem);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public void RegisterSystem_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RegisterSystemAsync(null));
    }

    [Test]
    public void ModifyEventAsync_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ModifyEventAsync(null));
    }

    [Test]
    public async Task ModifyEventAsync_WithInvalidData_ShouldReturnNotFoundResultAsync()
    {
        // Arrange
        var registeredSystems = GetRegisteredSystems(Guid.NewGuid(), "SYSTEM1");

        var message = CreateInterface<ModifyEvent>(new
        {
            Id = Guid.NewGuid(),
            ModifiedBy = "",
            IsDeleted = false
        });
        _dbContext.RegisteredSystems.Add(registeredSystems[0]);
        _dbContext.SaveChanges();

        // Create a ModifySystem object with invalid data (e.g. missing required fields)

        // Act
        var result = await _sut.ModifyEventAsync(message);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ModifyEventAsync_WithValidData_ShouldReturnNoContentResultAsync()
    {
        // Arrange
        var registeredSystems = GetRegisteredSystems(Guid.NewGuid(), "SYSTEM1");
        var id = registeredSystems[0].Events.First().Id;

        var message = CreateInterface<ModifyEvent>(new
        {
            Id = id,
            ModifiedBy = "",
            IsDeleted = true
        });

        _dbContext.RegisteredSystems.Add(registeredSystems[0]);
        _dbContext.SaveChanges();

        // Act
        var result = await _sut.ModifyEventAsync(message);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NoContent);
    }

    [Test]
    public async Task ModifyEventAsync_WithValidData_ShouldReturnNotModifiedResultAsync()
    {
        // Arrange
        var registeredSystems = GetRegisteredSystems(Guid.NewGuid(), "SYSTEM1");
        var id = registeredSystems[0].Events.First().Id;

        var message = CreateInterface<ModifyEvent>(new
        {
            Id = id,
            ModifiedBy = "",
            IsDeleted = false
        });

        _dbContext.RegisteredSystems.Add(registeredSystems[0]);
        _dbContext.SaveChanges();

        // Act
        var result = await _sut.ModifyEventAsync(message);

        // Assert
        CheckServiceResult(result, HttpStatusCode.NotModified);
    }

    [Test]
    public void SendAsync_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SendAsync(null));
    }

    [Test]
    public async Task SendAsync_WithInvalidData_ShouldReturnWarningLogAsync()
    {
        // Arrange
        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = new Guid("00000000-0000-0000-0000-000000000001"),
            SystemName = string.Empty,
            EventCode = string.Empty
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnWarningLogSysNotExistAsync()
    {
        // Arrange
        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = NewId.NextGuid().ToString(),
            EventCode = NewId.NextGuid().ToString(),
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("Notification will not be sent. {0} does not exist in the system", message.SystemName);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Warning, Times.Once());
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnInformationLogEventIsDeletedAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].Events.First().IsDeleted = true;
        registeredSystems[0].IsApproved = true;

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("Notification will not be sent. {0}:{1} is deleted",
                message.SystemName, message.EventCode);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Information, Times.Once());
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnWarningLogSystemNotApprovedAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = false;

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("Notification will not be sent. {0}:{1} is not approved",
                nameof(message.SystemName), message.SystemName);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Warning, Times.Once());
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnWarningLogSystemIsDeletedAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;
        registeredSystems[0].IsDeleted = true;

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("Notification will not be sent. {0} is archived", message.SystemName);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Warning, Times.Once());
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnInformationLogDeactivatedAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";

        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;
        registeredSystems[0].Events.First().IsMandatory = false;

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.DeactivatedUserEvents.Add(
            new DeactivatedUserEvent
            {
                Id = Guid.NewGuid(),
                UserId = message.UserId.HasValue ? message.UserId.Value : Guid.Empty,
                SystemEventId = registeredSystems[0].Events.First().Id
            });
        _dbContext.SaveChanges();

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("Notification will not be sent. {0}:{1} is deactivated from the user {2}",
                message.SystemName, message.EventCode, message.UserId);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Information, Times.Once());
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnWarningLogNoNotificationChannelsAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;

        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("{0} are empty", nameof(_dbContext.NotificationChannels));
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Warning, Times.Once());
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
    }

    private static NotificationChannelApproved CreateApprovedNotificationChannel(string name, bool isBuiltIn = false)
       => new NotificationChannelApproved
       {
           Id = new Guid("2711532B-5FEE-4518-97C6-43B7D40A0A85"),
           IsBuiltIn = isBuiltIn,
           Name = name,
           Description = "Вътрешен канал за изпращане на имейли",
           CallbackUrl = "http://localhost/",
           Price = 0,
           InfoUrl = "http://localhost/",
           Translations = new List<Service.Entities.NotificationChannelTranslation>
                {
                    new Service.Entities.NotificationChannelTranslation
                    {
                        Language = "bg",
                        Name = name,
                        Description = "Вътрешен канал за изпращане на имейли",
                    },
                    new Service.Entities.NotificationChannelTranslation
                    {
                        Language = "en",
                        Name = name,
                        Description = "Internal channel for sending emails",
                    }
                }
       };

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnWarningLogNotUserNCSelectedAndNotDefaultChannelAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;
        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.NotificationChannels.Add(CreateApprovedNotificationChannel(Guid.NewGuid().ToString()));
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("Default notification channel ({0}) is missing", ConfigurationsConstants.SMTP);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Error, Times.Once());
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldReturnInfLogUserWillFallBackToSMTPAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;
        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.NotificationChannels.Add(CreateApprovedNotificationChannel(ConfigurationsConstants.SMTP, isBuiltIn: true));
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            },
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        var errorMessage = string.Format("User {0} did not select any {1}. It has been fell back to {2}",
                message.UserId, nameof(_dbContext.UserNotificationChannels), ConfigurationsConstants.SMTP);
        _mocklogger.VerifyLogging(errorMessage, LogLevel.Information, Times.Once());
        CheckServiceResult(result, HttpStatusCode.Accepted);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldSendInternallySendEmailAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;
        _dbContext.RegisteredSystems.AddRange(registeredSystems);
        _dbContext.NotificationChannels.Add(CreateApprovedNotificationChannel(ConfigurationsConstants.SMTP, isBuiltIn: true));
        _dbContext.SaveChanges();

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            }
        });

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        _mockPublishEndpoint.Verify(x => x.Publish<SendEmail>(It.IsAny<object>(),
            It.IsAny<CancellationToken>()), Times.Once);

        _mockHttpMessageHandler.Protected().Verify("SendAsync", Times.Exactly(0),
            ItExpr.Is<HttpRequestMessage>(req =>
                req.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>());

        CheckServiceResult(result, HttpStatusCode.Accepted);
    }

    [Test]
    public async Task SendAsync_WithValidData_ShouldExternalUriAsync()
    {
        // Arrange
        Guid systemId = Guid.NewGuid();
        var name = "SYSTEM1";
        var registeredSystems = GetRegisteredSystems(systemId, name);
        registeredSystems[0].IsApproved = true;

        var message = CreateInterface<SendNotification>(new
        {
            CorrelationId = NewId.NextGuid(),
            SystemName = name,
            EventCode = registeredSystems[0].Events.First().Code,
            Translations = new List<SendNotificationTranslation>
            {
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "bg",
                        Message = "Съобщение",
                    }),
                CreateInterface<SendNotificationTranslation>(new
                    {
                        Language = "en",
                        Message = "Message"
                    })
            },
            UserId = new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6")
        });
        _dbContext.RegisteredSystems.AddRange(registeredSystems);

        var notificationChannel = CreateApprovedNotificationChannel(Guid.NewGuid().ToString());
        _dbContext.NotificationChannels.Add(notificationChannel);
        _dbContext.UserNotificationChannels.Add(new UserNotificationChannel
        {
            Id = Guid.NewGuid(),
            UserId = message.UserId.HasValue ? message.UserId.Value : Guid.Empty,
            NotificationChannelId = notificationChannel.Id
        });

        _dbContext.SaveChanges();

        // Act
        var result = await _sut.SendAsync(message);

        // Assert
        _mockPublishEndpoint.Verify(x => x.Publish<SendEmail>(It.IsAny<object>(),
            It.IsAny<CancellationToken>()), Times.Never);

        CheckServiceResult(result, HttpStatusCode.Accepted);
    }

    [Test]
    public void RejectSystemAsync_ThrowsArgumentNullException()
    {
        // Arrange		
        // Act & Assert		
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RejectSystemAsync(null));
    }

    [Test]
    public async Task RejectSystemAsync_WhenCannotFindSystem_ReturnsNotFound()
    {
        // Arrange		
        var rejectSystem = CreateInterface<RejectSystem>(new
        {
            CorrelationId = Guid.NewGuid(),
            SystemId = Guid.NewGuid(),
            UserId = Guid.NewGuid().ToString(),
            Reason = Guid.NewGuid().ToString(),
        });

        //Act
        var serviceResult = await _sut.RejectSystemAsync(rejectSystem);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RejectSystemAsync_WhenCalledWithValidData_ShouldDeleteSystemFromRegisteredSystemsAndMoveItToRejected()
    {
        // Arrange	
        var testId1 = Guid.NewGuid();

        await SeedRegisteredSystemsAsync(testId1);

        var rejectSystem = CreateInterface<RejectSystem>(new
        {
            CorrelationId = Guid.NewGuid(),
            SystemId = testId1,
            UserId = Guid.NewGuid().ToString(),
            Reason = Guid.NewGuid().ToString(),
        });

        var getSystemById = CreateInterface<GetSystemById>(new
        {
            Id = testId1,
        });

        //Act
        var serviceResult = await _sut.RejectSystemAsync(rejectSystem);
        var notFoundSystem = await _sut.GetSystemByIdAsync(getSystemById);

        //Assert
        var rejectedSystem = _dbContext.RegisteredSystemsRejected.FirstOrDefault();
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        CheckServiceResult(notFoundSystem, HttpStatusCode.NotFound);
        Assert.Multiple(() =>
        {
            Assert.That(rejectedSystem, Is.Not.Null);
            Assert.That(serviceResult.Result, Is.EqualTo(rejectedSystem?.Id));
            Assert.That(rejectedSystem?.Name, Is.EqualTo("Name0"));
            Assert.That(_dbContext.RegisteredSystemsRejected.Count(), Is.EqualTo(1));
        });

        CheckServiceResult(serviceResult, HttpStatusCode.OK);
    }

    [Test]
    public void ApproveSystemAsync_ThrowsArgumentNullException()
    {
        // Arrange		
        // Act & Assert		
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ApproveSystemAsync(null));
    }

    [Test]
    public async Task ApproveSystemAsync_WhenCalledWithInvalidId_ShouldReturnNotFound()
    {
        var approveSystem = CreateInterface<ApproveSystem>(new
        {
            SystemId = Guid.NewGuid(),
        });

        //Act
        var serviceResult = await _sut.ApproveSystemAsync(approveSystem);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ApproveSystemAsync_WhenCalledValidData_ShouldSetIsApprovedToTrue()
    {
        //Arrange
        var testId1 = Guid.NewGuid();

        await SeedRegisteredSystemsAsync(testId1);

        var approveSystem = CreateInterface<ApproveSystem>(new
        {
            SystemId = testId1,
        });

        var getSystemById = CreateInterface<GetSystemById>(new
        {
            Id = testId1,
        });

        //Act
        var serviceResult = await _sut.ApproveSystemAsync(approveSystem);

        //Assert
        var approvedSystem = await _sut.GetSystemByIdAsync(getSystemById);
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result, Is.EqualTo(testId1));
        CheckServiceResult(approvedSystem, HttpStatusCode.OK);

        Assert.That(approvedSystem?.Result?.IsApproved, Is.EqualTo(true));
    }

    [Test]
    public void ArchiveSystemAsync_ThrowsArgumentNullException()
    {
        // Arrange		
        // Act & Assert		
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ArchiveSystemAsync(null));
    }

    [Test]
    public async Task ArchiveSystemAsync_WhenCalledWithInvalidId_ShouldReturnNotFound()
    {
        //Arrange
        var archiveSystem = CreateInterface<ArchiveSystem>(new
        {
            SystemId = Guid.NewGuid(),
        });

        //Act
        var serviceResult = await _sut.ArchiveSystemAsync(archiveSystem);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ArchiveSystemAsync_WhenCalledWithValidIdButSystemIsNotApproved_ShouldReturnConflict()
    {
        //Arrange
        var testId1 = Guid.NewGuid();
        var testId2 = Guid.NewGuid();

        await SeedRegisteredSystemsAsync(testId1, testId2);

        var archiveSystem = CreateInterface<ArchiveSystem>(new
        {
            SystemId = testId1
        });

        //Act
        var serviceResult = await _sut.ArchiveSystemAsync(archiveSystem);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ArchiveSystemAsync_WhenCalledWithValidData_ShouldSetIsDeletedToTrue()
    {
        //Arrange
        var testId1 = Guid.NewGuid();

        await SeedRegisteredSystemsAsync(testId1);

        var archiveSystem = CreateInterface<ArchiveSystem>(new
        {
            SystemId = testId1
        });

        var approveSystem = CreateInterface<ApproveSystem>(new
        {
            SystemId = testId1,
        });
        var approvedSystem = await _sut.ApproveSystemAsync(approveSystem);

        //Act
        var serviceResult = await _sut.ArchiveSystemAsync(archiveSystem);

        //Assert
        var archivedSystem = await _sut.GetSystemByIdAsync(CreateInterface<GetSystemById>(new
        {
            Id = serviceResult.Result
        }));
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result, Is.EqualTo(testId1));
        Assert.That(archivedSystem?.Result?.IsDeleted, Is.EqualTo(true));
    }

    [Test]
    public void RestoreSystemAsync_ThrowsArgumentNullException()
    {
        // Arrange		
        // Act & Assert		
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RestoreSystemAsync(null));
    }

    [Test]
    public async Task RestoreSystemAsync_WhenCalledWithInvalidId_ShouldReturnNotFound()
    {
        //Arrange
        var restoreSystem = CreateInterface<RestoreSystem>(new
        {
            SystemId = Guid.NewGuid(),
        });

        //Act
        var serviceResult = await _sut.RestoreSystemAsync(restoreSystem);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RestoreSystemAsync_WhenSystemIsNotDeleted_ShouldReturnConflict()
    {
        //Arrange
        var testId1 = Guid.NewGuid();

        await SeedRegisteredSystemsAsync(testId1);

        var restoreSystem = CreateInterface<RestoreSystem>(new
        {
            SystemId = testId1,
        });


        //Act
        var restoredSystem = await _sut.RestoreSystemAsync(restoreSystem);

        //Assert
        CheckServiceResult(restoredSystem, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task RestoreSystemAsync_WhenCalledWithValidData_ShouldRestoreProperlyAndReturnRestoredId()
    {
        //Arrange
        var testId1 = Guid.NewGuid();

        await SeedRegisteredSystemsAsync(testId1);

        _ = await _sut.ApproveSystemAsync(CreateInterface<ApproveSystem>(new
        {
            SystemId = testId1,
        }));
        _ = await _sut.ArchiveSystemAsync(CreateInterface<ArchiveSystem>(new
        {
            SystemId = testId1,
        }));

        //Act
        var restoredSystemRequest = await _sut.RestoreSystemAsync(CreateInterface<RestoreSystem>(new
        {
            SystemId = testId1,
        }));
        var systemById = await _sut.GetSystemByIdAsync(CreateInterface<GetSystemById>(new
        {
            Id = restoredSystemRequest.Result,
        }));

        //Assert
        CheckServiceResult(restoredSystemRequest, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(restoredSystemRequest?.Result, Is.EqualTo(testId1));
            Assert.That(systemById?.Result?.IsDeleted, Is.EqualTo(false));
        });
    }

    private async Task SeedRegisteredSystemsAsync(params Guid[] ids)
    {
        var registeredSystems = new List<RegisteredSystem>();

        for (int i = 0; i < ids.Length; i++)
        {
            registeredSystems.Add(new RegisteredSystem
            {
                Id = ids[i],
                Name = $"Name{i}",
                IsApproved = false,
                IsDeleted = false,
                ModifiedBy = "",
                ModifiedOn = DateTime.UtcNow,
            });
        }

        await _dbContext.RegisteredSystems.AddRangeAsync(registeredSystems);
        await _dbContext.SaveChangesAsync();
    }
}
