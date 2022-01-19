using AutoFixture;
using BonusCalcApi.Tests.V1.Controllers.Mocks;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.Tests.V1.Helpers.Mocks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class WeeksControllerTests : ControllerTests
    {
        private Fixture _fixture;
        private Mock<IGetWeekUseCase> _getWeekUseCaseMock;
        private Mock<IUpdateWeekUseCase> _updateWeekUseCaseMock;
        private Mock<IUpdateWeekReportsSentAtUseCase> _updateWeekReportsSentAtUseCaseMock;
        private MockOperativeHelpers _operativeHelpers;
        private MockProblemDetailsFactory _problemDetailsFactoryMock;

        private WeeksController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _operativeHelpers = new MockOperativeHelpers();
            _getWeekUseCaseMock = new Mock<IGetWeekUseCase>();
            _updateWeekUseCaseMock = new Mock<IUpdateWeekUseCase>();
            _updateWeekReportsSentAtUseCaseMock = new Mock<IUpdateWeekReportsSentAtUseCase>();
            _problemDetailsFactoryMock = new MockProblemDetailsFactory();

            _classUnderTest = new WeeksController(
                _operativeHelpers.Object,
                _getWeekUseCaseMock.Object,
                _updateWeekUseCaseMock.Object,
                _updateWeekReportsSentAtUseCaseMock.Object
            );

            // .NET 3.1 doesn't set ProblemDetailsFactory so we need to mock it
            _classUnderTest.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;
        }

        [Test]
        public async Task GetWeekReturnsOk()
        {
            // Arrange
            var expectedWeek = _fixture.Create<Week>();
            _operativeHelpers.ValidDate(true);
            _getWeekUseCaseMock.Setup(x => x.ExecuteAsync("2021-11-08"))
                .ReturnsAsync(expectedWeek);

            // Act
            var objectResult = await _classUnderTest.GetWeek("2021-11-08");
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<WeekResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedWeek.ToResponse());
        }

        [Test]
        public async Task GetWeekReturnsNotFoundIfWeekNotFound()
        {
            // Arrange
            var expectedWeek = _fixture.Create<Week>();
            _operativeHelpers.ValidDate(true);
            _getWeekUseCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Week);

            // Act
            var objectResult = await _classUnderTest.GetWeek(expectedWeek.Id);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetWeekReturnsBadRequestIfWeekInvalid()
        {
            // Arrange
            _operativeHelpers.ValidDate(false);

            // Act
            var objectResult = await _classUnderTest.GetWeek("00000000");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateWeekReturnsOk()
        {
            // Arrange
            var expectedWeek = _fixture.Create<Week>();
            var updateRequest = _fixture.Create<WeekUpdate>();
            _operativeHelpers.ValidDate(true);
            _updateWeekUseCaseMock
                .Setup(x => x.ExecuteAsync(expectedWeek.Id, updateRequest))
                .ReturnsAsync(expectedWeek);

            // Act
            var objectResult = await _classUnderTest.UpdateWeek(expectedWeek.Id, updateRequest);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<WeekResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedWeek.ToResponse());

            _updateWeekUseCaseMock
                .Verify(x => x.ExecuteAsync(expectedWeek.Id, updateRequest));
        }

        [Test]
        public async Task UpdateWeekReturnsNotFoundIfWeekNotFound()
        {
            // Arrange
            var expectedWeek = _fixture.Create<Week>();
            var updateRequest = _fixture.Create<WeekUpdate>();
            _operativeHelpers.ValidDate(true);
            _updateWeekUseCaseMock
                .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<WeekUpdate>()))
                .ReturnsAsync(null as Week);

            // Act
            var objectResult = await _classUnderTest.UpdateWeek(expectedWeek.Id, updateRequest);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task UpdateWeekReturnsBadRequestIfWeekInvalid()
        {
            // Arrange
            var updateRequest = _fixture.Create<WeekUpdate>();
            _operativeHelpers.ValidDate(false);

            // Act
            var objectResult = await _classUnderTest.UpdateWeek("00000000", updateRequest);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateReportsSentAtReturnsOk()
        {
            // Arrange
            const string expectedWeekId = "week_id";
            _operativeHelpers.ValidDate(true);

            // Act
            var objectResult = await _classUnderTest.UpdateReportsSentAt(expectedWeekId);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            _updateWeekReportsSentAtUseCaseMock.Verify(x => x.ExecuteAsync(expectedWeekId));
        }

        [Test]
        public async Task UpdateWeekReportsSentAtReturnsBadRequestIfWeekIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidDate(false);

            // Act
            var objectResult = await _classUnderTest.UpdateReportsSentAt("week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }
    }
}
