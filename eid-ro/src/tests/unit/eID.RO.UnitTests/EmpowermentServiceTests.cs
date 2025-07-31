using System.Net;
using System.Text.RegularExpressions;
using eID.PJS.AuditLogging;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Events;
using eID.RO.Contracts.Results;
using eID.RO.Service;
using eID.RO.Service.Database;
using eID.RO.Service.Entities;
using eID.RO.Service.Interfaces;
using eID.RO.Service.Options;
using eID.RO.Service.Requests;
using eID.RO.UnitTests.Generic;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Moq;
using NUnit.Framework;

namespace eID.RO.UnitTests;

internal class EmpowermentServiceTests : BaseTest
{
    private Mock<ILogger<EmpowermentsService>> _logger;
    private IDistributedCache _cache;
    private ApplicationDbContext _dbContext;
    private Mock<IPublishEndpoint> _publishEndpoint;
    private Mock<IRequestClient<CheckUidsRestrictions>> _checkUidsRestrictions;
    private Mock<IRequestClient<CheckForEmpowermentsVerification>> _checkEmpowermentVerificationSagasClient;
    private EmpowermentsService _sut;
    private Mock<IVerificationService> _verificationService;
    private Mock<IHttpClientFactory> _httpClientFactory;
    private Mock<INumberRegistrator> _numberRegistrator;
    private Mock<INotificationSender> _notificationSender;

    private const string _testUserName = "Test User";
    private readonly AuthorizerIdentifierData _testEmpoweringUid = new() { Uid = "9201011235", UidType = IdentifierType.EGN, Name = "Тест", IsIssuer = true };
    private const string _testEmpoweringName = "Test Name";

    private const string _testProviderId = "1";
    private const string _testProviderName = "Пътна Полиция";
    private const int _testServiceId = 5;
    private const string _testServiceName = "Управление на автомобил";
    private readonly VolumeOfRepresentation[] _testVolumeOfRepresentation = new[] { new VolumeOfRepresentation { Name = "1" }, new VolumeOfRepresentation { Name = "3" } };
    private readonly DateTime _testStartDate = DateTime.Today;
    private readonly DateTime _testEndDate = DateTime.Today.AddMonths(2);

    [SetUp]
    public void Init()
    {
        _logger = new Mock<ILogger<EmpowermentsService>>();
        var auditLogger = new Mock<IAuditLogger>().Object;

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();
        _publishEndpoint = new Mock<IPublishEndpoint>();
        _checkUidsRestrictions = new Mock<IRequestClient<CheckUidsRestrictions>>();
        _checkEmpowermentVerificationSagasClient = new Mock<IRequestClient<CheckForEmpowermentsVerification>>();

        var mockEmpowermentsVerificationSagasCheckResult = new Mock<Response<EmpowermentsVerificationSagasCheckResult>>();
        mockEmpowermentsVerificationSagasCheckResult.Setup(x => x.Message).Returns(new EmpowermentsVerificationSagasCheckResultDTO
        {
            AllSagasExistAndFinished = true
        });

        _checkEmpowermentVerificationSagasClient
            .Setup(x => x.GetResponse<EmpowermentsVerificationSagasCheckResult>(It.IsAny<object>(), default, default))
            .ReturnsAsync(mockEmpowermentsVerificationSagasCheckResult.Object);

        _verificationService = new Mock<IVerificationService>();
        _httpClientFactory = new Mock<IHttpClientFactory>();
        _numberRegistrator = new Mock<INumberRegistrator>();
        _notificationSender = new Mock<INotificationSender>();

        var configuration = new Mock<IConfiguration>();
        var configurationSectionMock = new Mock<IConfigurationSection>();

        configuration
            .Setup(c => c.GetSection(It.IsAny<string>()))
            .Returns(configurationSectionMock.Object);

        configurationSectionMock
            .Setup(s => s.Value)
            .Returns("true");

        _sut = new EmpowermentsService(
            configuration.Object, _logger.Object, auditLogger, _cache, _dbContext, _publishEndpoint.Object, _checkUidsRestrictions.Object,
            _verificationService.Object, _httpClientFactory.Object, _numberRegistrator.Object, _notificationSender.Object, _checkEmpowermentVerificationSagasClient.Object);
    }
    internal class EmpowermentsVerificationSagasCheckResultDTO : EmpowermentsVerificationSagasCheckResult
    {
        public bool AllSagasExistAndFinished { get; set; }
        public IEnumerable<Guid> MissingIds { get; set; }
    }
    internal class CryptoKeyProviderStub : ICryptoKeyProvider
    {
        public byte[] GetKey()
        {
            return new byte[] { 0x00 };
        }
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    #region CreateIndividualStatement
    [Test]
    public async Task CreateIndividualStatementAsync_EmpowerSinglePerson_ReturnsSingleIdAsync()
    {
        // Arrange
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = Guid.NewGuid(),
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "9002022237", UidType = IdentifierType.EGN } },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.CreateStatementAsync(addIndividualStatement);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(IEnumerable<Guid>)));
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task CreateIndividualStatementAsync_EmpowerSinglePersonWithoutEndDate_ReturnsSingleIdAsync()
    {
        // Arrange
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = Guid.NewGuid(),
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "9002022237", UidType = IdentifierType.EGN } },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            CreatedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.CreateStatementAsync(addIndividualStatement);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(IEnumerable<Guid>)));
        Assert.That(result.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task CreateIndividualStatementAsync_EmpowerMultiplePeopleTogether_ReturnsSingleIdAsync()
    {
        // Arrange
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = Guid.NewGuid(),
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Together,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[]
            {
                new() { Uid = "9002022237", UidType = IdentifierType.EGN },
                new() { Uid = "9102023345", UidType = IdentifierType.EGN },
                new() { Uid = "9203035560", UidType = IdentifierType.EGN }
            },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.CreateStatementAsync(addIndividualStatement);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(IEnumerable<Guid>)));
        Assert.That(result.Count(), Is.EqualTo(1));

        var createdStatement = _dbContext.EmpowermentStatements
           .Include(es => es.StatusHistory)
           .Where(es => result.Contains(es.Id))
           .ToList();

        Assert.That(createdStatement.All(cs => cs.StatusHistory.Any(sh => sh.Status == EmpowermentStatementStatus.Created)));
    }

    [Test]
    public async Task CreateIndividualStatementAsync_EmpowerMultiplePeopleSeparately_ReturnsSingleIdAsync()
    {
        // Arrange
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = Guid.NewGuid(),
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[]
            {
                new() { Uid = "9002022237", UidType = IdentifierType.EGN },
                new() { Uid = "9102023345", UidType = IdentifierType.EGN },
                new() { Uid = "9203035560", UidType = IdentifierType.EGN }
            },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });

        // Act
        var serviceResult = await _sut.CreateStatementAsync(addIndividualStatement);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.Not.Empty);
        Assert.That(result, Is.InstanceOf(typeof(IEnumerable<Guid>)));
        Assert.That(result.Count(), Is.EqualTo(3));

        var createdStatement = _dbContext.EmpowermentStatements
            .Include(es => es.StatusHistory)
            .Where(es => result.Contains(es.Id))
            .ToList();

        Assert.That(createdStatement.All(cs => cs.StatusHistory.Any(sh => sh.Status == EmpowermentStatementStatus.Created)));
    }

    [Test]
    public async Task ChangeEmpowermentStatusAsync_WorksCorrectlyAsync()
    {
        // Arrange
        var empowermentId = Guid.NewGuid();
        _dbContext.EmpowermentStatements.Add(new EmpowermentStatement
        {
            Id = empowermentId,
            Uid = _testEmpoweringUid.Uid,
            Name = _testEmpoweringName,
            AuthorizerUids = new List<AuthorizerUid>
            {
                new AuthorizerUid { EmpowermentStatementId = empowermentId, Uid = _testEmpoweringUid.Uid, UidType = _testEmpoweringUid.UidType }
            },
            EmpoweredUids = new List<EmpoweredUid>
            {
                new EmpoweredUid { EmpowermentStatementId = empowermentId, Uid = "9002022237", UidType = IdentifierType.EGN}
            },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName,
            XMLRepresentation = string.Empty
        });
        _dbContext.SaveChanges();
        var changeStatusCommand = CreateInterface<ChangeEmpowermentStatus>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            Status = EmpowermentStatementStatus.Active
        });
        // Act
        var serviceResult = await _sut.ChangeEmpowermentStatusAsync(changeStatusCommand);
        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.True);
        var dbEntry = _dbContext.EmpowermentStatements
            .Include(x => x.StatusHistory)
            .FirstOrDefault(x => x.Id == empowermentId);
        Assert.That(dbEntry, Is.Not.Null);
        Assert.That(dbEntry.Status, Is.EqualTo(EmpowermentStatementStatus.Active));

        Assert.That(dbEntry.StatusHistory.Any(sh => sh.Status == EmpowermentStatementStatus.Active));
        Assert.That(dbEntry.StatusHistory.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task ChangeEmpowermentStatusAsync_AddsNewStatusHistoryRecordEveryTime_Async()
    {
        // Arrange
        var empowermentId = Guid.NewGuid();
        _dbContext.EmpowermentStatements.Add(new EmpowermentStatement
        {
            Id = empowermentId,
            Uid = _testEmpoweringUid.Uid,
            Name = _testEmpoweringName,
            AuthorizerUids = new List<AuthorizerUid>
            {
                new AuthorizerUid { EmpowermentStatementId = empowermentId, Uid = _testEmpoweringUid.Uid, UidType = _testEmpoweringUid.UidType}
            },
            EmpoweredUids = new List<EmpoweredUid>
            {
                new EmpoweredUid { EmpowermentStatementId = empowermentId, Uid = "9002022237", UidType = IdentifierType.EGN }
            },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName,
            XMLRepresentation = string.Empty,
            StatusHistory = new List<StatusHistoryRecord>()
            {
                new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Created },
            }
        });
        _dbContext.SaveChanges();
        var changeStatusCommand = CreateInterface<ChangeEmpowermentStatus>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            Status = EmpowermentStatementStatus.Active
        });
        // Act
        var serviceResult = await _sut.ChangeEmpowermentStatusAsync(changeStatusCommand);
        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;
        Assert.That(result, Is.True);
        var dbEntry = _dbContext.EmpowermentStatements
            .Include(x => x.StatusHistory)
            .FirstOrDefault(x => x.Id == empowermentId);
        Assert.That(dbEntry, Is.Not.Null);
        Assert.That(dbEntry.Status, Is.EqualTo(EmpowermentStatementStatus.Active));

        Assert.That(dbEntry.StatusHistory.Any(sh => sh.Status == EmpowermentStatementStatus.Active));
        Assert.That(dbEntry.StatusHistory.Count, Is.EqualTo(2));

        changeStatusCommand.Status = EmpowermentStatementStatus.Withdrawn;
        serviceResult = await _sut.ChangeEmpowermentStatusAsync(changeStatusCommand);

        Assert.That(dbEntry.StatusHistory.Any(sh => sh.Status == EmpowermentStatementStatus.Active) && dbEntry.StatusHistory.Any(sh => sh.Status == EmpowermentStatementStatus.Withdrawn));
        Assert.That(dbEntry.StatusHistory.Count, Is.EqualTo(3));
    }

    [Test]
    public async Task SignEmpowermentAsync_ProvidedValidData_Succeeds()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = correlationId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new AuthorizerIdentifierData[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "9002022237", UidType = IdentifierType.EGN } },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });
        var creationResult = await _sut.CreateStatementAsync(addIndividualStatement);
        var empowermentId = creationResult.Result?.First();
        await _sut.ChangeEmpowermentStatusAsync(CreateInterface<ChangeEmpowermentStatus>(new
        {
            CorrelationId = correlationId,
            EmpowermentId = empowermentId,
            Status = EmpowermentStatementStatus.CollectingAuthorizerSignatures
        }));


        // No restricted authorizers
        _checkUidsRestrictions
            .Setup(rc => rc.GetResponse<ServiceResult<UidsRestrictionsResult>>(It.IsAny<object>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(() => CreateInterface<Response<ServiceResult<UidsRestrictionsResult>>>(new
            {
                Message = new ServiceResult<UidsRestrictionsResult>
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = new UidsRestrictionsResult { Successful = true }
                }
            }));

        // Successful signature verification
        _verificationService
            .Setup(vs => vs.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()))
            .ReturnsAsync(() => new ServiceResult { StatusCode = HttpStatusCode.OK });

        var signEmpowermentCommand = CreateInterface<SignEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            SignatureProvider = SignatureProvider.KEP,
            DetachedSignature = "real-string"
        });

        // Act
        var serviceResult = await _sut.SignEmpowermentAsync(signEmpowermentCommand);
        // Assert
        _verificationService.Verify(x =>
                x.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()),
                Times.Once()
        );
        Assert.That(_dbContext.EmpowermentSignatures.Count(r => r.EmpowermentStatementId == empowermentId), Is.EqualTo(1));
        _publishEndpoint.Verify(pe =>
            pe.Publish<EmpowermentSigned>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once()
        );
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
    }

    [Test]
    public async Task SignEmpowermentAsync_ProvidedNonExistingEmpowerment_Stops()
    {
        // Arrange
        var empowermentId = Guid.NewGuid();

        var signEmpowermentCommand = CreateInterface<SignEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            SignatureProvider = SignatureProvider.KEP,
            DetachedSignature = "real-string"
        });

        // Act
        var serviceResult = await _sut.SignEmpowermentAsync(signEmpowermentCommand);
        // Assert
        _verificationService.Verify(x =>
                x.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()),
                Times.Never
        );
        Assert.That(_dbContext.EmpowermentSignatures.Count(r => r.EmpowermentStatementId == empowermentId), Is.EqualTo(0));
        _publishEndpoint.Verify(pe =>
            pe.Publish<EmpowermentSigned>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task SignEmpowermentAsync_ProvidedWrongStatus_Stops()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = correlationId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "9002022237", UidType = IdentifierType.EGN } },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });
        var creationResult = await _sut.CreateStatementAsync(addIndividualStatement);
        var empowermentId = creationResult.Result?.First();

        var signEmpowermentCommand = CreateInterface<SignEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            SignatureProvider = SignatureProvider.KEP,
            DetachedSignature = "real-string"
        });

        // Act
        var serviceResult = await _sut.SignEmpowermentAsync(signEmpowermentCommand);
        // Assert
        _verificationService.Verify(x =>
                x.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()),
                Times.Never
        );
        Assert.That(_dbContext.EmpowermentSignatures.Count(r => r.EmpowermentStatementId == empowermentId), Is.EqualTo(0));
        _publishEndpoint.Verify(pe =>
            pe.Publish<EmpowermentSigned>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        CheckServiceResult(serviceResult, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task SignEmpowermentAsync_ProvidedWrongAuthorizer_Stops()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = correlationId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "9002022237", UidType = IdentifierType.EGN } },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });
        var creationResult = await _sut.CreateStatementAsync(addIndividualStatement);
        var empowermentId = creationResult.Result?.First();

        var signEmpowermentCommand = CreateInterface<SignEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            Uid = "9002022237",
            UidType = IdentifierType.EGN,
            SignatureProvider = SignatureProvider.KEP,
            DetachedSignature = "real-string"
        });

        // Act
        var serviceResult = await _sut.SignEmpowermentAsync(signEmpowermentCommand);
        // Assert
        _verificationService.Verify(x =>
                x.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()),
                Times.Never
        );
        Assert.That(_dbContext.EmpowermentSignatures.Count(r => r.EmpowermentStatementId == empowermentId), Is.EqualTo(0));
        _publishEndpoint.Verify(pe =>
            pe.Publish<EmpowermentSigned>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        CheckServiceResult(serviceResult, HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task SignEmpowermentAsync_ProvidedWrongCertificate_Stops()
    {
        // Arrange
        var correlationId = Guid.NewGuid();
        var addIndividualStatement = CreateInterface<AddEmpowermentStatement>(new
        {
            CorrelationId = correlationId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            Name = _testEmpoweringName,
            OnBehalfOf = OnBehalfOf.Individual,
            TypeOfEmpowerment = TypeOfEmpowerment.Separately,
            AuthorizerUids = new[] { _testEmpoweringUid },
            EmpoweredUids = new UserIdentifierData[] { new() { Uid = "9002022237", UidType = IdentifierType.EGN } },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName
        });
        var creationResult = await _sut.CreateStatementAsync(addIndividualStatement);
        var empowermentId = creationResult.Result?.First();
        await _sut.ChangeEmpowermentStatusAsync(CreateInterface<ChangeEmpowermentStatus>(new
        {
            CorrelationId = correlationId,
            EmpowermentId = empowermentId,
            Status = EmpowermentStatementStatus.CollectingAuthorizerSignatures
        }));


        // No restricted authorizers
        _checkUidsRestrictions
            .Setup(rc => rc.GetResponse<ServiceResult<UidsRestrictionsResult>>(It.IsAny<object>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(() => CreateInterface<Response<ServiceResult<UidsRestrictionsResult>>>(new
            {
                Message = new ServiceResult<UidsRestrictionsResult>
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = new UidsRestrictionsResult { Successful = true }
                }
            }));

        // Failing signature verification
        _verificationService
            .Setup(vs => vs.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()))
            .ReturnsAsync(() => new ServiceResult { StatusCode = HttpStatusCode.BadRequest });

        var signEmpowermentCommand = CreateInterface<SignEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentId,
            _testEmpoweringUid.Uid,
            _testEmpoweringUid.UidType,
            SignatureProvider = SignatureProvider.KEP,
            DetachedSignature = "real-string"
        });

        // Act
        var serviceResult = await _sut.SignEmpowermentAsync(signEmpowermentCommand);
        // Assert
        _verificationService.Verify(x =>
                x.VerifySignatureAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IdentifierType>(), It.IsAny<SignatureProvider>()),
                Times.Once()
        );
        Assert.That(_dbContext.EmpowermentSignatures.Count(r => r.EmpowermentStatementId == empowermentId), Is.EqualTo(0));
        _publishEndpoint.Verify(pe =>
            pe.Publish<EmpowermentSigned>(It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
        Assert.That(serviceResult.StatusCode == HttpStatusCode.BadRequest);
    }
    #endregion
    [Test]
    public void GetEmpowermentsToMeByFilterAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetEmpowermentsToMeByFilterAsync(null));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_DoesNotReturnDisallowedStatuses_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var createdStatements = serviceResult.Result.Data.Where(r => r.Status == EmpowermentStatementStatus.Created).Count();
        var deniedStatements = serviceResult.Result.Data.Where(r => r.Status == EmpowermentStatementStatus.Denied).Count();
        Assert.Multiple(() =>
        {
            Assert.That(createdStatements, Is.EqualTo(0));
            Assert.That(deniedStatements, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_GetOneLegalEntityWithPartOfEik_Async()
    {
        // Arrange
        var seededData = await SeedEmpowermentStatementsAsync();
        var legalEntity = seededData.First(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity && sd.Status == EmpowermentStatementStatus.Active);
        var uid = legalEntity.EmpoweredUids.First();

        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = uid.Uid,
            UidType = uid.UidType,
            OnBehalfOf = OnBehalfOf.LegalEntity,
            Eik = legalEntity.Uid,

            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var statements = serviceResult.Result.Data.Count();
        var legalEntityCount = seededData.Count(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity && sd.Uid == "762640804");
        
        Assert.Multiple(() =>
        {
            Assert.That(statements, Is.EqualTo(legalEntityCount));
        });
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_GetOneLegalEntityWithoutEik_Async()
    {
        // Arrange
        var seededData = await SeedEmpowermentStatementsAsync();
        var legalEntity = seededData.First(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity && sd.Status == EmpowermentStatementStatus.Active);
        var uid = legalEntity.EmpoweredUids.First();

        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = uid.Uid,
            UidType = uid.UidType,
            OnBehalfOf = OnBehalfOf.LegalEntity,
            //Eik = "762640804",

            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var statements = serviceResult.Result.Data.Count();
        var legalEntityCount = seededData.Count(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity);

        Assert.Multiple(() =>
        {
            Assert.That(statements, Is.EqualTo(legalEntityCount));
        });
    }

    [Test]
    [TestCase(EmpowermentsToMeFilterStatus.Active)]
    [TestCase(EmpowermentsToMeFilterStatus.DisagreementDeclared)]
    [TestCase(EmpowermentsToMeFilterStatus.Withdrawn)]
    [TestCase(EmpowermentsToMeFilterStatus.Expired)]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenStatus_ReturnStatementsWithProvidedStatus_Async(EmpowermentsToMeFilterStatus status)
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Status = status,
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var expectedStatus = status switch
        {
            EmpowermentsToMeFilterStatus.Active => EmpowermentStatementStatus.Active,
            EmpowermentsToMeFilterStatus.DisagreementDeclared => EmpowermentStatementStatus.DisagreementDeclared,
            EmpowermentsToMeFilterStatus.Withdrawn => EmpowermentStatementStatus.Withdrawn,
            EmpowermentsToMeFilterStatus.Expired => EmpowermentStatementStatus.Active,
        };

        var areAllActiveEmpowerments = serviceResult.Result.Data.All(r => r.Status == expectedStatus);
        Assert.That(areAllActiveEmpowerments, Is.EqualTo(true));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenAuthorizer_ReturnStatementsWithProvidedAuthorizer_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            Authorizer = "TestName1",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenProviderName_ReturnStatementsWithProvidedSupplier_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            ProviderName = "TestProviderName5",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenProviderName_ReturnNotStatementsWithProvidedSupplierAndUnavailableStauts_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            ProviderName = "TestProviderName2",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenServiceName_ReturnStatementsWithProvidedServiceName_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            ServiceName = "TestServiceName5",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenEmpoweredUids_ReturnsFilteredStatementsByEmpoweredUids_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            EmpoweredUids = new List<UserIdentifierData> {
                new() { Uid = "4711183713", UidType = IdentifierType.EGN },
                new() { Uid = "8802184852", UidType = IdentifierType.EGN } },
            PageSize = 10,
            PageIndex = 1
        }); ;

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
    }


    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenServiceName_ReturnStatementsWithProvidedServiceIdAlso_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            ServiceName = "666",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenValidToDate_ReturnStatementsWithDateBeforeProvidedDate_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var validToDate = DateTime.UtcNow.AddYears(1);
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            ValidToDate = validToDate,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var allStatementsBeforeDate = serviceResult.Result.Data.All(r => r.ExpiryDate < validToDate);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(4));
            Assert.That(allStatementsBeforeDate, Is.EqualTo(true));
        });
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenShowOnlyNoExpiryDate_ReturnOnlyNoExpiryStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            ShowOnlyNoExpiryDate = true,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenAscendingSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsToMeSortBy.Authorizer,
            SortDirection = SortDirection.Asc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.First().Name, Is.EqualTo("TestName1"));
        Assert.That(serviceResult.Result.Data.Last().Name, Is.EqualTo("TestName7"));
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenDescendingSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsToMeSortBy.Authorizer,
            SortDirection = SortDirection.Desc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.First().Name, Is.EqualTo("TestName7"));
        Assert.That(serviceResult.Result.Data.Last().Name, Is.EqualTo("TestName1"));
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenServiceNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsToMeSortBy.ServiceName,
            SortDirection = SortDirection.Asc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.First().ServiceName, Is.EqualTo("TestServiceName1"));
        Assert.That(serviceResult.Result.Data.Last().ServiceName, Is.EqualTo("TestServiceName7"));
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenDescendingServiceNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsToMeSortBy.ServiceName,
            SortDirection = SortDirection.Desc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.First().ServiceName, Is.EqualTo("TestServiceName7"));
        Assert.That(serviceResult.Result.Data.Last().ServiceName, Is.EqualTo("TestServiceName1"));
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenProviderNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsToMeSortBy.ProviderName,
            SortDirection = SortDirection.Asc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.First().ProviderName, Is.EqualTo("TestProviderName1"));
        Assert.That(serviceResult.Result.Data.Last().ProviderName, Is.EqualTo("TestProviderName7"));
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(5));
    }

    [Test]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenGivenDescendingProviderNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsToMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsToMeSortBy.ProviderName,
            SortDirection = SortDirection.Desc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsToMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult.Result.Data.First().ProviderName, Is.EqualTo("TestProviderName7"));
        Assert.That(serviceResult.Result.Data.Last().ProviderName, Is.EqualTo("TestProviderName1"));
        Assert.That(serviceResult.Result.Data.Count(), Is.EqualTo(5));
    }

    [Test]
    public void GetEmpowermentsFromMeByFilterAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetEmpowermentsFromMeByFilterAsync(null));
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_ReturnsAllStatuses_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(6));
    }

    [Test]
    [TestCase(EmpowermentsFromMeFilterStatus.Active)]
    [TestCase(EmpowermentsFromMeFilterStatus.DisagreementDeclared)]
    [TestCase(EmpowermentsFromMeFilterStatus.Withdrawn)]
    [TestCase(EmpowermentsFromMeFilterStatus.Expired)]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenStatus_ReturnStatementsWithProvidedStatus_Async(EmpowermentsFromMeFilterStatus status)
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Status = status,
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var expectedStatus = status switch
        {
            EmpowermentsFromMeFilterStatus.Active => EmpowermentStatementStatus.Active,
            EmpowermentsFromMeFilterStatus.DisagreementDeclared => EmpowermentStatementStatus.DisagreementDeclared,
            EmpowermentsFromMeFilterStatus.Withdrawn => EmpowermentStatementStatus.Withdrawn,
            EmpowermentsFromMeFilterStatus.Expired => EmpowermentStatementStatus.Active,
        };

        var areAllActiveEmpowerments = serviceResult?.Result?.Data?.All(r => r.Status == expectedStatus);
        Assert.That(areAllActiveEmpowerments, Is.EqualTo(true));
    }

    [Test]
    [TestCaseSource(nameof(EmpowermentStatementNumberTestCases))]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenFilteredByNumber_ReturnsOneOrZeroResults_Async(string number, string numberFilter, bool oneResult)
    {
        // Arrange
        var empowermentId = Guid.NewGuid();
        _dbContext.EmpowermentStatements.Add(new EmpowermentStatement
        {
            Id = empowermentId,
            Number = number,
            Status = EmpowermentStatementStatus.Active,
            Uid = _testEmpoweringUid.Uid,
            Name = _testEmpoweringName,
            AuthorizerUids = new List<AuthorizerUid>
            {
                new AuthorizerUid { EmpowermentStatementId = empowermentId, Uid = _testEmpoweringUid.Uid, UidType = _testEmpoweringUid.UidType }
            },
            EmpoweredUids = new List<EmpoweredUid>
            {
                new EmpoweredUid { EmpowermentStatementId = empowermentId, Uid = "9002022237", UidType = IdentifierType.EGN}
            },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName,
            XMLRepresentation = string.Empty
        });
        _dbContext.SaveChanges();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Number = numberFilter,
            Uid = _testEmpoweringUid.Uid,
            UidType = _testEmpoweringUid.UidType,
            PageSize = 10,
            PageIndex = 1
        });

        // Act
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(() => { return serviceResult?.Result?.Data?.Count() == (oneResult ? 1 : 0); }, 
            "EmpowermentStatement number {0}, filter {1} result must be {2}, but result is {3}", 
            number, numberFilter, oneResult ? 1 : 0, serviceResult?.Result?.Data?.Count());
    }

    [Test]
    [TestCaseSource(nameof(EmpowermentStatementNumberTestCases))]
    public async Task GetEmpowermentsToMeByFilterAsync_WhenFilteredByNumber_ReturnsOneOrZeroResults_Async(string number, string numberFilter, bool oneResult)
    {
        // Arrange
        var empowermentId = Guid.NewGuid();
        _dbContext.EmpowermentStatements.Add(new EmpowermentStatement
        {
            Id = empowermentId,
            Number = number,
            Status = EmpowermentStatementStatus.Active,
            Uid = _testEmpoweringUid.Uid,
            Name = _testEmpoweringName,
            AuthorizerUids = new List<AuthorizerUid>
            {
                new AuthorizerUid { EmpowermentStatementId = empowermentId, Uid = _testEmpoweringUid.Uid, UidType = _testEmpoweringUid.UidType }
            },
            EmpoweredUids = new List<EmpoweredUid>
            {
                new EmpoweredUid { EmpowermentStatementId = empowermentId, Uid = "9002022237", UidType = IdentifierType.EGN}
            },
            ProviderId = _testProviderId,
            ProviderName = _testProviderName,
            ServiceId = _testServiceId,
            ServiceName = _testServiceName,
            VolumeOfRepresentation = _testVolumeOfRepresentation,
            StartDate = _testStartDate,
            ExpiryDate = _testEndDate,
            CreatedBy = _testUserName,
            XMLRepresentation = string.Empty
        });
        _dbContext.SaveChanges();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsToMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Number = numberFilter,
            Uid = "9002022237",
            UidType = IdentifierType.EGN,
            PageSize = 10,
            PageIndex = 1
        });

        // Act
        var serviceResult = await _sut.GetEmpowermentsToMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(() => { return serviceResult?.Result?.Data?.Count() == (oneResult ? 1 : 0); },
            "EmpowermentStatement number {0}, filter {1} result must be {2}, but result is {3}",
            number, numberFilter, oneResult ? 1 : 0, serviceResult?.Result?.Data?.Count());
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenAuthorizer_ReturnStatementsWithProvidedAuthorizer_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            Authorizer = "TestName1",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenProviderName_ReturnStatementsWithProvidedSupplier_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ProviderName = "TestProviderName5",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenProviderName_ReturnStatementsWithProvidedSupplierAndAllStautuses_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ProviderName = "TestProviderName2",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(serviceResult?.Result?.Data?.First().Status, Is.EqualTo(EmpowermentStatementStatus.Created));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenServiceName_ReturnStatementsWithProvidedServiceName_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ServiceName = "TestServiceName5",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(serviceResult?.Result?.Data?.First().ServiceName, Is.EqualTo("TestServiceName5"));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenServiceName_ReturnStatementsWithProvidedServiceIdAlso_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ServiceName = "666",
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(1));
        Assert.That(serviceResult?.Result?.Data?.First().ServiceId, Is.EqualTo(666));
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenValidToDate_ReturnStatementsWithDateBeforeProvidedDate_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var validToDate = DateTime.UtcNow.AddYears(1);
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ValidToDate = validToDate,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var allStatementsBeforeDate = serviceResult?.Result?.Data.All(r => r.ExpiryDate < validToDate);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(5));
            Assert.That(allStatementsBeforeDate, Is.EqualTo(true));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenSortByCreatedOnDesc_ReturnsStatementsSortedByCreatedOnDesc_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var validToDate = DateTime.UtcNow.AddYears(1);
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ValidToDate = validToDate,
            PageSize = 10,
            PageIndex = 1,
            SortBy = EmpowermentsFromMeSortBy.CreatedOn,
            SortDirection = SortDirection.Desc
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(5));
            Assert.That(serviceResult?.Result?.Data.First().CreatedOn, Is.GreaterThan(serviceResult?.Result?.Data.Last().CreatedOn));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenSortByCreatedOnAsc_ReturnsStatementsSortedByCreatedOnAsc_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var validToDate = DateTime.UtcNow.AddYears(1);
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ValidToDate = validToDate,
            PageSize = 10,
            PageIndex = 1,
            SortBy = EmpowermentsFromMeSortBy.CreatedOn,
            SortDirection = SortDirection.Asc
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(5));
            Assert.That(serviceResult?.Result?.Data.First().CreatedOn, Is.LessThan(serviceResult?.Result?.Data.Last().CreatedOn));
        });
    }
    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenShowOnlyNoExpiryDate_ReturnOnlyNoExpiryStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            ShowOnlyNoExpiryDate = true,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(serviceResult?.Result?.Data?.First().ExpiryDate, Is.Null);
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenAscendingSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsFromMeSortBy.Name,
            SortDirection = SortDirection.Asc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.First().Name, Is.EqualTo("TestName1"));
            Assert.That(serviceResult?.Result?.Data?.Last().Name, Is.EqualTo("TestName7"));
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(6));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenDescendingSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsFromMeSortBy.Name,
            SortDirection = SortDirection.Desc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.First().Name, Is.EqualTo("TestName7"));
            Assert.That(serviceResult?.Result?.Data?.Last().Name, Is.EqualTo("TestName1"));
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(6));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenServiceNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsFromMeSortBy.ServiceName,
            SortDirection = SortDirection.Asc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.First().ServiceName, Is.EqualTo("TestServiceName1"));
            Assert.That(serviceResult?.Result?.Data?.Last().ServiceName, Is.EqualTo("TestServiceName7"));
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(6));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenDescendingServiceNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsFromMeSortBy.ServiceName,
            SortDirection = SortDirection.Desc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.First().ServiceName, Is.EqualTo("TestServiceName7"));
            Assert.That(serviceResult?.Result?.Data?.Last().ServiceName, Is.EqualTo("TestServiceName1"));
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(6));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_WhenGivenDescendingProviderNameSort_ReturnSortedStatements_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();
        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "4711183713",
            UidType = IdentifierType.EGN,
            SortBy = EmpowermentsFromMeSortBy.ProviderName,
            SortDirection = SortDirection.Desc,
            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(serviceResult?.Result?.Data?.First().ProviderName, Is.EqualTo("TestProviderName7"));
            Assert.That(serviceResult?.Result?.Data?.Last().ProviderName, Is.EqualTo("TestProviderName1"));
            Assert.That(serviceResult?.Result?.Data?.Count(), Is.EqualTo(6));
        });
    }


    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_GetOneLegalEntityWithPartOfEik_Async()
    {
        // Arrange
        var seededData = await SeedEmpowermentStatementsAsync();
        var legalEntity = seededData.First(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity && sd.Status == EmpowermentStatementStatus.Active);
        var authorizer = legalEntity.AuthorizerUids.First();

        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = authorizer.Uid,
            UidType = authorizer.UidType,
            OnBehalfOf = OnBehalfOf.LegalEntity,
            Eik = legalEntity.Uid,

            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var statements = serviceResult.Result.Data.Count();
        var legalEntityCount = seededData.Count(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity && sd.Uid == "762640804");

        Assert.Multiple(() =>
        {
            Assert.That(statements, Is.EqualTo(legalEntityCount));
        });
    }

    [Test]
    public async Task GetEmpowermentsFromMeByFilterAsync_GetOneLegalEntityWithoutEik_Async()
    {
        // Arrange
        var seededData = await SeedEmpowermentStatementsAsync();
        var legalEntity = seededData.First(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity && sd.Status == EmpowermentStatementStatus.Active);
        var authorizer = legalEntity.AuthorizerUids.First();

        var getEmpowermentsFromMeByFilter = CreateInterface<GetEmpowermentsFromMeByFilter>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = authorizer.Uid,
            UidType = authorizer.UidType,
            OnBehalfOf = OnBehalfOf.LegalEntity,
            //Eik = "762640804",

            PageSize = 10,
            PageIndex = 1
        });

        // Act & Assert
        var serviceResult = await _sut.GetEmpowermentsFromMeByFilterAsync(getEmpowermentsFromMeByFilter);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        var statements = serviceResult.Result.Data.Count();
        var legalEntityCount = seededData.Count(sd => sd.OnBehalfOf == OnBehalfOf.LegalEntity);

        Assert.Multiple(() =>
        {
            Assert.That(statements, Is.EqualTo(legalEntityCount));
        });
    }


    [Test]
    public void GetEmpowermentDisagreementReasonsAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetEmpowermentDisagreementReasonsAsync(null));
    }

    [Test]
    public async Task GetEmpowermentDisagreementReasonsAsync_ReturnBadRequest_Async()
    {
        // Arrange
        var message = CreateInterface<GetEmpowermentDisagreementReasons>(new
        {
        });

        // Act
        var serviceResult = await _sut.GetEmpowermentDisagreementReasonsAsync(message);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetEmpowermentDisagreementReasonsAsync_ReturnAnEmptyArray_Async()
    {
        // Arrange
        var message = CreateInterface<GetEmpowermentDisagreementReasons>(new
        {
            CorrelationId = Guid.NewGuid()
        });

        // Act
        var serviceResult = await _sut.GetEmpowermentDisagreementReasonsAsync(message);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        Assert.That(serviceResult?.Result?.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetEmpowermentDisagreementReasonsAsync_ReturnArrayWithTwoResults_Async()
    {
        // Arrange
        var list = await SeedEmpowermentDisagreementReasonListAsync();

        var message = CreateInterface<GetEmpowermentDisagreementReasons>(new
        {
            CorrelationId = Guid.NewGuid()
        });

        // Act
        var serviceResult = await _sut.GetEmpowermentDisagreementReasonsAsync(message);

        //Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);

        Assert.That(serviceResult?.Result?.Count(), Is.EqualTo(2));
        Assert.That(serviceResult?.Result.OrderBy(r => r.Id), Is.EqualTo(list.OrderBy(l => l.Id)));
    }

    [Test]
    public void DisagreeEmpowermentAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.DisagreeEmpowermentAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(DisagreeEmpowermentBadRequestMessages))]
    public async Task DisagreeEmpowermentAsync_WhenBadRequest_ReturnBadRequest_Async(DisagreeEmpowerment message, string caseName)
    {
        // Arrange
        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpowermentNotFound_ReturnNotFound_Async()
    {
        // Arrange
        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "8802184852", //Valid EGN
            UidType = IdentifierType.EGN,
            EmpowermentId = Guid.NewGuid(),
            Reason = "Reason text"
        });

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpoweredUidNotFound_ReturnForbidden_Async()
    {
        // Arrange
        var list = await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = "0401078163", //Valid EGN but does not found
            UidType = IdentifierType.EGN,
            EmpowermentId = list[0].Id,
            Reason = "Reason text"
        });

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpowermentNoActive_ReturnConflict_Async()
    {
        // Arrange
        var list = await SeedEmpowermentStatementsAsync();

        var empowerment = list.First(em => em.Status != EmpowermentStatementStatus.Active);

        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            empowerment.EmpoweredUids.First().Uid,
            empowerment.EmpoweredUids.First().UidType,
            EmpowermentId = empowerment.Id,
            Reason = "Reason text"
        });

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpowermentActiveButExpired_ReturnConflict_Async()
    {
        // Arrange
        var list = await SeedEmpowermentStatementsAsync();

        var empowerment = list.First(em => em.Status == EmpowermentStatementStatus.Active && em.ExpiryDate < DateTime.UtcNow);

        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            empowerment.EmpoweredUids.First().Uid, // Valid and empowered EGN
            empowerment.EmpoweredUids.First().UidType,
            EmpowermentId = empowerment.Id,
            Reason = "Reason text"
        });

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpowermentActiveCheckRestrictionReturnNull_ReturnInternalServerError_Async()
    {
        // Arrange
        var list = await SeedEmpowermentStatementsAsync();

        var empowerment = list.First(em => em.Status == EmpowermentStatementStatus.Active && em.ExpiryDate > DateTime.UtcNow);

        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = empowerment.EmpoweredUids.First().Uid, // Valid and empowered EGN
            empowerment.EmpoweredUids.First().UidType,
            EmpowermentId = empowerment.Id,
            Reason = "Reason text"
        });

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpowermentActiveCheckRestrictionServiceResultNoOK_ReturnInternalServerError_Async()
    {
        // Arrange
        var list = await SeedEmpowermentStatementsAsync();

        var empowerment = list.First(em => em.Status == EmpowermentStatementStatus.Active && em.ExpiryDate > DateTime.UtcNow);

        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = empowerment.EmpoweredUids.First().Uid, // Valid and empowered EGN
            empowerment.EmpoweredUids.First().UidType,
            EmpowermentId = empowerment.Id,
            Reason = "Reason text"
        });

        var checkUidForRestrictionResult = new ServiceResult<UidsRestrictionsResult>
        {
            StatusCode = HttpStatusCode.BadGateway,
            Errors = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("ErrorKey", "ErrorValue") }
        };
        _checkUidsRestrictions
            .Setup(rc => rc.GetResponse<ServiceResult<UidsRestrictionsResult>>(It.IsAny<object>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(() => CreateInterface<Response<ServiceResult<UidsRestrictionsResult>>>(new
            {
                Message = checkUidForRestrictionResult
            }));

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task DisagreeEmpowermentAsync_WhenEmpowermentActiveCheckRestrictionServiceResultOKButFalse_ReturnForbidden_Async()
    {
        // Arrange
        var list = await SeedEmpowermentStatementsAsync();

        var empowerment = list.First(em => em.Status == EmpowermentStatementStatus.Active && em.ExpiryDate > DateTime.UtcNow);

        var message = CreateInterface<DisagreeEmpowerment>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uid = empowerment.EmpoweredUids.First().Uid,
            empowerment.EmpoweredUids.First().UidType,
            EmpowermentId = empowerment.Id,
            Reason = "Reason text"
        });

        _checkUidsRestrictions
            .Setup(rc => rc.GetResponse<ServiceResult<UidsRestrictionsResult>>(It.IsAny<object>(), It.IsAny<CancellationToken>(), It.IsAny<RequestTimeout>()))
            .ReturnsAsync(() => CreateInterface<Response<ServiceResult<UidsRestrictionsResult>>>(new
            {
                Message = new ServiceResult<UidsRestrictionsResult>
                {
                    StatusCode = HttpStatusCode.OK,
                    Result = new UidsRestrictionsResult()
                }
            }));

        // Act
        var result = await _sut.DisagreeEmpowermentAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Forbidden);
        Assert.That(result.Errors, Is.EqualTo(new List<KeyValuePair<string, string>>
            {
                new ("Forbidden", $"After checking '{message.Uid}' it can't be allowed to disagree this empowerment.")
            }));

        var maskedUid = Regex.Replace(message.Uid, @".{4}$", "****", RegexOptions.None, matchTimeout: TimeSpan.FromMilliseconds(100));
        var errorMessage = string.Format("{0} has restrictions. Denying declaring disagreement with empowerment {1}.",
            maskedUid, message.EmpowermentId);
        _logger.VerifyLogging(errorMessage, LogLevel.Warning, Times.Once());
    }

    [Test]
    public void GetEmpowermentsByDeauAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetEmpowermentsByDeauAsync(null));
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_ProviderValidation_ReturnsBadRequest_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            AuthorizerUid = "8802184852",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            PageIndex = 1,
            PageSize = 10
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithValidData_ReturnsOk_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "8802184852",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledSupplierAndServiceIds_ReturnsFilteredEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithOnBehalfOf_ReturnsFilteredEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            OnBehalfOf = OnBehalfOf.Individual,
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(2));
            Assert.That(2, Is.EqualTo(result?.Result?.Data.Count(x => x.OnBehalfOf == OnBehalfOf.Individual)));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithValidOn_ReturnsExpiredEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "8802184852",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddYears(1)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithValidOn_ReturnsNonActiveEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "8802184852",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 2,
            ProviderId = "2",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithValidOn_ReturnsMultipleStatusEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(2));
            Assert.That(result?.Result?.Data?.First().Status, Is.EqualTo(EmpowermentStatementStatus.Active));
            Assert.That(result?.Result?.Data?.Last().Status, Is.EqualTo(EmpowermentStatementStatus.Denied));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithValidOn_ReturnsNonExpiryDateEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 5,
            ProviderId = "5",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().Status, Is.EqualTo(EmpowermentStatementStatus.Active));
            Assert.That(result?.Result?.Data?.First().ExpiryDate, Is.Null);
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithVolumeOfRepresentation_DoesNotReturnWhenAllAreNotContained_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "8802184852",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            VolumeOfRepresentation = new List<string> { "1", "3" },
            ServiceId = 5,
            ProviderId = "5",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithVolumeOfRepresentation_ReturnsPartiallyContained_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            VolumeOfRepresentation = new List<string> { "1" },
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().VolumeOfRepresentation.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithVolumeOfRepresentation_ReturnsWhenMultipleAreContained_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            VolumeOfRepresentation = new List<string> { "1", "2" },
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().VolumeOfRepresentation.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_CalculatesActiveStatusUntilProvidedUTCHours_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1000,
            ProviderId = "1000",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = new DateTime(1000, 10, 10, 23, 59, 59).ToUniversalTime()
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Active));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_CalculatesDisagreemenStatusAfterProvidedUTCHours_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1000,
            ProviderId = "1000",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = new DateTime(1000, 10, 11, 01, 00, 01).ToUniversalTime()
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.DisagreementDeclared));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_CalculatesActiveStatusOneSecondBeforeDisagreementHappens_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1000,
            ProviderId = "1000",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = new DateTime(1000, 10, 11, 01, 00, 00).ToUniversalTime()
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Active));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithMultipleFilters_ReturnsMultipleFiltered_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            OnBehalfOf = OnBehalfOf.Individual,
            VolumeOfRepresentation = new List<string> { "1", "2" },
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(1));
            Assert.That(result?.Result?.Data?.First().VolumeOfRepresentation.Count(), Is.EqualTo(2));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithValidDate_ReturnsDefaultSortedEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(2));
            Assert.That(result?.Result?.Data?.First().Status, Is.EqualTo(EmpowermentStatementStatus.Active));
            Assert.That(result?.Result?.Data?.Last().Status, Is.EqualTo(EmpowermentStatementStatus.Denied));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsActiveEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "4711183713",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "8802184852",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 1,
            ProviderId = "1",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(2));
            Assert.That(result?.Result?.Data?.First().Status, Is.EqualTo(EmpowermentStatementStatus.Active));
            Assert.That(result?.Result?.Data?.Last().Status, Is.EqualTo(EmpowermentStatementStatus.Denied));
            Assert.That(result?.Result?.Data?.First().CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Active));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_DoesNotReturnNotYetCreatedEmpoermentsEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0206254083",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddDays(15)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsDeniedEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddDays(5)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(3));
            Assert.That(result?.Result?.Data?.First().CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Denied));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsUpcomingEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(3));
            Assert.That(result?.Result?.Data?.Last().CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.UpComing));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsActiveEmpowermentsBeforeWithdrawn_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddDays(10)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(3));
            Assert.That(result?.Result?.Data?.First(x => x.Status == EmpowermentStatementStatus.Withdrawn).CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Active));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsWithdrawnEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddDays(35)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(3));
            Assert.That(result?.Result?.Data?.First(x => x.Status == EmpowermentStatementStatus.Withdrawn).CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Withdrawn));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsDisagreementDeclaresEmpowerments_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddDays(50)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(3));
            Assert.That(result?.Result?.Data?.First(x => x.Status == EmpowermentStatementStatus.DisagreementDeclared).CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.DisagreementDeclared));
        });
    }

    [Test]
    public async Task GetEmpowermentsByDeauAsync_WhenCalledWithStatusOnDate_ReturnsActiveEmpowermentsBeforeDisagreementHappend_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var message = CreateInterface<GetEmpowermentsByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            AuthorizerUid = "0206254083",
            AuthorizerUidType = IdentifierType.EGN,
            EmpoweredUid = "0505219387",
            EmpoweredUidType = IdentifierType.EGN,
            RequesterUid = "8802184852",
            ServiceId = 100,
            ProviderId = "100",
            PageIndex = 1,
            PageSize = 10,
            StatusOn = DateTime.UtcNow.AddDays(10)
        });

        // Act
        var result = await _sut.GetEmpowermentsByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Data?.Count(), Is.EqualTo(3));
            Assert.That(result?.Result?.Data?.First(x => x.Status == EmpowermentStatementStatus.DisagreementDeclared).CalculatedStatusOn, Is.EqualTo(CalculatedEmpowermentStatus.Active));
        });
    }

    [Test]
    public void GetExpiringEmpowermentsAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetExpiringEmpowermentsAsync(null));
    }

    [Test]
    public async Task GetExpiringEmpowermentsAsync_WhenCalledWithNonPositiveDays_ReturnsBadRequest_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        var request = new GetExpiringEmpowermentsRequest
        {
            CorrelationId = Guid.NewGuid(),
            DaysUntilExpiration = 0
        };

        // Act
        var result = await _sut.GetExpiringEmpowermentsAsync(request);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetExpiringEmpowermentsAsync_WhenCalledWithLessDaysThanExpiration_ReturnsEmptyCollection_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        // We have 2 Statements expiring after 100 days
        var request = new GetExpiringEmpowermentsRequest
        {
            CorrelationId = Guid.NewGuid(),
            DaysUntilExpiration = 99
        };

        // Act
        var result = await _sut.GetExpiringEmpowermentsAsync(request);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result?.Result?.Count(), Is.EqualTo(0));
    }

    [Test]
    public async Task GetExpiringEmpowermentsAsync_WhenCalledWithGreaterDaysThanExpiration_ReturnsStatementsWithoutNoExpiryOnes_Async()
    {
        // Arrange
        await SeedEmpowermentStatementsAsync();

        // We have 2 Statements expiring after 100 days
        var request = new GetExpiringEmpowermentsRequest
        {
            CorrelationId = Guid.NewGuid(),
            DaysUntilExpiration = 100
        };

        // Act
        var result = await _sut.GetExpiringEmpowermentsAsync(request);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var allStatementsAreNoExpiry = result?.Result?.All(x => x.ExpiryDate.HasValue);
        Assert.Multiple(() =>
        {
            Assert.That(result?.Result?.Count(), Is.EqualTo(3));
            Assert.That(allStatementsAreNoExpiry, Is.EqualTo(true));
        });
    }

    #region DenyEmpowermentByDeau
    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithActiveEmpowermentAndPredefinedDenialReason_ReturnsOK_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Active").Value,
            ProviderId = "000001",
            DenialReasonComment = "Other",

        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result.Result is Guid, Is.True);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithUnconfirmedEmpowermentAndPredefinedDenialReason_ReturnsOK_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Unconfirmed").Value,
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Guid.TryParse(result.ToString().Trim('{'), out Guid parsedGuid);
        var isResultGuid = Guid.TryParse(parsedGuid.ToString().Trim('}'), out Guid _);
        Assert.That(isResultGuid, Is.True);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithWrongSupplier_ReturnsForbidden_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Active").Value,
            ProviderId = "1111",  //Wrong ProviderId
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithDeniedEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Denied").Value,
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithWithdrawnEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Withdrawn").Value,
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithDisagreementDeclaredEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "DisagreementDeclared").Value,
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithCollectingAuthorizerSignaturesEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "CollectingAuthorizerSignatures").Value,
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithWrongEmpowermentId_ReturnsNotFound_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = Guid.NewGuid(), //It will not be found
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithMissingSupplier_ReturnsBadRequest_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Active").Value,
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    public async Task DenyEmpowermentByDeauAsync_WhenCalledWithMissingEmpowermentId_ReturnsBadRequest_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<DenyEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            ProviderId = "000001",
            DenialReasonComment = "Other",
        });

        // Act
        var result = await _sut.DenyEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }
    #endregion DenyEmpowermentByDeau

    #region ApproveEmpowermentByDeau
    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithUnconfirmedEmpowerment_ReturnsOK_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Unconfirmed").Value,
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result.Result is Guid, Is.True);
    }

    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithWrongSupplier_ReturnsForbidden_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Active").Value,
            ProviderId = "1111"  //Wrong ProviderId
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithActiveEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Active").Value,
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithWithdrawnEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Withdrawn").Value,
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithDisagreementDeclaredEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "DisagreementDeclared").Value,
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithCollectingAuthorizerSignaturesEmpowerment_ReturnsConflict_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "CollectingAuthorizerSignatures").Value,
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.Conflict);
    }

    [Test]
    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithWrongEmpowermentId_ReturnsNotFound_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = Guid.NewGuid(), //It will not be found
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
    }

    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithMissingSupplier_ReturnsBadRequest_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            EmpowermentId = empowermentStatusIdPairs.FirstOrDefault(kvp => kvp.Key == "Active").Value
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }

    public async Task ApproveEmpowermentByDeauAsync_WhenCalledWithMissingEmpowermentId_ReturnsBadRequest_Async()
    {
        // Arrange
        List<KeyValuePair<string, Guid>> empowermentStatusIdPairs = new List<KeyValuePair<string, Guid>>
        {
            new KeyValuePair<string, Guid>("Active", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Unconfirmed", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("CollectingAuthorizerSignatures", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Withdrawn", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("Denied", Guid.NewGuid()),
            new KeyValuePair<string, Guid>("DisagreementDeclared", Guid.NewGuid()),
        };
        await SeedEmpowermentStatementsForValidationByDeauAsync(empowermentStatusIdPairs);

        var message = CreateInterface<ApproveEmpowermentByDeau>(new
        {
            CorrelationId = Guid.NewGuid(),
            ProviderId = "000001"
        });

        // Act
        var result = await _sut.ApproveEmpowermentByDeauAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest);
    }
    #endregion ApproveEmpowermentByDeau

    private static readonly object[] EmpowermentStatementNumberTestCases =
    {
        new object[]
        {
            "PO3/22.02.2024", // PO En
            "РО3", // РО Bg
            false
        },
        new object[]
        {
            "РО3/22.02.2024",
            "RO3",
            false
        },
        new object[]
        {
            "РО3/22.02.2024",
            "РО2",
            false
        },
        new object[]
        {
            "РО3/22.02.2024",
            "2/",
            false
        },
        new object[]
        {
            "РО3/22.02.2024",
            "/11",
            false
        },
        new object[]
        {
            "РО3/22.02.2024",
            ".14",
            false
        },
        new object[]
        {
            "РО3/22.02.2024",
            "РО3",
            true
        },
        new object[]
        {
            "РО4/22.02.2024",
            "ро4",
            true
        },
        new object[]
        {
            "РО3/22.02.2024",
            "22.02",
            true
        },
        new object[]
        {
            "РО33/22.02.2024",
            "РО3",
            true
        },
        new object[]
        {
            "РО22/22.02.2024",
            "2/",
            true
        },
        new object[]
        {
            "РО22/22.02.2024",
            "22",
            true
        },
        new object[]
        {
            "РО22/22.02.2024",
            ".02",
            true
        },
        new object[]
        {
            "РО22/22.02.2024",
            ".20",
            true
        },
        new object[]
        {
            "РО22/22.02.2024",
            "2024",
            true
        },
        new object[]
        {
            "РО22/22.02.2024",
            "22.02",
            true
        },
    };

    private static readonly object[] DisagreeEmpowermentBadRequestMessages =
    {
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                Uid = "8802184852", //Valid EGN
                EmpowermentId = Guid.NewGuid(),
                Reason = "Reason text"
            }),
            "No CorrelationId"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.Empty,
                Uid = "8802184852", //Valid EGN
                EmpowermentId = Guid.NewGuid(),
                Reason = "Reason text"
            }),
            "Empty CorrelationId"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Reason = "Reason text"
            }),
            "No Uid"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uid = "  ",
                EmpowermentId = Guid.NewGuid(),
                Reason = "Reason text"
            }),
            "Empty Uid"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uid = "3148300486", // Invalid EGN
                EmpowermentId = Guid.NewGuid(),
                Reason = "Reason text"
            }),
            "Invalid Uid"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uid = "8802184852", //Valid EGN
                Reason = "Reason text"
            }),
            "No EmpowermentId"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uid = "8802184852", //Valid EGN
                EmpowermentId = Guid.Empty,
                Reason = "Reason text"
            }),
            "Empty EmpowermentId"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uid = "8802184852", //Valid EGN
                EmpowermentId = Guid.NewGuid()
            }),
            "No Reason"
        },
        new object[]
        {
            CreateInterface<DisagreeEmpowerment>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uid = "8802184852", //Valid EGN
                EmpowermentId = Guid.NewGuid(),
                Reason = "     "
            }),
            "Empty Reason"
        },
    };

    private async Task<List<EmpowermentStatement>> SeedEmpowermentStatementsForValidationByDeauAsync(List<KeyValuePair<string, Guid>> empowermentIdStatusPair)
    {
        var dbEmpowermentStatements = new List<EmpowermentStatement>
        {
            new EmpowermentStatement    //Active
            {
                Id = empowermentIdStatusPair.FirstOrDefault(kvp => kvp.Key == "Active").Value,  //Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "ActiveEmpowermentTest1",
                AuthorizerUids = new List<AuthorizerUid>() { new() { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>() { new() { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 1,
                ServiceName = "TestServiceName1",
                ProviderId = "000001",
                ProviderName = "TestProviderName000001",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Active,
                CreatedBy = "TestCreatedBy1",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation1",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>() {new VolumeOfRepresentation
                    { Name = "1"}, new VolumeOfRepresentation { Name = "2" } },
                StatusHistory = new List<StatusHistoryRecord>(){ new StatusHistoryRecord
                    { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active } }
            },
            new EmpowermentStatement    //Unconfirmed
            {
                Id = empowermentIdStatusPair.FirstOrDefault(kvp => kvp.Key == "Unconfirmed").Value,
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "UnconfirmedEmpowermentTest2",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 2,
                ServiceName = "TestServiceName2",
                ProviderId = "000001",
                ProviderName = "TestProviderName000001",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Unconfirmed,
                CreatedBy = "TestCreatedBy2",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation2",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>()
            },
            new EmpowermentStatement    //CollectingAuthorizerSignatures
            {
                Id = empowermentIdStatusPair.FirstOrDefault(kvp => kvp.Key == "CollectingAuthorizerSignatures").Value,
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "CollectingAuthorizerSignaturesEmpowermentTest3",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 3,
                ServiceName = "TestServiceName3",
                ProviderId = "000001",
                ProviderName = "TestProviderName000001",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.CollectingAuthorizerSignatures,
                CreatedBy = "TestCreatedBy3",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation3",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>()
            },
            new EmpowermentStatement // Withdrawn
            {
                Id = empowermentIdStatusPair.FirstOrDefault(kvp => kvp.Key == "Withdrawn").Value,
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "WithdrawnEmpowermentTest4",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                ServiceId = 100,
                ServiceName = "TestServiceName4",
                ProviderId = "000001",
                ProviderName = "TestProviderName000001",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Withdrawn,
                CreatedBy = "TestCreatedBy4",
                StartDate = DateTime.UtcNow.AddDays(10),
                XMLRepresentation = "TestXMLRepresentation4",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20), Status = EmpowermentWithdrawalStatus.Completed } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(15), Status = EmpowermentStatementStatus.Withdrawn }
                }
            },
            new EmpowermentStatement // Denied
            {
                Id = empowermentIdStatusPair.FirstOrDefault(kvp => kvp.Key == "Denied").Value,
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "DeniedEmpowermentTest5",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                ServiceId = 100,
                ServiceName = "TestServiceName5",
                ProviderId = "000001",
                ProviderName = "TestProviderName000001",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Denied,
                CreatedBy = "TestCreatedBy5",
                StartDate = DateTime.UtcNow.AddDays(10),
                XMLRepresentation = "TestXMLRepresentation5",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20) } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                }
            },
            new EmpowermentStatement // DisagreementDeclared
            {
                Id = empowermentIdStatusPair.FirstOrDefault(kvp => kvp.Key == "DisagreementDeclared").Value,
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "DisagreementDeclaredEmpowermentTest6",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                ServiceId = 100,
                ServiceName = "TestServiceName6",
                ProviderId = "000001",
                ProviderName = "TestProviderName000001",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.DisagreementDeclared,
                CreatedBy = "TestCreatedBy6",
                StartDate = DateTime.UtcNow.AddDays(10),
                XMLRepresentation = "TestXMLRepresentation6",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentDisagreements = new List<EmpowermentDisagreement>(){ new EmpowermentDisagreement { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(30) } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(15), Status = EmpowermentStatementStatus.DisagreementDeclared }
                }
            },
        };

        await _dbContext.EmpowermentStatements.AddRangeAsync(dbEmpowermentStatements);
        await _dbContext.SaveChangesAsync();

        return dbEmpowermentStatements;
    }

    private async Task<List<EmpowermentDisagreementReason>> SeedEmpowermentDisagreementReasonListAsync()
    {
        var list = new List<EmpowermentDisagreementReason>
        {
            new EmpowermentDisagreementReason
            {
                Id = Guid.NewGuid(),
                Translations = new List<EmpowermentDisagreementReasonTranslation>
                {
                    new EmpowermentDisagreementReasonTranslation
                    {
                        Language = "bg",
                        Name = "тест1bg"
                    },
                    new EmpowermentDisagreementReasonTranslation
                    {
                        Language = "en",
                        Name = "test1en"
                    }
                }
            },
            new EmpowermentDisagreementReason
            {
                Id = Guid.NewGuid(),
                Translations = new List<EmpowermentDisagreementReasonTranslation>
                {
                    new EmpowermentDisagreementReasonTranslation
                    {
                        Language = "bg",
                        Name = "тест2bg"
                    },
                    new EmpowermentDisagreementReasonTranslation
                    {
                        Language = "en",
                        Name = "test2en"
                    }
                }
            }
        };

        await _dbContext.EmpowermentDisagreementReasons.AddRangeAsync(list);
        await _dbContext.SaveChangesAsync();

        return list;
    }

    private async Task<List<EmpowermentStatement>> SeedEmpowermentStatementsAsync()
    {
        var dbEmpowermentStatements = new List<EmpowermentStatement>
        {
            new EmpowermentStatement
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName1",
                AuthorizerUids = new List<AuthorizerUid>() { new() { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>() { new() { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 1,
                ServiceName = "TestServiceName1",
                ProviderId = "1",
                ProviderName = "TestProviderName1",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Active,
                CreatedBy = "TestCreatedBy1",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation1",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>() {new VolumeOfRepresentation
                    { Name = "1"}, new VolumeOfRepresentation { Name = "2" } },
                EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal
                    { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(2) } },
                StatusHistory = new List<StatusHistoryRecord>(){ new StatusHistoryRecord
                    { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active } }
            },
            new EmpowermentStatement
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName2",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 2,
                ServiceName = "TestServiceName2",
                ProviderId = "2",
                ProviderName = "TestProviderName2",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Created,
                CreatedBy = "TestCreatedBy2",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation2",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>()
            },
            new EmpowermentStatement
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName4",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 1,
                ServiceName = "TestServiceName4",
                ProviderId = "1",
                ProviderName = "TestProviderName4",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Denied,
                CreatedBy = "TestCreatedBy4",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation4",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                }
            },
            new EmpowermentStatement
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName5",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 5,
                ServiceName = "TestServiceName5",
                ProviderId = "5",
                ProviderName = "TestProviderName5",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Active,
                CreatedBy = "TestCreatedBy5",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation5",
                ExpiryDate = null,
                VolumeOfRepresentation = new List<VolumeOfRepresentation>()
            },
            new EmpowermentStatement
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName6",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 666,
                ServiceName = "TestServiceName6",
                ProviderId = "6",
                ProviderName = "TestProviderName6",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Active,
                CreatedBy = "TestCreatedBy6",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation6",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>()
            },
            new EmpowermentStatement // Expired
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName7",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow,
                ServiceId = 777,
                ServiceName = "TestServiceName7",
                ProviderId = "7",
                ProviderName = "TestProviderName7",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Active,
                CreatedBy = "TestCreatedBy7",
                StartDate = DateTime.UtcNow.AddDays(-100),
                XMLRepresentation = "TestXMLRepresentation7",
                ExpiryDate = DateTime.UtcNow.AddDays(-1),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>()
            },
            new EmpowermentStatement // Withdrawn
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName100",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                ServiceId = 100,
                ServiceName = "TestServiceName100",
                ProviderId = "100",
                ProviderName = "TestProviderName100",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Withdrawn,
                CreatedBy = "TestCreatedBy100",
                StartDate = DateTime.UtcNow.AddDays(10),
                XMLRepresentation = "TestXMLRepresentation100",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20), Status = EmpowermentWithdrawalStatus.Completed } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(15), Status = EmpowermentStatementStatus.Withdrawn }
                }
            },
            new EmpowermentStatement // Denied
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName100",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                ServiceId = 100,
                ServiceName = "TestServiceName100",
                ProviderId = "100",
                ProviderName = "TestProviderName100",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.Denied,
                CreatedBy = "TestCreatedBy100",
                StartDate = DateTime.UtcNow.AddDays(10),
                XMLRepresentation = "TestXMLRepresentation100",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20) } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                }
            },
            new EmpowermentStatement // DisagreementDeclared
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName100",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-5),
                ServiceId = 100,
                ServiceName = "TestServiceName100",
                ProviderId = "100",
                ProviderName = "TestProviderName100",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.DisagreementDeclared,
                CreatedBy = "TestCreatedBy100",
                StartDate = DateTime.UtcNow.AddDays(10),
                XMLRepresentation = "TestXMLRepresentation100",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentDisagreements = new List<EmpowermentDisagreement>(){ new EmpowermentDisagreement { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(30) } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow.AddDays(15), Status = EmpowermentStatementStatus.DisagreementDeclared }
                }
            },
            new EmpowermentStatement // DisagreementDeclared
            {
                Id = Guid.NewGuid(),
                Uid = "8802184852",
                UidType = IdentifierType.EGN,
                Name = "TestName100",
                AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387", UidType = IdentifierType.EGN } },
                CreatedOn = new DateTime(1000,10,10,01,00,00).ToUniversalTime().ToUniversalTime(),
                ServiceId = 1000,
                ServiceName = "TestServiceName1000",
                ProviderId = "1000",
                ProviderName = "TestProviderName1000",
                OnBehalfOf = OnBehalfOf.Individual,
                Status =  EmpowermentStatementStatus.DisagreementDeclared,
                CreatedBy = "TestCreatedBy1000",
                StartDate = new DateTime(1000,10,10,00,00,00).ToUniversalTime(),
                XMLRepresentation = "TestXMLRepresentation1000",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                EmpowermentDisagreements = new List<EmpowermentDisagreement>(){ new EmpowermentDisagreement { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(30) } },
                StatusHistory = new List<StatusHistoryRecord>()
                {
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = new DateTime(1000,10,10,01,00,00).ToUniversalTime(), Status = EmpowermentStatementStatus.Created },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = new DateTime(1000,10,10,01,00,01).ToUniversalTime(), Status = EmpowermentStatementStatus.CollectingAuthorizerSignatures },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = new DateTime(1000,10,10,01,00,02).ToUniversalTime(), Status = EmpowermentStatementStatus.Active },
                    new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = new DateTime(1000,10,11,01,00,00).ToUniversalTime(), Status = EmpowermentStatementStatus.DisagreementDeclared }
                }
            },
            new EmpowermentStatement // Active LegalEntity
            {
                Id = Guid.NewGuid(),
                Uid = "762640804",
                UidType = IdentifierType.NotSpecified,
                Name = "LegalEntity22",
                AuthorizerUids = new List<AuthorizerUid>() { new() { Id = Guid.NewGuid(), Uid = "2105251412", UidType = IdentifierType.EGN } },
                EmpoweredUids = new List<EmpoweredUid>() { new() { Id = Guid.NewGuid(), Uid = "6002016774", UidType = IdentifierType.EGN } },
                CreatedOn = DateTime.UtcNow.AddDays(-2),
                ServiceId = 1,
                ServiceName = "TestServiceName1",
                ProviderId = "1",
                ProviderName = "TestProviderName1",
                OnBehalfOf = OnBehalfOf.LegalEntity,
                Status =  EmpowermentStatementStatus.Active,
                CreatedBy = "TestCreatedBy1",
                StartDate = DateTime.UtcNow.AddDays(-1),
                XMLRepresentation = "TestXMLRepresentation1",
                ExpiryDate = DateTime.UtcNow.AddDays(100),
                VolumeOfRepresentation = new List<VolumeOfRepresentation>() {new VolumeOfRepresentation
                    { Name = "1"}, new VolumeOfRepresentation { Name = "2" } },
                EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(),
                StatusHistory = new List<StatusHistoryRecord>(){ new StatusHistoryRecord
                    { Id = Guid.NewGuid(), DateTime = DateTime.UtcNow, Status = EmpowermentStatementStatus.Active } }
            }
        };

        await _dbContext.EmpowermentStatements.AddRangeAsync(dbEmpowermentStatements);
        await _dbContext.SaveChangesAsync();

        return dbEmpowermentStatements;
    }
}
