using eID.PAN.Contracts.Commands;
using eID.PAN.Service;
using eID.PAN.Service.Database;
using eID.PAN.Service.Entities;
using eID.PAN.Service.Options;
using eID.PAN.UnitTests.Generic;
using MailKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using NUnit.Framework;

namespace eID.PAN.UnitTests;

public class CommunicationsServiceTests : BaseTest
{
    private ILogger<CommunicationsService> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private CommunicationsService _sut;

    private Mock<ISmtpClient> _smtpClientMock;
    private Mock<IPushNotificationSender> _pushNotificationSender;
    private Mock<ISmsSender> _smsSender;
    private Mock<IHttpCallbackSender> _httpCallbackSender;

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<CommunicationsService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();

        //SMTP options Mock
        var smtpOptionsMock = new Mock<IOptions<SmtpOptions>>();
        smtpOptionsMock.Setup(x => x.Value).Returns(() => new SmtpOptions { SenderEmail = "testsenderemail@test.bg", SenderName = "Test Sender Name" });

        //SMTP client Mock
        _smtpClientMock = new Mock<ISmtpClient>();
        _pushNotificationSender = new Mock<IPushNotificationSender>();

        _pushNotificationSender
            .Setup(x => x.SendPushNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _smsSender = new Mock<ISmsSender>();
        _smsSender
            .Setup(x => x.SendSmsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _httpCallbackSender = new Mock<IHttpCallbackSender>();
        _httpCallbackSender
            .Setup(x => x.SendHttpCallbackAsync(It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(true);

        var aesOptionsMock = new Mock<IOptions<AesOptions>>();
        aesOptionsMock.Setup(x => x.Value).Returns(() => new AesOptions { Key = "MyTestkey1234567", Vector = "MyTestVector1234" });

        var mockMpozeiCaller = new Mock<IMpozeiCaller>();
        mockMpozeiCaller.Setup(x => x.FetchUserProfileAsync(It.IsAny<Guid>())).ReturnsAsync(new MpozeiUserProfile() { PhoneNumber  = "1234567", FirebaseId = "xyz"});
        mockMpozeiCaller.Setup(x => x.FetchUserProfileByCitizenProfileIdAsync(It.IsAny<Guid>())).ReturnsAsync(new MpozeiUserProfile() { PhoneNumber  = "1234567", FirebaseId = "xyz"});
        _sut = new CommunicationsService(_logger, _cache, _dbContext, smtpOptionsMock.Object, _smtpClientMock.Object, _pushNotificationSender.Object, _smsSender.Object, _httpCallbackSender.Object, aesOptionsMock.Object, mockMpozeiCaller.Object);
    }

    [Test]
    public void SendEmailAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SendEmailAsync(null));
    }

    [Test]
    public async Task SendEmailAsync_WhenCalledWithInvalidMessage_ShouldNotSendEmailAsync()
    {
        // Arrange
        await SeedSmtpConfigurationAsync();

        var sendEmail = CreateInterface<SendEmail>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Translations = new List<SendEmailTranslation>
                {
                    CreateInterface<SendEmailTranslation>(new { Language = "bg", Message = "" }),
                    CreateInterface<SendEmailTranslation>(new { Language = "en", Message = "EN" }),
                }
        });

        // Act
        await _sut.SendEmailAsync(sendEmail);

        // Assert
        _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never);
    }

    [Test]
    public async Task SendEmailAsync_WhenCalledWithInvalidLanguage_ShouldNotSendEmailAsync()
    {
        // Arrange
        await SeedSmtpConfigurationAsync();

        var sendEmail = CreateInterface<SendEmail>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Translations = new List<SendEmailTranslation>
                {
                    CreateInterface<SendEmailTranslation>(new { Language = "bg1", Message = "БГ" }),
                    CreateInterface<SendEmailTranslation>(new { Language = "en", Message = "EN" }),
                }
        });

        // Act
        await _sut.SendEmailAsync(sendEmail);
        // Assert
        _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never);
    }

    [Test]
    public async Task SendEmailAsync_WhenCannotFindSmtpConfiguration_ShouldNotSendEmailAsync()
    {
        // Arrange
        var sendEmail = CreateInterface<SendEmail>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Translations = new List<SendEmailTranslation>
            {
                CreateInterface<SendEmailTranslation>(new { Language = "bg", Message = "БГ" }),
                CreateInterface<SendEmailTranslation>(new { Language = "en", Message = "EN" }),
            }
        });

        // Act
        await _sut.SendEmailAsync(sendEmail);

        // Assert
        _smtpClientMock.Verify(x => x.SendAsync(It.IsAny<MimeMessage>(), It.IsAny<CancellationToken>(), It.IsAny<ITransferProgress>()), Times.Never);
    }

    [Test]
    public async Task SendPushNotificationAsync_CalledWithValidData_ShouldSucceedAsync()
    {
        // Arrange
        var command = CreateInterface<SendPushNotification>(new
        {
            CorrelationId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Translations = new List<PushNotificationTranslation>
                {
                    CreateInterface<PushNotificationTranslation>(new { Language = "bg", Message = "БГ" }),
                    CreateInterface<PushNotificationTranslation>(new { Language = "en", Message = "EN" }),
                }
        });

        // Act
        var result = await _sut.SendPushNotificationAsync(command);

        //Assert
        Assert.That(result, Is.True);
        _pushNotificationSender.Verify(x => x.SendPushNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(1));

    }

    private static readonly object[] PushNotificationInvalidDataTestCases =
    {
         new object[] {
            "Missing UserId",
            CreateInterface<SendPushNotification>(new
            {
                Translations = new List<PushNotificationTranslation>
                    {
                        CreateInterface<PushNotificationTranslation>(new { Language = "bg", Message = "БГ" }),
                        CreateInterface<PushNotificationTranslation>(new { Language = "en", Message = "EN" }),
                    }
            })
         },
         new object[] {
            "Missing Translations",
            CreateInterface<SendPushNotification>(new
            {
                UserId = Guid.NewGuid()
            })
         },
         new object[] {
            "Missing bg translation",
            CreateInterface<SendPushNotification>(new
            {
                UserId = Guid.NewGuid(),
                Translations = new List<PushNotificationTranslation>
                    {
                        CreateInterface<PushNotificationTranslation>(new { Language = "en", Message = "EN" }),
                    }
            })
         },
         new object[] {
            "Missing en translation",
            CreateInterface<SendPushNotification>(new
            {
                UserId = Guid.NewGuid(),
                Translations = new List<PushNotificationTranslation>
                    {
                        CreateInterface<PushNotificationTranslation>(new { Language = "bg", Message = "БГ" })
                    }
            })
         }
    };

    [Test]
    [TestCaseSource(nameof(PushNotificationInvalidDataTestCases))]
    public async Task SendPushNotificationAsync_WhenCalledWithInvalidData_ShouldReturnFalseAndNotTrySendingAsync(string caseName, SendPushNotification command)
    {
        // Act
        var result = await _sut.SendPushNotificationAsync(command);

        //Assert
        Assert.That(result, Is.False, caseName);
        _pushNotificationSender.Verify(x => x.SendPushNotificationAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void SendSmsAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SendSmsAsync(null));
    }

    [Test]
    public async Task SendSmsAsync_WhenCalledWithInvalidTranslations_ShouldReturnFalseAsync()
    {
        // Arrange
        // Act
        var sendSms = CreateInterface<SendSms>(new
        {
            UserId = Guid.NewGuid(),
            //Empty translations
        });

        var result = await _sut.SendSmsAsync(sendSms);
        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SendSmsAsync_WhenCalledWithInvalidUserId_ShouldReturnFalseAsync()
    {
        // Arrange
        // Act
        var sendSms = CreateInterface<SendSms>(new { });

        var result = await _sut.SendSmsAsync(sendSms);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task SendSmsAsync_WhenCalledWithValidData_ShouldReturnTrueAndExecutedSendSmsAsyncMethod()
    {
        // Arrange
        // Act
        var sendSms = CreateInterface<SendSms>(new
        {
            UserId = Guid.NewGuid(),
            Translations = new List<SendSmsTranslation>
            {
                CreateInterface<SendSmsTranslation>(new { Language = "bg", Message = "БГ" }),
                CreateInterface<SendSmsTranslation>(new { Language = "en", Message = "EN" }),
            }
        });

        var result = await _sut.SendSmsAsync(sendSms);

        // Assert
        _smsSender.Verify(x => x.SendSmsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        Assert.That(result, Is.True);
    }

    private async Task SeedSmtpConfigurationAsync()
    {
        var dbConfigurations = new List<SmtpConfiguration>
        {
            new SmtpConfiguration
            {
                Id = Guid.NewGuid(),
                Server = "TestServer1",
                Port = 1,
                UserName = "TestUserName1",
                Password = "TestPassword1",
                CreatedOn = DateTime.UtcNow,
                CreatedBy = "TestCreatedBy1",
                ModifiedOn = null,
                ModifiedBy = null
            },
        };

        await _dbContext.SmtpConfigurations.AddRangeAsync(dbConfigurations);
        await _dbContext.SaveChangesAsync();
    }
}
