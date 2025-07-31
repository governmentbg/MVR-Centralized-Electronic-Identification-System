using System.Net;
using eID.PIVR.Contracts.Commands;
using eID.PIVR.Contracts.Results;
using eID.PIVR.Service;
using eID.PIVR.UnitTests.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace eID.PIVR.UnitTests
{
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public class RegiXServiceTests : BaseTest
    {
        private ILogger<RegiXService> _logger;
        private RegiXService _sut;

        private Mock<IRegiXCaller> _regix;

        [SetUp]
        public void Init()
        {
            _logger = new NullLogger<RegiXService>();

            _regix = new Mock<IRegiXCaller>();

            _sut = new RegiXService(_logger, _regix.Object);
        }

        [Test]
        public void SearchAsync_WhenCalledWithNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SearchAsync(null));
        }


        [Test]
        [TestCaseSource(nameof(RegiXSearchCommandValidDataTestCases))]
        public async Task SearchAsync_WhenCalledWithValidData_ShouldCallRegixAsync(string caseName, RegixSearchResultDTO regixResponse, HttpStatusCode expectedResponseStatusCode)
        {
            var command = CreateInterface<RegiXSearchCommand>(new
            {
                CorrelationId = Guid.NewGuid(),
                Command = _validCommand
            });
            _regix.Setup(x => x.SearchAsync(It.IsAny<RegiXSearchCommand>()))
                .ReturnsAsync(regixResponse);
            // Act
            var result = await _sut.SearchAsync(command);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.StatusCode, Is.EqualTo(expectedResponseStatusCode));
            });
            _regix.Verify(x => x.SearchAsync(It.IsAny<RegiXSearchCommand>()), Times.Once);
        }

        [Test]
        [TestCaseSource(nameof(RegiXSearchCommandInvalidDataTestCases))]
        public async Task SearchAsync_WhenCalledWithInvalidData_ShouldReturnBadRequestAsync(RegiXSearchCommand command, string caseName)
        {
            // Act
            var result = await _sut.SearchAsync(command);

            //Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Error, Is.Not.Empty);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            });
            _regix.Verify(x => x.SearchAsync(It.IsAny<RegiXSearchCommand>()), Times.Never);
        }

        private static readonly string _validCommand = JsonConvert.SerializeObject(new
        {
            Operation = "TechnoLogica.RegiX.GraoCivilStatusActsAdapter.APIService.IGraoCivilStatusActsAPI.GetMarriageCertificate",
            Argument = new
            {
                Type = "MarriageCertificateRequest",
                Xmlns = "http://egov.bg/RegiX/GRAO/MarriageCertificate/MarriageCertificateRequest",
                Parameters = new List<Dictionary<string, object>> {
                            new Dictionary<string, object>
                            {
                                {
                                    "Egn",
                                    new {
                                        ParameterStringValue = "8206061432",
                                        ParameterType = "STRING"
                                    }
                                }
                            }
                        }
            }
        });
        private static readonly object[] RegiXSearchCommandInvalidDataTestCases =
        {
            new object[]
            {
                CreateInterface<RegiXSearchCommand>(new
                {
                    Command = _validCommand
                }),
                "No CorrelationId"
            },
            new object[]
            {
                CreateInterface<RegiXSearchCommand>(new
                {
                    CorrelationId = Guid.Empty,
                    Command = _validCommand
                }),
                "Empty CorrelationId"
            },
            new object[]
            {
                CreateInterface<RegiXSearchCommand>(new
                {
                    CorrelationId = Guid.NewGuid()
                }),
                "No Command"
            },
            new object[]
            {
                CreateInterface<RegiXSearchCommand>(new
                {
                    CorrelationId = Guid.NewGuid(),
                    Command = ""
                }),
                "Empty Command"
            }
        };

        private static readonly object[] RegiXSearchCommandValidDataTestCases =
        {
            new object[] {
                "RegiX responds with a result.",
                new RegixSearchResultDTO { Response = new Dictionary<string, object?> { { "OK", "OK" } } },
                HttpStatusCode.OK
            },
            new object[] {
                "RegiX failed to respond.",
                new RegixSearchResultDTO{ HasFailed = true, Error = "Whatever" },
                HttpStatusCode.BadGateway
            },
            new object[] {
                "RegiX returned null response",
                null,
                HttpStatusCode.InternalServerError
            }
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
}
