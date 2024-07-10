using System.Net;
using eID.RO.Contracts.Commands;
using eID.RO.Contracts.Enums;
using eID.RO.Contracts.Results;
using eID.RO.Service;
using eID.RO.Service.Responses;
using eID.RO.UnitTests.Generic;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;

namespace eID.RO.UnitTests;

internal class VerificationServiceTests : BaseTest
{
    private ILogger<VerificationService> _logger;
    private IDistributedCache _cache;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private VerificationService _sut;

    private string _originalFile = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String("77u/PEVtcG93ZXJtZW50U3RhdGVtZW50SXRlbT4NCiAgPElkPmZhYTZlMzcxLWEyYjEtNDE2Yy1hMWNhLTZjYzg0OTJmMDgyNzwvSWQ+DQogIDxPbkJlaGFsZk9mPkluZGl2aWR1YWw8L09uQmVoYWxmT2Y+DQogIDxVaWQ+MDAwMDAwMDAwMDwvVWlkPg0KICA8TmFtZT5UZXN0IFVzZXIgRnVsbCBOYW1lPC9OYW1lPg0KICA8QXV0aG9yaXplclVpZHM+DQogICAgPFVpZD4wMDAwMDAwMDAwPC9VaWQ+DQogIDwvQXV0aG9yaXplclVpZHM+DQogIDxFbXBvd2VyZWRVaWRzPg0KICAgIDxVaWQ+MDAwMDAwMDAwMDwvVWlkPg0KICA8L0VtcG93ZXJlZFVpZHM+DQogIDxTdXBwbGllcklkPjAwMDAwMDAwNjE8L1N1cHBsaWVySWQ+DQogIDxTdXBwbGllck5hbWU+0JzQuNC90LjRgdGC0LXRgNGB0YLQstC+INC90LAg0L/RgNCw0LLQvtGB0YrQtNC40LXRgtC+PC9TdXBwbGllck5hbWU+DQogIDxTZXJ2aWNlSWQ+OTU1PC9TZXJ2aWNlSWQ+DQogIDxTZXJ2aWNlTmFtZT7Qn9GA0L7QtNGK0LvQttCw0LLQsNC90LUg0L3QsCDRgNCw0LfRgNC10YjQtdC90LjQtSDQt9CwINC40LfQstGK0YDRiNCy0LDQvdC1INC90LAg0LTQtdC50L3QvtGB0YIg0YEg0L3QtdGB0YLQvtC/0LDQvdGB0LrQsCDRhtC10Lsg0L7RgiDRh9GD0LbQtNC10L3QtdGGINCyINCg0LXQv9GD0LHQu9C40LrQsCDQkdGK0LvQs9Cw0YDQuNGPPC9TZXJ2aWNlTmFtZT4NCiAgPFR5cGVPZkVtcG93ZXJtZW50PlNlcGFyYXRlbHk8L1R5cGVPZkVtcG93ZXJtZW50Pg0KICA8Vm9sdW1lT2ZSZXByZXNlbnRhdGlvbj4NCiAgICA8SXRlbT4NCiAgICAgIDxDb2RlPjY5PC9Db2RlPg0KICAgICAgPE5hbWU+0JLRgdC40YfQutC+PC9OYW1lPg0KICAgIDwvSXRlbT4NCiAgPC9Wb2x1bWVPZlJlcHJlc2VudGF0aW9uPg0KICA8Q3JlYXRlZE9uPjIwMjMtMDktMThUMTI6MTk6MTguNTg1MTcyMlo8L0NyZWF0ZWRPbj4NCiAgPFN0YXJ0RGF0ZT4yMDIzLTA5LTE3VDIxOjAwOjAwWjwvU3RhcnREYXRlPg0KICA8RXhwaXJ5RGF0ZSBwMjpuaWw9InRydWUiIHhtbG5zOnAyPSJodHRwOi8vd3d3LnczLm9yZy8yMDAxL1hNTFNjaGVtYS1pbnN0YW5jZSIgLz4NCjwvRW1wb3dlcm1lbnRTdGF0ZW1lbnRJdGVtPg=="));
    private string _signature = "MIIJuQYJKoZIhvcNAQcCoIIJqjCCCaYCAQExDzANBglghkgBZQMEAgEFADALBgkqhkiG9w0BBwGgggXaMIIF1jCCBL6gAwIBAgIUMqjqKNGWDRn2j+u0qSjDwVKdEfgwDQYJKoZIhvcNAQELBQAwgagxCzAJBgNVBAYTAkJHMRgwFgYDVQRhEw9OVFJCRy0yMDMzOTczNTYxIzAhBgNVBAoTGkV2cm90cnVzdCBUZWNobm9sb2dpZXMgSlNDMS8wLQYDVQQLEyZGb3IgdGVzdCBhbmQgZGV2ZWxvcG1lbnQgcHVycG9zZXMgb25seTEpMCcGA1UEAxMgRXZyb3RydXN0IERFViBSU0EgT3BlcmF0aW9uYWwgQ0EwHhcNMjMwNzEyMTE0MjM2WhcNMjUwNzExMTE0MjM1WjBwMQswCQYDVQQGEwJCRzEiMCAGA1UEAwwZQUxFS1NBTkRBUiBCT1lLT1YgU1RPSUxPVjETMBEGA1UEKgwKQUxFS1NBTkRBUjEQMA4GA1UEBAwHU1RPSUxPVjEWMBQGA1UEBRMNQTAwMDAwMDA3MjU0NjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAMqclXrGR+hGvdzjdfi26WmmzkjjSMo2EboFOr9kTZZZeqAo4JQ7V98c7r3yqaHBsSoCXIsHUtC4xw95uX50aMrRNikoQEq5uaYLl5LTKht5HcwJsaadQ9lo4r3d3Jtuk1LkoY/mmviB3azGZppjpDbSwiV7nSGOCZ+VGNJarujgY6m24ITeluU2u0o/J4e3jOGKTd03ujfKfMl6Rjakc+NEUuiC/qoFhYKaEF4/ov5hPBwD7tH2s+vOajEdvPPBZBvd+l5ujGpug/ZOrGu7i5FeOHTFk8SXmSOQVpeuvKWJdotyuA/vnA+hrt7RSmuGytudILsZfY5eU0MKV7nd4DUCAwEAAaOCAi0wggIpMIGKBggrBgEFBQcBAwR+MHwwCAYGBACORgEBMBUGBgQAjkYBAjALEwNFVVICAQICAQQwCAYGBACORgEEMDoGBgQAjkYBBTAwMC4WKGh0dHBzOi8vd3d3LmV2cm90cnVzdC5jb20vcGRzL3Bkc19lbi5wZGYTAmVuMBMGBgQAjkYBBjAJBgcEAI5GAQYBMB8GA1UdIwQYMBaAFC2ZcORoVwyOkdSXdEhkSxcOkX4UMIGIBggrBgEFBQcBAQR8MHowSwYIKwYBBQUHMAKGP2h0dHA6Ly9kZXZjYS5ldnJvdHJ1c3QuY29tL2FpYS9FdnJvdHJ1c3RERVZSU0FPcGVyYXRpb25hbENBLmNydDArBggrBgEFBQcwAYYfaHR0cDovL2RldmNhLmV2cm90cnVzdC5jb20vb2NzcDBOBgNVHSAERzBFMAkGBwQAi+xAAQIwOAYKKwYBBAGC8SgCAjAqMCgGCCsGAQUFBwIBFhxodHRwOi8vd3d3LmV2cm90cnVzdC5jb20vY3BzMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcDBDBQBgNVHR8ESTBHMEWgQ6BBhj9odHRwOi8vZGV2Y2EuZXZyb3RydXN0LmNvbS9jcmwvRXZyb3RydXN0REVWUlNBT3BlcmF0aW9uYWxDQS5jcmwwHQYDVR0OBBYEFM74HLuREUG/DsJBpvC8dKnk0WaDMA4GA1UdDwEB/wQEAwIF4DANBgkqhkiG9w0BAQsFAAOCAQEAHpC9uolCYIcZWUpcWb/TGveyFwGAxwu/USJXcQUmk9B+nOaeBXvPCJCNrjDE/Ou8FUgHKNcWC5kCjYgvWdV3wJEx9Mf5ypAm1K9w1Vvcs2S3R77S9uyxxw4zbvWgGFrxsL3kbHif1jLDP4sFBiitquqZog6K3sOzolJqFdRFqQ5V9GiExp7DmJQGlUo5wb+t0//NwTlHv3SJy0N+qECJDhQjgFE/a+MeiMaFw0+Y0DbsWv3Mz9vm2VfbkE7R+ob3uiWICJXgiAAnARbGwySQGWurudt8/K0v8ch22ZuKNIWISXSDobJu9B+x8gSYUabzgm5dQNAuEHUKVwAQgFYqKDGCA6MwggOfAgEBMIHBMIGoMQswCQYDVQQGEwJCRzEYMBYGA1UEYRMPTlRSQkctMjAzMzk3MzU2MSMwIQYDVQQKExpFdnJvdHJ1c3QgVGVjaG5vbG9naWVzIEpTQzEvMC0GA1UECxMmRm9yIHRlc3QgYW5kIGRldmVsb3BtZW50IHB1cnBvc2VzIG9ubHkxKTAnBgNVBAMTIEV2cm90cnVzdCBERVYgUlNBIE9wZXJhdGlvbmFsIENBAhQyqOoo0ZYNGfaP67SpKMPBUp0R+DANBglghkgBZQMEAgEFAKCCAbIwGAYJKoZIhvcNAQkDMQsGCSqGSIb3DQEHATAcBgkqhkiG9w0BCQUxDxcNMjMwOTE5MDY0OTA1WjAtBgkqhkiG9w0BCTQxIDAeMA0GCWCGSAFlAwQCAQUAoQ0GCSqGSIb3DQEBCwUAMC8GCSqGSIb3DQEJBDEiBCC+MMHvZ8UetLNYkug52E7kx67uPrLTodiQwVx4hoEvQDCCARYGCyqGSIb3DQEJEAIvMYIBBTCCAQEwgf4wgfswDQYJYIZIAWUDBAIBBQAEIKkqf/20D/iTHXsrY8FaRzeglYmqpvnz6g26v0tE8B/bMIHHMIGupIGrMIGoMQswCQYDVQQGEwJCRzEYMBYGA1UEYRMPTlRSQkctMjAzMzk3MzU2MSMwIQYDVQQKExpFdnJvdHJ1c3QgVGVjaG5vbG9naWVzIEpTQzEvMC0GA1UECxMmRm9yIHRlc3QgYW5kIGRldmVsb3BtZW50IHB1cnBvc2VzIG9ubHkxKTAnBgNVBAMTIEV2cm90cnVzdCBERVYgUlNBIE9wZXJhdGlvbmFsIENBAhQyqOoo0ZYNGfaP67SpKMPBUp0R+DANBgkqhkiG9w0BAQsFAASCAQAL9O87D8Brq0Jj0IBMrjstc591AxgoYef2eTTEoHOCXN4VUutoszM9EDvRzwMACf04Qc++9v2J2l/+uzSq7NlMChNRiL9m5P2mhaEYCwGrQ1n6ClVjvGuxNVla2PoWkZhoaSQWKKv3OPgH/+Fy2bvaOURCOzRSJ6NHLoVLR6Y48XE1ajSk7XzH8AIdtnj5LFB+HhTzSp8Ue5W6a2//2MRoxnRFDFQ3K0fZyFaiNTBMOPTD3LzrYj2zNWOKO0hl5sfQZfBA5BVwgQ57uYQUKGhev2Y0sXUzPjyCZ5AUDF+aQabr2UtxyMSd+wsHqW04aB605z4nUUtHsiZA6xyPw5xM";

    [SetUp]
    public void Init()
    {
        _logger = new NullLogger<VerificationService>();

        var opts = Options.Create(new MemoryDistributedCacheOptions());
        _cache = new MemoryDistributedCache(opts);

        // Prepare HttpClient
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _httpClient.BaseAddress = new Uri("http://local");

        _sut = new VerificationService(_logger, _cache, _httpClient);
    }

    [Test]
    public void VerifyRequesterInLegalEntityAsync_WhenCalledWithNull_ThrowsArgumentNullException()
    {
        //Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.VerifyRequesterInLegalEntityAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(RequesterCheckInLECommandInvalidDataTestCases))]
    public async Task VerifyRequesterInLegalEntityAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync(CheckLegalEntityInNTR command, string caseName)
    {
        // Act
        var result = await _sut.VerifyRequesterInLegalEntityAsync(command);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Error, Is.Not.Empty);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        });
        //_regix.Verify(x => x.SearchAsync(It.IsAny<RegiXSearchCommand>()), Times.Never);
    }

    [Test]
    public async Task VerifySignatureAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync()
    {
        string originalFile = string.Empty;
        string signature = string.Empty;
        string uid = string.Empty;
        // Act
        var result = await _sut.VerifySignatureAsync(originalFile, signature, uid, IdentifierType.EGN, Contracts.Enums.SignatureProvider.KEP);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Error, Is.Not.Empty);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        });
    }

    [Test]
    public async Task VerifySignatureAsync_ValidEurotrustCertWithoutEGNData_ShouldReturnOKAsync()
    {
        // Arrange
        string uid = "0000000000";
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            });

        // Act
        var result = await _sut.VerifySignatureAsync(_originalFile, _signature, uid, IdentifierType.EGN, Contracts.Enums.SignatureProvider.Evrotrust);

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Error, Is.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public void CheckAuthorizersForRestrictionsAsync_WhenCalledWithNull_ThrowsArgumentNullException()
    {
        //Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CheckUidsRestrictionsAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(CheckAuthorizersForRestrictionsCommandInvalidDataTestCases))]
    public async Task CheckAuthorizersForRestrictionsAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync(CheckUidsRestrictions command, string caseName)
    {
        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_CallWithLawfulEGN_OkWithBelowLawfulAgeAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-10).Year);
        var egn = "2051040060";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.BelowLawfulAge));
        });
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_CheckDateOfDeathThrowException_InternalServerErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } },
            RapidRetries = true
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/dateofdeath")), ItExpr.IsAny<CancellationToken>())
            .Throws<Exception>()
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
        VerifyDateOfDeatCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_CheckDateOfDeathUnsuccessfulStatusCode_OKWithErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofdeath")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck));
        });
        VerifyDateOfDeatCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_CheckDateOfDeathUnsuccessfulStatusCode_OKDeceasedAuthorizerAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofdeath")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"{{ 'date' : '{DateTime.UtcNow.AddMonths(-1):u}' }}")
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.DeceasedUid));
        });
        VerifyDateOfDeatCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_DateOfProhibitionThrowException_InternalServerErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } },
            RapidRetries = true
        });

        AddDateOfDeatSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/dateofprohibition")), ItExpr.IsAny<CancellationToken>())
            .Throws<Exception>()
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_CheckDateOfProhibitionUnsuccessfulStatusCode_OKWithErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        AddDateOfDeatSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofprohibition")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck));
        });
        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_CheckDateOfProhibitionUnsuccessfulStatusCode_OKProhibitedAuthorizerAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        AddDateOfDeatSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofprohibition")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent($"{{ 'date' : '{DateTime.UtcNow.AddMonths(-1):u}' }}")
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.ProhibitedUid));
        });
        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentityThrowException_InternalServerErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } },
            RapidRetries = true
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .Throws<Exception>()
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.InternalServerError);

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentityUnsuccessfulStatusCode_OKWithErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulWithUnknownRegixError_OKWithErrorAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState("9999", "", false, DateTime.UtcNow.AddYears(-30)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulWithNotFoundLNCh_OKWithErrorAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.NotFound, "", false, DateTime.UtcNow.AddYears(-30)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.UnsuccessfulRestrictionsCheck));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulWithLNChNoPermit_OKWithErrorAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "Непознато разрешение", false, DateTime.UtcNow.AddYears(-30)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.NoPermit));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulWithLNChButDead_OKWithErrorAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", true, DateTime.UtcNow.AddYears(-30)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.DeceasedUid));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulLNChBelowLawfulAge_OKWithErrorAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", false, DateTime.UtcNow.AddYears(-12)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(false));
            Assert.That(result.Result?.DenialReason, Is.EqualTo(EmpowermentsDenialReason.BelowLawfulAge));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulLNCh_OKAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", false, DateTime.UtcNow.AddYears(-32)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(true));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckAuthorizersForRestrictionsAsync_GetForeignIdentitySuccessfulEGN_OKAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "ДЪЛГОСРОЧНО ПРЕБИВАВАЩ В РБ", false, DateTime.UtcNow.AddYears(-32)))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(true));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task CheckUidsRestrictionsAsync_GetForeignIdentitySuccessfulEGNWithoutForeignIdentity_OKAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-32).Year);
        var egn = "9006110819";

        var message = CreateInterface<CheckUidsRestrictions>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        AddDateOfDeatSuccessfulResponse();

        AddDateOfProhibitionSuccessfulResponse();

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.NotFound, "", false, DateTime.UtcNow))
            })
            .Verifiable();

        // Act
        var result = await _sut.CheckUidsRestrictionsAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.Not.Null);
            Assert.That(result.Result?.Successfull, Is.EqualTo(true));
        });

        VerifyDateOfDeatCalls();
        VerifyDateOfProhibitionCalls();
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public void VerifyUidsLawfulAgeAsync_WhenCalledWithNull_ThrowsArgumentNullException()
    {
        //Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => _sut.VerifyUidsLawfulAgeAsync(null));
    }

    [Test]
    [TestCaseSource(nameof(VerifyUidsLawfulAgeCommandInvalidDataTestCases))]
    public async Task VerifyUidsLawfulAgeAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync(VerifyUidsLawfulAge command, string caseName)
    {
        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(command);

        //Assert
        CheckServiceResult(result, HttpStatusCode.BadRequest, caseName);
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CallWithLawfulEGN_OkWithBelowLawfulAgeAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-10).Year);
        var egn = "2051040060";

        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);

        Assert.Multiple(() =>
        {
            Assert.That(result.Result, Is.EqualTo(false));
        });
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CallWithLawfulEGN_OKTrueAsync()
    {
        // Arrange
        //var egn = EgnGenerator.GenerateEgn(DateTime.UtcNow.AddYears(-19).Year);
        var egn = "9006110819";

        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = egn, UidType = IdentifierType.EGN } }
        });

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result.Result, Is.EqualTo(true));
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CheckLnchThrowException_InternalServerErrorAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .Throws<Exception>()
            .Verifiable();

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.InternalServerError);
        Assert.That(result.Result, Is.EqualTo(false));
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CheckLnchUnsuccessfulStatusCode_OKWithFalseAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            })
            .Verifiable();

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result.Result, Is.EqualTo(false));
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CheckLnchSuccessfulWithNotFound_OKWithFalseAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.NotFound, "", false, DateTime.UtcNow.AddYears(-30)))
            })
            .Verifiable();

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.NotFound);
        Assert.That(result.Result, Is.EqualTo(false));
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CheckLnchSuccessfulWithNotLowfulAge_OKWithFalseAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "", false, DateTime.UtcNow.AddYears(-12)))
            })
            .Verifiable();

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result.Result, Is.EqualTo(false));
        VerifyGetForeignIdentityCalls();
    }

    [Test]
    public async Task VerifyUidsLawfulAgeAsync_CheckLnchSuccessfulWithLowfulAge_OKWithTureAsync()
    {
        // Arrange
        var lnch = "5789302455";
        var message = CreateInterface<VerifyUidsLawfulAge>(new
        {
            CorrelationId = Guid.NewGuid(),
            Uids = new UserIdentifierData[] { new() { Uid = lnch, UidType = IdentifierType.LNCh } }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(GetForeignIdentityActualState(RegiXReturnCode.OK, "", false, DateTime.UtcNow.AddYears(-22)))
            })
            .Verifiable();

        // Act
        var result = await _sut.VerifyUidsLawfulAgeAsync(message);

        //Assert
        CheckServiceResult(result, HttpStatusCode.OK);
        Assert.That(result.Result, Is.EqualTo(true));
        VerifyGetForeignIdentityCalls();
    }

    private static string GetForeignIdentityActualState(string returnCode, string rPTypeOfPermit, bool deathDateSpecified, DateTime birthDate) => JsonConvert.SerializeObject(
        new ForeignIdentityActualState
        {
            Error = null,
            HasFailed = false,
            Response = new ActualForeignIdentityInfoResponseType
            {
                ForeignIdentityInfoResponse = new ForeignIdentityInfoResponseType
                {
                    ReturnInformations = new ReturnInformation { ReturnCode = returnCode },
                    IdentityDocument = new IdentityDocument { RPTypeOfPermit = rPTypeOfPermit },
                    DeathDateSpecified = deathDateSpecified,
                    BirthDate = birthDate.ToString("u"),
                }
            }
        });

    private void AddDateOfDeatSuccessfulResponse()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofdeath")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            })
            .Verifiable();
    }

    private void VerifyDateOfDeatCalls()
    {
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofdeath")), ItExpr.IsAny<CancellationToken>());
    }

    private void AddDateOfProhibitionSuccessfulResponse()
    {
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofprohibition")), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            })
            .Verifiable();
    }

    private void VerifyDateOfProhibitionCalls()
    {
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/dateofprohibition")), ItExpr.IsAny<CancellationToken>());
    }

    private void VerifyGetForeignIdentityCalls()
    {
        _mockHttpMessageHandler.Protected()
            .Verify("SendAsync", Times.AtLeastOnce(),
            ItExpr.Is<HttpRequestMessage>(x =>
                    x.RequestUri.AbsolutePath.Contains("/api/v1/registries/mvr/getforeignidentityv2")), ItExpr.IsAny<CancellationToken>());
    }

    private static readonly object[] CheckAuthorizersForRestrictionsCommandInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<CheckUidsRestrictions>(new
            {
                CorrelationId = Guid.Empty
            }),
            "CorrelationId is Empty"
        },
        new object[]
        {
            CreateInterface<CheckUidsRestrictions>(new
            {
                CorrelationId = Guid.NewGuid()
            }),
            "AuthorizerUids is null"
        },
        new object[]
        {
            CreateInterface<CheckUidsRestrictions>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uids = Array.Empty<UserIdentifierData>()
            }),
            "AuthorizerUids is empty"
        },
        new object[]
        {
            CreateInterface<CheckUidsRestrictions>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uids = new UserIdentifierData[] { new UserIdentifierData { Uid =  "1234567890", UidType = IdentifierType.EGN} }
            }),
            "AuthorizerUids contains with not valid EGN or LNCh"
        }
    };

    private static readonly object[] RequesterCheckInLECommandInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "No CorrelationId"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Empty CorrelationId"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Missing IssuerUid"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "",
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Empty IssuerUid"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "",
                IssuerUidType = IdentifierType.NotSpecified,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Invalid IssuerUidType"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "88021848", //Too short
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Invalid IssuerUid - too short"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184855", //Wrong check sum
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Invalid IssuerUid - invalid check sum"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerPosition = "Boss"
            }),
            "Missing IssuerName"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.NewGuid(),
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "",
                IssuerPosition = "Boss"
            }),
            "Empty IssuerName"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name"
            }),
            "Missing IssuerPossition"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = ""
            }),
            "Empty IssuerPossition"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Missing Uid"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "",
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Empty Uid"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111", //Too short
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Invalid Uid - too short"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111111", //Wrong check sum
                Name = "Company Name",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Invalid Uid - wrong check sum"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Missing Name"
        },
        new object[]
        {
            CreateInterface<CheckLegalEntityInNTR>(new
            {
                CorrelationId = Guid.Empty,
                EmpowermentId = Guid.NewGuid(),
                Uid = "111111113", //Valid Bulstat
                Name = "",
                IssuerUid = "8802184852", //Valid EGN
                IssuerUidType = IdentifierType.EGN,
                IssuerName = "Test User Full Name",
                IssuerPosition = "Boss"
            }),
            "Empty Name"
        }
    };

    private static readonly object[] VerifyUidsLawfulAgeCommandInvalidDataTestCases =
    {
        new object[]
        {
            CreateInterface<VerifyUidsLawfulAge>(new
            {
                CorrelationId = Guid.Empty,
            }),
            "CorrelationId is Empty"
        },
        new object[]
        {
            CreateInterface<VerifyUidsLawfulAge>(new
            {
                CorrelationId = Guid.NewGuid(),
            }),
            "AuthorizerUids is null"
        },
        new object[]
        {
            CreateInterface<VerifyUidsLawfulAge>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uids = Array.Empty<UserIdentifierData>()
            }),
            "AuthorizerUids is empty"
        },
        new object[]
        {
            CreateInterface<VerifyUidsLawfulAge>(new
            {
                CorrelationId = Guid.NewGuid(),
                Uids = new UserIdentifierData[] { new UserIdentifierData { Uid = "1234567890", UidType = IdentifierType.EGN } }
            }),
            "AuthorizerUids contains with not valid EGN or LNCh"
        }
    };
}

internal class EgnGenerator
{
    public static string GenerateEgn(int year)
    {
        var random = new Random();

        //int year = random.Next(1800, 2024)
        var month = random.Next(1, 13);

        int day;
        if (month == 2)
        {
            if (IsLeapYear(year))
            {
                day = random.Next(1, 30);
            }
            else
            {
                day = random.Next(1, 29);
            }
        }
        else if (month == 4 || month == 6 || month == 9 || month == 11)
        {
            day = random.Next(1, 31);
        }
        else
        {
            day = random.Next(1, 32);
        }
        if (year >= 2000)
        {
            month += 40;
        }
        if (year < 1900)
        {
            month += 20;
        }

        int sequenceNumber = random.Next(0, 1000);

        var egn = year.ToString("0000")[2..] + month.ToString("00") + day.ToString("00") + sequenceNumber.ToString("000");

        var controlDigit = CalculateControlDigit(egn);

        return egn + controlDigit;
    }

    private static string CalculateControlDigit(string egn)
    {
        int[] weights = new int[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };
        int sum = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            int digit = int.Parse(egn[i].ToString());
            sum += digit * weights[i];
        }

        int controlDigit = sum % 11;
        controlDigit %= 10;

        return controlDigit.ToString();
    }

    private static bool IsLeapYear(int year)
    {
        return (year % 4 == 0 && year % 100 != 0) || year % 400 == 0;
    }
}
