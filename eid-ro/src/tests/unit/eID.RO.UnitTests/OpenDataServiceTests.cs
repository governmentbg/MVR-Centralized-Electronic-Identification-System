using System.Net;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service;
using eID.RO.Service.Entities;
using eID.RO.Service.Interfaces;
using eID.RO.UnitTests.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace eID.RO.UnitTests;

internal class OpenDataServiceTests : BaseTest
{
    private ILogger<OpenDataService> _logger;
    private IDistributedCache _cache;
    private TestApplicationDbContext _dbContext;
    private OpenDataService _sut;
    private FakeDateTimeProvider _fakeDateTimeProvider;

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<OpenDataService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        _dbContext = GetTestDbContext();

        _fakeDateTimeProvider = new FakeDateTimeProvider(new DateTime(2023, 10, 18));

        _sut = new OpenDataService(_logger, _cache, _dbContext, _fakeDateTimeProvider);
    }

    [Test]
    public void GetEmpowermentsByYearAsync_WhenCalledWithNull_ThrowsArgumentNullException()
    {
        //Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetActivatedEmpowermentsByYearAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(GetEmpowermentsByYearCommandInvalidDataTestCases))]
    public async Task GetEmpowermentsByYearAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync(GetActivatedEmpowermentsByYear command, string caseName)
    {
        // Act
        var result = await _sut.GetActivatedEmpowermentsByYearAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task GetEmpowermentsByYearAsync_WhenCalledWithJanuaryMonth_ShouldReturnPreviousYearAsync()
    {
        // Arrange
        _fakeDateTimeProvider.UtcNow = new DateTime(DateTime.UtcNow.Year, 1, 1);
        var message = CreateInterface<GetActivatedEmpowermentsByYear>(new
        {
            CorrelationId = Guid.NewGuid(),
            DateTime.UtcNow.Year
        });
        var lastYear = DateTime.UtcNow.AddYears(-1).Year;
        var dateOld20210115 = new DateTime(lastYear - 1, 01, 15).ToUniversalTime();
        var date20220115 = new DateTime(lastYear, 01, 15).ToUniversalTime(); // 1 
        var date20220203 = new DateTime(lastYear, 02, 03).ToUniversalTime(); // 1
        var date20220518 = new DateTime(lastYear, 05, 18).ToUniversalTime(); // 2
        var date20221222 = new DateTime(lastYear, 12, 22).ToUniversalTime(); // 1
        var date20221231 = new DateTime(lastYear, 12, 31).ToUniversalTime(); // 1
        var dbEmpowermentStatements = new List<EmpowermentStatement>
            {
                // OK, 2022 01 15
                new EmpowermentStatement
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1",
                    CreatedOn = date20220115.AddDays(-10),
                    CreatedBy = "TestCreatedBy1",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    ServiceId = 115,
                    ServiceName = "TestServiceName0115",
                    ProviderId = "1",
                    ProviderName = "TestProviderName1",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Active,
                    StartDate = date20220115.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation1",
                    ExpiryDate = date20220115.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>()
                    {
                        new VolumeOfRepresentation { Name = "Name1"},
                        new VolumeOfRepresentation { Name = "Name2" }
                    },
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20220115.AddDays(2) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220115, Status = EmpowermentStatementStatus.Active }
                    }
                },
                // Not OK. It doesn't have StatusHistoryRecord
                new EmpowermentStatement
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName2",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    CreatedOn = date20220115,
                    ServiceId = 2,
                    ServiceName = "TestServiceName2",
                    ProviderId = "2",
                    ProviderName = "TestProviderName2",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Created,
                    CreatedBy = "TestCreatedBy2",
                    StartDate = date20220115.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation2",
                    ExpiryDate = date20220115.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>()
                },
                // OK 2022 02 03
                new EmpowermentStatement
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName4",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    CreatedOn = date20220203,
                    ServiceId = 203,
                    ServiceName = "TestServiceName0203",
                    ProviderId = "1",
                    ProviderName = "TestProviderName4",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy4",
                    StartDate = date20220203.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation4",
                    ExpiryDate = date20220203.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220203, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220203.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // Not OK old dateOld20210115
                new EmpowermentStatement // Expired
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName7",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    CreatedOn = dateOld20210115,
                    ServiceId = 777,
                    ServiceName = "TestServiceName7",
                    ProviderId = "7",
                    ProviderName = "TestProviderName7",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Active,
                    CreatedBy = "TestCreatedBy7",
                    StartDate = dateOld20210115.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation7",
                    ExpiryDate = dateOld20210115.AddDays(-1),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = dateOld20210115, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = dateOld20210115.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // OK 2022 05 18
                new EmpowermentStatement // Withdrawn
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName100",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20220518.AddDays(-5),
                    ServiceId = 518,
                    ServiceName = "TestServiceName0508",
                    ProviderId = "100",
                    ProviderName = "TestProviderName100",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Withdrawn,
                    CreatedBy = "TestCreatedBy100",
                    StartDate = date20220518.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation100",
                    ExpiryDate = date20220518.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20), Status = EmpowermentWithdrawalStatus.Completed } },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518.AddDays(15), Status = EmpowermentStatementStatus.Withdrawn }
                    }
                },
                // OK 2022 05 18 The same as previous
                new EmpowermentStatement // Denied
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName100",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20220518.AddDays(-5),
                    ServiceId = 518,
                    ServiceName = "TestServiceName0508",
                    ProviderId = "100",
                    ProviderName = "TestProviderName100",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy100",
                    StartDate = date20220518.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation100",
                    ExpiryDate = date20220518.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20) } },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // OK 2022 12 22
                new EmpowermentStatement // Active
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1222",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20221222.AddDays(-5),
                    ServiceId = 1222,
                    ServiceName = "TestServiceName1222",
                    ProviderId = "1222",
                    ProviderName = "TestProviderName1222",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy1222",
                    StartDate = date20221222.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation1222",
                    ExpiryDate = date20221222.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20221222.AddDays(20) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // OK 2022 12 22 but different service name
                new EmpowermentStatement // Active
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1222",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20221222.AddDays(-5),
                    ServiceId = 1222,
                    ServiceName = "TestServiceName1222_2",
                    ProviderId = "1222",
                    ProviderName = "TestProviderName1222",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy1222",
                    StartDate = date20221222.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation1222",
                    ExpiryDate = date20221222.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20221222.AddDays(20) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },                
                // OK 2022 12 31
                new EmpowermentStatement // Active
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1231",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20221231.AddDays(-5),
                    ServiceId = 1231,
                    ServiceName = "TestServiceName1231",
                    ProviderId = "1231",
                    ProviderName = "TestProviderName1231",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy1231",
                    StartDate = date20221231.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation1231",
                    ExpiryDate = date20221231.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20221231.AddDays(20) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221231, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221231.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                }
            };

        await _dbContext.EmpowermentStatements.AddRangeAsync(dbEmpowermentStatements);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetActivatedEmpowermentsByYearAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var data = result.Result ?? new OpenDataResult();
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Count.EqualTo(7));
            // data[0] is header
            Assert.That(data[1][0], Is.EqualTo(115));
            Assert.That(data[1][1], Is.EqualTo("TestServiceName0115"));
            Assert.That(data[1][1/* month*/ + 2 /* serviceId, serviceName */ - 1 /* Positions starts form 0*/], Is.EqualTo(1));

            Assert.That(data[2][0], Is.EqualTo(203));
            Assert.That(data[2][1], Is.EqualTo("TestServiceName0203"));
            Assert.That(data[2][2 + 2 - 1], Is.EqualTo(1));

            Assert.That(data[3][0], Is.EqualTo(518));
            Assert.That(data[3][1], Is.EqualTo("TestServiceName0508"));
            Assert.That(data[3][5 + 2 - 1], Is.EqualTo(2));

            Assert.That(data[4][0], Is.EqualTo(1222));
            Assert.That(data[4][1], Is.EqualTo("TestServiceName1222"));
            Assert.That(data[4][12 + 2 - 1], Is.EqualTo(1));

            Assert.That(data[5][0], Is.EqualTo(1222));
            Assert.That(data[5][1], Is.EqualTo("TestServiceName1222_2"));
            Assert.That(data[5][12 + 2 - 1], Is.EqualTo(1));

            Assert.That(data[6][0], Is.EqualTo(1231));
            Assert.That(data[6][1], Is.EqualTo("TestServiceName1231"));
            Assert.That(data[6][12 + 2 - 1], Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetEmpowermentsByYearAsync_WhenCalledWithLastYear_ShouldReturnOKAsync()
    {
        // Arrange
        _fakeDateTimeProvider.UtcNow = new DateTime(2023, 12, 31);
        var message = CreateInterface<GetActivatedEmpowermentsByYear>(new
        {
            CorrelationId = Guid.NewGuid(),
            _fakeDateTimeProvider.UtcNow.AddYears(-1).Year // 2022 12 31
        });

        var dateOld20210115 = new DateTime(2021, 01, 15).ToUniversalTime();
        var date20220115 = new DateTime(2022, 01, 15).ToUniversalTime(); // 1 
        var date20220203 = new DateTime(2022, 02, 03).ToUniversalTime(); // 1
        var date20220518 = new DateTime(2022, 05, 18).ToUniversalTime(); // 2
        var date20221222 = new DateTime(2022, 12, 22).ToUniversalTime(); // 1
        var date20221231 = new DateTime(2022, 12, 31).ToUniversalTime(); // 1
        var dbEmpowermentStatements = new List<EmpowermentStatement>
            {
                // OK, 2022 01 15
                new EmpowermentStatement
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1",
                    CreatedOn = date20220115.AddDays(-10),
                    CreatedBy = "TestCreatedBy1",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    ServiceId = 115,
                    ServiceName = "TestServiceName0115",
                    ProviderId = "1",
                    ProviderName = "TestProviderName1",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Active,
                    StartDate = date20220115.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation1",
                    ExpiryDate = date20220115.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>()
                    {
                        new VolumeOfRepresentation { Name = "Name1"},
                        new VolumeOfRepresentation { Name = "Name2" }
                    },
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20220115.AddDays(2) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220115, Status = EmpowermentStatementStatus.Active }
                    }
                },
                // Not OK. It doesn't have StatusHistoryRecord
                new EmpowermentStatement
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName2",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    CreatedOn = date20220115,
                    ServiceId = 2,
                    ServiceName = "TestServiceName2",
                    ProviderId = "2",
                    ProviderName = "TestProviderName2",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Created,
                    CreatedBy = "TestCreatedBy2",
                    StartDate = date20220115.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation2",
                    ExpiryDate = date20220115.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>()
                },
                // OK 2022 02 03
                new EmpowermentStatement
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName4",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    CreatedOn = date20220203,
                    ServiceId = 203,
                    ServiceName = "TestServiceName0203",
                    ProviderId = "1",
                    ProviderName = "TestProviderName4",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy4",
                    StartDate = date20220203.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation4",
                    ExpiryDate = date20220203.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220203, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220203.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // Not OK old dateOld20210115
                new EmpowermentStatement // Expired
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName7",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "4711183713" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "8802184852" } },
                    CreatedOn = dateOld20210115,
                    ServiceId = 777,
                    ServiceName = "TestServiceName7",
                    ProviderId = "7",
                    ProviderName = "TestProviderName7",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Active,
                    CreatedBy = "TestCreatedBy7",
                    StartDate = dateOld20210115.AddDays(-100),
                    XMLRepresentation = "TestXMLRepresentation7",
                    ExpiryDate = dateOld20210115.AddDays(-1),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = dateOld20210115, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = dateOld20210115.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // OK 2022 05 18
                new EmpowermentStatement // Withdrawn
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName100",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20220518.AddDays(-5),
                    ServiceId = 518,
                    ServiceName = "TestServiceName0508",
                    ProviderId = "100",
                    ProviderName = "TestProviderName100",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Withdrawn,
                    CreatedBy = "TestCreatedBy100",
                    StartDate = date20220518.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation100",
                    ExpiryDate = date20220518.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20), Status = EmpowermentWithdrawalStatus.Completed } },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518.AddDays(15), Status = EmpowermentStatementStatus.Withdrawn }
                    }
                },
                // OK 2022 05 18 The same as previous
                new EmpowermentStatement // Denied
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName100",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20220518.AddDays(-5),
                    ServiceId = 518,
                    ServiceName = "TestServiceName0508",
                    ProviderId = "100",
                    ProviderName = "TestProviderName100",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy100",
                    StartDate = date20220518.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation100",
                    ExpiryDate = date20220518.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>(){ new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = DateTime.UtcNow.AddDays(20) } },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20220518.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // OK 2022 12 22
                new EmpowermentStatement // Active
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1222",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20221222.AddDays(-5),
                    ServiceId = 1222,
                    ServiceName = "TestServiceName1222",
                    ProviderId = "1222",
                    ProviderName = "TestProviderName1222",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy1222",
                    StartDate = date20221222.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation1222",
                    ExpiryDate = date20221222.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    { 
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20221222.AddDays(20) } 
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },
                // OK 2022 12 22 but different service name
                new EmpowermentStatement // Active
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1222",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20221222.AddDays(-5),
                    ServiceId = 1222,
                    ServiceName = "TestServiceName1222_2",
                    ProviderId = "1222",
                    ProviderName = "TestProviderName1222",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy1222",
                    StartDate = date20221222.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation1222",
                    ExpiryDate = date20221222.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20221222.AddDays(20) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221222.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                },                
                // OK 2022 12 31
                new EmpowermentStatement // Active
                {
                    Id = Guid.NewGuid(),
                    Uid = "8802184852",
                    Name = "TestName1231",
                    AuthorizerUids = new List<AuthorizerUid>() { new AuthorizerUid { Id = Guid.NewGuid(), Uid = "0206254083" } },
                    EmpoweredUids = new List<EmpoweredUid>(){ new EmpoweredUid { Id = Guid.NewGuid(), Uid = "0505219387" } },
                    CreatedOn = date20221231.AddDays(-5),
                    ServiceId = 1231,
                    ServiceName = "TestServiceName1231",
                    ProviderId = "1231",
                    ProviderName = "TestProviderName1231",
                    OnBehalfOf = OnBehalfOf.Individual,
                    Status =  EmpowermentStatementStatus.Denied,
                    CreatedBy = "TestCreatedBy1231",
                    StartDate = date20221231.AddDays(10),
                    XMLRepresentation = "TestXMLRepresentation1231",
                    ExpiryDate = date20221231.AddDays(100),
                    VolumeOfRepresentation = new List<VolumeOfRepresentation>(),
                    EmpowermentWithdrawals = new List<EmpowermentWithdrawal>()
                    {
                        new EmpowermentWithdrawal { Id = Guid.NewGuid(), Reason = "Reason 1", ActiveDateTime = date20221231.AddDays(20) }
                    },
                    StatusHistory = new List<StatusHistoryRecord>()
                    {
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221231, Status = EmpowermentStatementStatus.Active },
                        new StatusHistoryRecord { Id = Guid.NewGuid(), DateTime = date20221231.AddDays(5), Status = EmpowermentStatementStatus.Denied }
                    }
                }
            };

        await _dbContext.EmpowermentStatements.AddRangeAsync(dbEmpowermentStatements);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetActivatedEmpowermentsByYearAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        var data = result.Result ?? new OpenDataResult();
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Count.EqualTo(7));
            // data[0] is header
            Assert.That(data[1][0], Is.EqualTo(115));
            Assert.That(data[1][1], Is.EqualTo("TestServiceName0115"));
            Assert.That(data[1][1/* month*/ + 2 /* serviceId, serviceName */ - 1 /* Positions starts form 0*/], Is.EqualTo(1));

            Assert.That(data[2][0], Is.EqualTo(203));
            Assert.That(data[2][1], Is.EqualTo("TestServiceName0203"));
            Assert.That(data[2][2 + 2 - 1], Is.EqualTo(1));

            Assert.That(data[3][0], Is.EqualTo(518));
            Assert.That(data[3][1], Is.EqualTo("TestServiceName0508"));
            Assert.That(data[3][5 + 2 - 1], Is.EqualTo(2));

            Assert.That(data[4][0], Is.EqualTo(1222));
            Assert.That(data[4][1], Is.EqualTo("TestServiceName1222"));
            Assert.That(data[4][12 + 2 - 1], Is.EqualTo(1));

            Assert.That(data[5][0], Is.EqualTo(1222));
            Assert.That(data[5][1], Is.EqualTo("TestServiceName1222_2"));
            Assert.That(data[5][12 + 2 - 1], Is.EqualTo(1));

            Assert.That(data[6][0], Is.EqualTo(1231));
            Assert.That(data[6][1], Is.EqualTo("TestServiceName1231"));
            Assert.That(data[6][12 + 2 - 1], Is.EqualTo(1));
        });
    }

    private static readonly object[] GetEmpowermentsByYearCommandInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<GetActivatedEmpowermentsByYear>(new
            {
                CorrelationId = Guid.Empty,
            }),
            "CorrelationId is Empty"
        },
        new object[]
        {
            CreateInterface<GetActivatedEmpowermentsByYear>(new
            {
                CorrelationId = Guid.NewGuid(),
                Year = 0
            }),
            "Year is 0"
        },
        new object[]
        {
            CreateInterface<GetActivatedEmpowermentsByYear>(new
            {
                CorrelationId = Guid.NewGuid(),
                Year = -1
            }),
            "Year is -1"
        },
        new object[]
        {
            CreateInterface<GetActivatedEmpowermentsByYear>(new
            {
                CorrelationId = Guid.NewGuid(),
                Year = int.MinValue
            }),
            "Year is int.MinValue"
        }
    };
}

internal class FakeDateTimeProvider : IDateTimeProvider
{
    private DateTime _dateTime;

    public DateTime UtcNow
    {
        get { return _dateTime; }
        set { _dateTime = value; }
    }

    public FakeDateTimeProvider(DateTime dateTime)
    {
        _dateTime = dateTime;
    }
}
