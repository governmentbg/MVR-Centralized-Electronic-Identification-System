using System.Net;
using eID.POD.Contracts.Commands;
using eID.POD.Contracts.Results;
using eID.POD.Service;
using eID.POD.Service.Database;
using eID.POD.Service.Entities;
using eID.POD.Service.Options;
using eID.POD.UnitTests.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Quartz;

namespace eID.POD.UnitTests;

public class DatasetsServiceTests : BaseTest
{
    private Mock<ILogger<DatasetsService>> _logger;
    private ApplicationDbContext _dbContext;
    private DatasetsService _sut;
    private Mock<IOptions<OpenDataSettings>> _openDataSettings;
    private Mock<ISchedulerFactory> _schedulerFactory;

    [SetUp]
    public void Init()
    {
        _logger = new Mock<ILogger<DatasetsService>>();
        _openDataSettings = new Mock<IOptions<OpenDataSettings>>();
        _openDataSettings.Setup(x => x.Value).Returns(new OpenDataSettings { AutomaticStart = false, CategoryId = 1, OpenDataApiKey = "forTest", OpenDataUrl = "https://forTest.test", OrganizationId = 1 });
        _schedulerFactory = new Mock<ISchedulerFactory>();
        _schedulerFactory.Setup(f => f.GetScheduler(It.IsAny<CancellationToken>())).ReturnsAsync(new Mock<IScheduler>().Object);
        _dbContext = GetTestDbContext();
        var mockHttpClientFactory = new Mock<IHttpClientFactory>();
        _sut = new DatasetsService(_logger.Object, mockHttpClientFactory.Object, _openDataSettings.Object, _dbContext, _schedulerFactory.Object);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void CreateDatasetAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateDatasetAsync(null));
    }

    [Test]
    public async Task CreateDatasetAsync_WithGivenData_ReturnsSingleIdAsync()
    {
        // Arrange
        var datasetName = "Test";
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = datasetName,
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "https://google.com",
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.Created);
        var result = serviceResult.Result;

        var createdDataset = _dbContext.Datasets.FirstOrDefault(x => x.Id == result);

        Assert.Multiple(() =>
        {
            Assert.That(createdDataset?.DatasetName, Is.EqualTo(datasetName));
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf(typeof(Guid)));
        });
    }

    [Test]
    public async Task CreateDatasetAsync_WithGivenEmptyDatasetName_ShouldReturnBadRequest()
    {
        // Arrange
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = string.Empty,
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "https://google.com",
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateDatasetAsync_WithInvalidCronPeriod_ShouldReturnBadRequest()
    {
        // Arrange
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = "test",
            CronPeriod = "InvalidCronExpression",
            DataSource = "https://google.com",
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateDatasetAsync_WithEmptyDataSource_ShouldReturnBadRequest()
    {
        // Arrange
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = "test",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = string.Empty,
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateDatasetAsync_WithInvalidAbsoluteUrlAsDataSource_ShouldReturnBadRequest()
    {
        // Arrange
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = "test",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "invalid url",
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateDatasetAsync_WithRelativeUrlAsDataSource_ShouldReturnBadRequest()
    {
        // Arrange
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = "test",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "/test/something",
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateDatasetAsync_WithEmptyCreatedBy_ShouldReturnBadRequest()
    {
        // Arrange
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = "test",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "/test/something",
            CreatedBy = string.Empty,
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CreateDatasetAsync_WithGivenData_CreatesDatasetWithEmptyDatasetUriAsync()
    {
        // Arrange
        var datasetName = "Test";
        var createDataset = CreateInterface<CreateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            DatasetName = datasetName,
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "https://google.com",
            CreatedBy = "test",
            IsActive = true
        });

        // Act
        var serviceResult = await _sut.CreateDatasetAsync(createDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.Created);
        var result = serviceResult.Result;

        var createdDataset = _dbContext.Datasets.FirstOrDefault(x => x.Id == result);

        Assert.Multiple(() =>
        {
            Assert.That(createdDataset?.DatasetName, Is.EqualTo(datasetName));
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf(typeof(Guid)));
            Assert.That(createdDataset?.DatasetUri, Is.Null);
        });
    }

    [Test]
    public void GetAllDatasets_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.GetAllDatasetsAsync(null));
    }

    [Test]
    public async Task GetAllDatasets_WhenCalled_ReturnsDatasetsSortedDefaultByName()
    {
        // Arrange
        await SeedTestDatasetsAsync();

        var getAllDatasets = CreateInterface<GetAllDatasets>(new
        {
            CorrelationId = Guid.NewGuid(),
        });

        // Act 
        var serviceResult = await _sut.GetAllDatasetsAsync(getAllDatasets);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf(typeof(IEnumerable<Dataset>)));
            Assert.That(result.Last().DatasetName, Is.Not.Null);
            Assert.That(result.First().DatasetName.CompareTo(result.Last().DatasetName), Is.EqualTo(-1));
            Assert.That(result.All(d => d.IsDeleted == false));
        });
    }

    [Test]
    public async Task GetAllDatasets_WhenCalled_ReturnsBothActiveAndNotActiveDatasets()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var getAllDatasets = CreateInterface<GetAllDatasets>(new
        {
            CorrelationId = Guid.NewGuid(),
        });

        // Act 
        var serviceResult = await _sut.GetAllDatasetsAsync(getAllDatasets);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.OK);
        var result = serviceResult.Result;

        Assert.Multiple(() =>
        {
            CheckServiceResult(serviceResult, HttpStatusCode.OK);
            Assert.That(result, Is.Not.Empty);
            Assert.That(result, Is.InstanceOf(typeof(IEnumerable<Dataset>)));
            Assert.That(result.Last().DatasetName, Is.Not.Null);
            Assert.That(result.First().DatasetName.CompareTo(result.Last().DatasetName), Is.EqualTo(-1));
            Assert.That(result.Any(d => d.IsActive == true));
            Assert.That(result.Any(d => d.IsActive == false));
        });
    }

    [Test]
    public async Task GetActiveDatasetsAsync_WhenCalled_ReturnsOnlyActiveDatasets()
    {
        // Arrange
        await SeedTestDatasetsAsync();

        // Act 
        var serviceRequest = await _sut.GetActiveDatasetsAsync();
        // Assert
        Assert.Multiple(() =>
        {
            CheckServiceResult(serviceRequest, HttpStatusCode.OK);
            Assert.That(serviceRequest, Is.InstanceOf(typeof(ServiceResult<IEnumerable<Dataset>>)));
            Assert.That(serviceRequest.Result.All(d => d.IsActive == true));
        });
    }

    [Test]
    public async Task GetActiveDatasetsAsync_DoesNotReturnDeletedDatasets()
    {
        // Arrange
        await SeedTestDatasetsAsync();

        // Act 
        var serviceRequest = await _sut.GetActiveDatasetsAsync();

        // Assert
        Assert.Multiple(() =>
        {
            CheckServiceResult(serviceRequest, HttpStatusCode.OK);
            Assert.That(serviceRequest, Is.InstanceOf(typeof(ServiceResult<IEnumerable<Dataset>>)));
            Assert.That(serviceRequest.Result.All(d => d.IsDeleted != true));
        });
    }

    [Test]
    public void UpdateDatasetAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateDatasetAsync(null));
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledWithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.Empty,
            DatasetName = "Test1",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "https://google.com",
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledWithoutDatasetName_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DatasetName = string.Empty,
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "https://google.com",
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledEmptyCronPeriod_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DatasetName = "Test1",
            CronPeriod = string.Empty,
            DataSource = "https://google.com",
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledInvalidCronPeriod_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DatasetName = "Test1",
            CronPeriod = "Invalid Cron Expession",
            DataSource = "https://google.com",
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledEmptyDataSource_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DatasetName = "Test1",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = string.Empty,
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledInvalidAbsoluteUrlAsDataSource_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DatasetName = "Test1",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "Invalid Absulute Url",
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledWithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            DatasetName = "Test1",
            CronPeriod = "0 0/3 * * * ? *",
            DataSource = "https://test.something",
            IsActive = true,
            LastModifiedBy = "Test1"
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task UpdateDatasetAsync_WhenCalledWithValidData_ReturnsNoContentAndUpdateRecord()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var serviceRequest = await _sut.GetActiveDatasetsAsync();
        var datasets = serviceRequest.Result;
        CheckServiceResult(serviceRequest, HttpStatusCode.OK);
        var initialDataset = datasets.FirstOrDefault();
        var datasetId = initialDataset?.Id;
        Assert.That(datasetId, Is.Not.Empty);
        var updatedName = "Test100";
        var updatedCronPeriod = "0 0/5 * * * ? *";
        var updatedAbsoluteUrl = "https://test.something";
        var lastModifiedBy = "Test100";
        var updateDataset = CreateInterface<UpdateDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = datasetId,
            DatasetName = updatedName,
            CronPeriod = updatedCronPeriod,
            DataSource = updatedAbsoluteUrl,
            IsActive = true,
            LastModifiedBy = lastModifiedBy
        });

        // Act 
        var serviceResult = await _sut.UpdateDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);
        var getDatasetRequest = await _sut.GetDatasetByIdAsync(datasetId.ToString());
        var updatedDataset = getDatasetRequest.Result;

        Assert.Multiple(() =>
        {
            Assert.That(updatedDataset.DatasetName, Is.EqualTo(updatedName));
            Assert.That(updatedDataset.CronPeriod, Is.EqualTo(updatedCronPeriod));
            Assert.That(updatedDataset.DataSource, Is.EqualTo(updatedAbsoluteUrl));
            Assert.That(updatedDataset.LastModifiedBy, Is.EqualTo(lastModifiedBy));
        });
    }

    [Test]
    public void DeleteDatasetAsync_WhenCalledWithNullMessage_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.DeleteDatasetAsync(null));
    }

    [Test]
    public async Task DeleteDatasetAsync_WhenCalledWithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<DeleteDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.Empty,
            LastModifiedBy = "Test100"
        });

        // Act 
        var serviceResult = await _sut.DeleteDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteDatasetAsync_WhenCalledWithEmptyLastModifiedBy_ReturnsBadRequest()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var updateDataset = CreateInterface<DeleteDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            LastModifiedBy = string.Empty
        });

        // Act 
        var serviceResult = await _sut.DeleteDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteDatasetAsync_WhenCalledWithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        await SeedTestDatasetsAsync();

        // Act 
        var updateDataset = CreateInterface<DeleteDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = Guid.NewGuid(),
            LastModifiedBy = "Test100"
        });

        // Act 
        var serviceResult = await _sut.DeleteDatasetAsync(updateDataset);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteDatasetAsync_WhenCalledWithValidData_UpdatesTheRecordAndReturnsNoContent()
    {
        // Arrange
        await SeedTestDatasetsAsync();
        var serviceRequest = await _sut.GetActiveDatasetsAsync();
        var datasets = serviceRequest.Result;
        CheckServiceResult(serviceRequest, HttpStatusCode.OK);
        var initialDataset = datasets.FirstOrDefault();
        var datasetId = initialDataset?.Id;
        Assert.That(datasetId, Is.Not.Empty);
        var lastModifiedBy = "Test100";
        var command = CreateInterface<DeleteDataset>(new
        {
            CorrelationId = Guid.NewGuid(),
            Id = datasetId,
            LastModifiedBy = lastModifiedBy
        });

        // Act 
        var serviceResult = await _sut.DeleteDatasetAsync(command);

        // Assert
        CheckServiceResult(serviceResult, HttpStatusCode.NoContent);
        var deletedDataset = await _dbContext.Datasets.FindAsync(datasetId);

        Assert.Multiple(() =>
        {
            Assert.That(deletedDataset, Is.Not.Null);
            Assert.That(deletedDataset.IsDeleted, Is.True);
            Assert.That(deletedDataset.LastModifiedBy, Is.EqualTo(lastModifiedBy));
        });
    }

    private async Task SeedTestDatasetsAsync()
    {
        // 4 active and 2 deleted
        var datasets = new List<Dataset>
        {
            new Dataset
            {
                Id = Guid.NewGuid(),
                DatasetName = "Test1",
                CronPeriod = "0 0/5 * * * ? *",
                DatasetUri = "ValidDatasetUri",
                DataSource = "ValidAbsoluteUrl2",
                IsActive = true,
                CreatedBy = "TestUser2",
                IsDeleted = false,
                LastModifiedBy = "TestUser2",
                LastRun = null
            },
            new Dataset
            {
                Id = Guid.NewGuid(),
                DatasetName = "Test2",
                CronPeriod = "0 0/5 * * * ? *",
                DatasetUri = "ValidDatasetUri",
                DataSource = "ValidAbsoluteUrl2",
                IsActive = true,
                CreatedBy = "TestUser2",
                IsDeleted = false,
                LastModifiedBy = "TestUser2",
                LastRun = null
            },
            new Dataset
            {
                Id = Guid.NewGuid(),
                DatasetName = "Test3",
                CronPeriod = "0 0/5 * * * ? *",
                DatasetUri = "ValidDatasetUri",
                DataSource = "ValidAbsoluteUrl3",
                IsActive = true,
                CreatedBy = "TestUser3",
                IsDeleted = false,
                LastModifiedBy = "TestUser3",
                LastRun = null
            },
            new Dataset
            {
                Id = Guid.NewGuid(),
                DatasetName = "Test4",
                CronPeriod = "0 0/5 * * * ? *",
                DatasetUri = "ValidDatasetUri",
                DataSource = "ValidAbsoluteUrl4",
                IsActive = false,
                CreatedBy = "TestUser4",
                IsDeleted = false,
                LastModifiedBy = "TestUser4",
                LastRun = null
            },
            new Dataset
            {
                Id = Guid.NewGuid(),
                DatasetName = "Test5",
                CronPeriod = "0 0/5 * * * ? *",
                DatasetUri = "ValidDatasetUri",
                DataSource = "ValidAbsoluteUrl5",
                IsActive = false,
                CreatedBy = "TestUser5",
                IsDeleted = true,
                LastModifiedBy = "TestUser5",
                LastRun = null
            },
            new Dataset
            {
                Id = Guid.NewGuid(),
                DatasetName = "Test6",
                CronPeriod = "0 0/5 * * * ? *",
                DatasetUri = "ValidDatasetUri",
                DataSource = "ValidAbsoluteUrl6",
                IsActive = true,
                CreatedBy = "TestUser6",
                IsDeleted = true,
                LastModifiedBy = "TestUser6",
                LastRun = null
            }
        };

        await _dbContext.Datasets.AddRangeAsync(datasets);
        await _dbContext.SaveChangesAsync();
    }
}
