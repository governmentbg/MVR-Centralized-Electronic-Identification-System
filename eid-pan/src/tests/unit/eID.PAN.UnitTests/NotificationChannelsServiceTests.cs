using System.Net;
using eID.PAN.Contracts.Commands;
using eID.PAN.Contracts.Events;
using eID.PAN.Service;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.UnitTests.Generic;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace eID.PAN.UnitTests;

[TestFixture]
public class NotificationChannelsServiceTests : BaseTest
{
    private ILogger<NotificationChannelsService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private Mock<IPublishEndpoint> _publishEndpoint;
    private NotificationChannelsService _sut;


    private const string _testUserName = "Test User";

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<NotificationChannelsService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _sut = new NotificationChannelsService(_logger, _cache, _dbContext, _publishEndpoint.Object);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    #region GetAllChannels
    [Test]
    public async Task GetAllChannels_WithoutGeneratedData_ReturnsEmptyResultButNotNullAsync()
    {
        var serviceResult = await _sut.GetAllChannelsAsync();

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;

        Assert.That(result, Is.Not.Null);
        CollectionAssert.IsEmpty(result.Approved);
        CollectionAssert.IsEmpty(result.Archived);
        CollectionAssert.IsEmpty(result.Pending);
        CollectionAssert.IsEmpty(result.Rejected);
    }

    [Test]
    public async Task GetAllChannels_WithPendingAndApprovedData_ReturnsNotEmptyResultForPendingAndApprovedRecordsAsync()
    {
        var systemId = Guid.NewGuid();
        var translationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var translationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        // Arrange
        var pendingNotificationChannels = new List<NotificationChannelPending>
        {
            new NotificationChannelPending { Id = Guid.NewGuid(), SystemId = systemId, Name = "TestCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ translationBg , translationEn} },
            new NotificationChannelPending { Id = Guid.NewGuid(), SystemId = systemId, Name = "TestCh2", Description = "Test Channel 2", Price = 10, CallbackUrl="www.aaa.com/callback", InfoUrl="www.aaa.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ translationBg , translationEn} }
        };
        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ translationBg , translationEn} },
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = systemId, Name = "AppCh2", Description = "Approved Channel 2", Price = 11, CallbackUrl="www.aaa.com/callback", InfoUrl="www.aaa.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ translationBg , translationEn} }
        };

        _dbContext.NotificationChannelsPending.AddRange(pendingNotificationChannels);
        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.SaveChanges();

        // Act
        var serviceResult = await _sut.GetAllChannelsAsync();

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Null);
        CollectionAssert.IsNotEmpty(result.Pending);
        CollectionAssert.IsNotEmpty(result.Approved);
        CollectionAssert.IsEmpty(result.Rejected);
        CollectionAssert.IsEmpty(result.Archived);

        Assert.That(result.Pending, Is.EquivalentTo(pendingNotificationChannels));
        Assert.That(result.Approved, Is.EquivalentTo(approvedNotificationChannels));
    }
    #endregion

    #region RegisterChannel
    [Test]
    public async Task RegisterChannelAsync_AddSingleChannel_ReturnsChannelIdAsync()
    {
        // Arrange
        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var registerNotificationChannel = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = Guid.NewGuid(),
            Name = "Channel 1",
            Description = "Descr for Ch 1",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));
    }

    [Test]
    public async Task RegisterChannelAsync_WithoutName_ReturnsBadRequestAsync()
    {
        // Arrange
        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var registerNotificationChannel = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = Guid.NewGuid(),
            //Name = "",
            Description = "Descr for Ch 1",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RegisterChannelAsync_InvalidWithoutBgTranslation_ReturnsBadRequestAsync()
    {
        // Arrange
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var registerNotificationChannel = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = Guid.NewGuid(),
            Name = "Channel 1",
            Description = "Descr for Ch 1",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationEn }
        });

        // Act
        var serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task RegisterChannelAsync_AddTwoChannelsWithSameNameAndSameSystemId_OneChannelIsAddedAsync()
    {
        // Arrange
        var systemId = Guid.NewGuid();
        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var registerNotificationChannel = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = systemId,
            Name = "Channel 1",
            Description = "Descr for Ch 1",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        var registerNotificationChannelWithTheSameName = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = systemId,
            Name = "channel 1",
            Description = "Description",
            ModifiedBy = "TestUser2",
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannelWithTheSameName);
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Pending.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task RegisterChannelAsync_AddChannelWithSameNameAndDifferentSystemId_ShouldReturnConflictAsync()
    {
        // Arrange
        var systemId = Guid.NewGuid();
        var system2Id = Guid.NewGuid();
        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var registerNotificationChannel = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = systemId,
            Name = "Channel 1",
            Description = "Descr for Ch 1",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        var registerNotificationChannelWithTheSameName = CreateInterface<RegisterNotificationChannel>(new
        {
            SystemId = system2Id,
            Name = "channel 1",
            Description = "Description",
            ModifiedBy = "TestUser2",
            CallbackUrl = "www.ch1.com/asdf",
            Price = 1.5m,
            InfoUrl = "www.ch1.com/about-us",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        serviceResult = await _sut.RegisterChannelAsync(registerNotificationChannelWithTheSameName);
        CheckServiceResult(serviceResult, HttpStatusCode.Conflict);
    }

    [Test]
    public void RegisterChannelAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.RegisterChannelAsync(null));
    }
    #endregion

    #region ModifyChannel
    [Test]
    public async Task ModifyChannelAsync_WithValidData_AddToPendingAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };

        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.SaveChanges();

        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var modifyChannel = CreateInterface<ModifyNotificationChannel>(new
        {
            Id = id,
            SystemId = systemId,
            Name = "channel 1",
            Description = "Description",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/test",
            Price = 0m,
            InfoUrl = "www.ch1.com",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.ModifyChannelAsync(modifyChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Approved.Count(), Is.EqualTo(1));
        Assert.That(getResult.Pending.Count(), Is.EqualTo(1));//new pending channel is added
    }

    [Test]
    public async Task ModifyChannelAsync_WithValidDataAndExistingPendingRecord_UpdatePendingAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };
        var pendingNotificationChannels = new List<NotificationChannelPending>
        {
            new NotificationChannelPending { Id = Guid.NewGuid(), SystemId = systemId, Name = "TestCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.NotificationChannelsPending.AddRange(pendingNotificationChannels);
        _dbContext.SaveChanges();

        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var modifyChannel = CreateInterface<ModifyNotificationChannel>(new
        {
            Id = id,
            SystemId = systemId,
            Name = "TestCh1",
            Description = "Description",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/test-new",
            Price = 12.5m,
            InfoUrl = "www.ch1.com",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.ModifyChannelAsync(modifyChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Approved.Count(), Is.EqualTo(1));
        Assert.That(getResult.Pending.Count(), Is.EqualTo(1));//nothing is added, just replaced
    }

    [Test]
    public async Task ModifyChannelAsync_WithNonExistingKey_ReturnsNotFoundAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };

        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.SaveChanges();

        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var modifyChannel = CreateInterface<ModifyNotificationChannel>(new
        {
            Id = Guid.NewGuid(),
            SystemId = systemId,
            Name = "channel 1",
            Description = "Description",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.ch1.com/test",
            Price = 0m,
            InfoUrl = "www.ch1.com",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.ModifyChannelAsync(modifyChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task ModifyChannelAsync_WithInvalidData_ReturnsBadRequestAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };

        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.SaveChanges();

        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });
        var translationEn = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        });

        var modifyChannel = CreateInterface<ModifyNotificationChannel>(new
        {
            Id = Guid.NewGuid(),
            SystemId = systemId,
            Name = "",
            Description = "Description",
            ModifiedBy = _testUserName,
            CallbackUrl = "",
            Price = 0m,
            InfoUrl = "",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg, translationEn }
        });

        // Act
        var serviceResult = await _sut.ModifyChannelAsync(modifyChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task ModifyChannelAsync_WithMissingTranslation_ReturnsBadRequestAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };

        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.SaveChanges();

        var translationBg = CreateInterface<Contracts.Commands.NotificationChannelTranslation>(new
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        });

        var modifyChannel = CreateInterface<ModifyNotificationChannel>(new
        {
            Id = Guid.NewGuid(),
            SystemId = systemId,
            Name = "Test",
            Description = "Description",
            ModifiedBy = _testUserName,
            CallbackUrl = "www.test",
            Price = 0m,
            InfoUrl = "www.test",
            Translations = new List<Contracts.Commands.NotificationChannelTranslation>
            { translationBg }
        });

        // Act
        var serviceResult = await _sut.ModifyChannelAsync(modifyChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public void ModifyChannelAsync_WithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ModifyChannelAsync(null));
    }
    #endregion

    #region Approve,Reject, Archive, Restore
    [Test]
    public async Task ApproveChannelAsync_WithValidData_ShouldMoveToApprovedAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var pendingNotificationChannels = new List<NotificationChannelPending>
        {
            new NotificationChannelPending { Id = id, SystemId = systemId, Name = "TestCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannelsPending.AddRange(pendingNotificationChannels);
        _dbContext.SaveChanges();

        var approveChannel = CreateInterface<ApproveNotificationChannel>(new
        {
            Id = id,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.ApproveChannelAsync(approveChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Approved.Count(), Is.EqualTo(1));
        Assert.That(getResult.Pending.Count(), Is.EqualTo(0));//removed from pending
    }

    [Test]
    public async Task ApproveChannelAsync_WithValidDataAndSameName_ShouldReplaceApprovedAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var approvedNotificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = Guid.NewGuid(), SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };
        var pendingNotificationChannels = new List<NotificationChannelPending>
        {
            new NotificationChannelPending { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannels.AddRange(approvedNotificationChannels);
        _dbContext.NotificationChannelsPending.AddRange(pendingNotificationChannels);
        _dbContext.SaveChanges();

        var approveChannel = CreateInterface<ApproveNotificationChannel>(new
        {
            Id = id,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.ApproveChannelAsync(approveChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Approved.Count(), Is.EqualTo(1));//replace approved
        Assert.That(getResult.Pending.Count(), Is.EqualTo(0));//removed from pending
    }

    [Test]
    public async Task ApproveChannelAsync_WithNonExistingKey_ReturnsNotFoundAsync()
    {
        // Arrange
        var approveChannel = CreateInterface<ApproveNotificationChannel>(new
        {
            Id = Guid.NewGuid(),
            ModifiedBy = _testUserName
        });

        //Act
        var serviceResult = await _sut.ApproveChannelAsync(approveChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public void ApproveChannelAsync_CalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.ApproveChannelAsync(null));
    }

    [Test]
    public async Task RejectChannelAsync_WithValidData_ShouldMoveToRejectedAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var pendingNotificationChannels = new List<NotificationChannelPending>
        {
            new NotificationChannelPending { Id = id, SystemId = Guid.NewGuid(), Name = "TestCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannelsPending.AddRange(pendingNotificationChannels);
        _dbContext.SaveChanges();

        var rejectChannel = CreateInterface<RejectNotificationChannel>(new
        {
            Id = id,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.RejectChannelAsync(rejectChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Rejected.Count(), Is.EqualTo(1));
        Assert.That(getResult.Approved.Count(), Is.EqualTo(0));
        Assert.That(getResult.Pending.Count(), Is.EqualTo(0));//removed from pending
    }

    [Test]
    public async Task ArchiveChannelAsync_WithValidData_ShouldMoveToArchivedAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var notificationChannels = new List<NotificationChannelApproved>
        {
            new NotificationChannelApproved { Id = id, SystemId = systemId, Name = "TestCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannels.AddRange(notificationChannels);
        _dbContext.SaveChanges();

        var archiveChannel = CreateInterface<ArchiveNotificationChannel>(new
        {
            Id = id,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.ArchiveChannelAsync(archiveChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Archived.Count(), Is.EqualTo(1));
        Assert.That(getResult.Approved.Count(), Is.EqualTo(0));

        _publishEndpoint.Verify(e => e.Publish<NotificationChannelDeactivated>(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task RestoreChannelAsync_WithValidData_ShouldMoveToApprovedAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var notificationChannels = new List<NotificationChannelArchive>
        {
            new NotificationChannelArchive { Id = id, SystemId = Guid.NewGuid(), Name = "TestCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannelsArchive.AddRange(notificationChannels);
        _dbContext.SaveChanges();

        var restoreChannel = CreateInterface<RestoreNotificationChannel>(new
        {
            Id = id,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.RestoreChannelAsync(restoreChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Archived.Count(), Is.EqualTo(0));
        Assert.That(getResult.Approved.Count(), Is.EqualTo(1)); //move from archive to approved
    }

    [Test]
    public async Task RestoreChannelAsync_WithValidDataAndSameName_ShouldReplacePendingAsync()
    {
        // Arrange
        var id = Guid.NewGuid();
        var systemId = Guid.NewGuid();
        var ncTranslationBg = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "bg",
            Name = "Тест",
            Description = "Тестово Описание"
        };
        var ncTranslationEn = new Service.Entities.NotificationChannelTranslation()
        {
            Language = "en",
            Name = "Test",
            Description = "Test Description"
        };

        var archivedNotificationChannels = new List<NotificationChannelArchive>
        {
            new NotificationChannelArchive { Id = id, SystemId = systemId, Name = "AppCh1", Description = "Approved Channel 1", Price = 5.5m, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg , ncTranslationEn} }
        };
        var pendingNotificationChannels = new List<NotificationChannelPending>
        {
            new NotificationChannelPending { Id = Guid.NewGuid(), SystemId = systemId, Name = "AppCh1", Description = "Test Channel 1", Price = 0, CallbackUrl="www.test.com/callback", InfoUrl="www.test.com",
                    Translations = new List<Service.Entities.NotificationChannelTranslation>{ ncTranslationBg, ncTranslationEn } },
        };

        _dbContext.NotificationChannelsArchive.AddRange(archivedNotificationChannels);
        _dbContext.NotificationChannelsPending.AddRange(pendingNotificationChannels);
        _dbContext.SaveChanges();

        var restoreChannel = CreateInterface<RestoreNotificationChannel>(new
        {
            Id = id,
            ModifiedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.RestoreChannelAsync(restoreChannel);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(Guid)));

        var getServiceResult = await _sut.GetAllChannelsAsync();
        CheckServiceResult(getServiceResult, HttpStatusCode.OK);
        var getResult = getServiceResult.Result;
        Assert.That(getResult, Is.Not.Null);
        Assert.That(getResult.Archived.Count(), Is.EqualTo(0));
        Assert.That(getResult.Pending.Count(), Is.EqualTo(1));
    }
    #endregion
}
