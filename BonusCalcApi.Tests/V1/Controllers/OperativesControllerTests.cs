using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Controllers.Mocks;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.Tests.V1.Helpers.Mocks;
using BonusCalcApi.V1.Boundary.Request;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class OperativesControllerTests : ControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private Mock<IGetOperativeUseCase> _getOperativeUseCaseMock;
        private Mock<IGetOperativesUseCase> _getOperativesUseCaseMock;
        private MockProblemDetailsFactory _problemDetailsFactoryMock;

        private OperativesController _classUnderTest;
        private MockOperativeHelpers _operativeHelpers;
        private Mock<IGetOperativeSummaryUseCase> _getOperativeSummaryUseCaseMock;
        private Mock<IGetOperativeTimesheetUseCase> _getOperativeTimesheetUseCaseMock;
        private Mock<IUpdateTimesheetUseCase> _updateTimesheetUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _getOperativeUseCaseMock = new Mock<IGetOperativeUseCase>();
            _getOperativesUseCaseMock = new Mock<IGetOperativesUseCase>();
            _getOperativeSummaryUseCaseMock = new Mock<IGetOperativeSummaryUseCase>();
            _getOperativeTimesheetUseCaseMock = new Mock<IGetOperativeTimesheetUseCase>();
            _updateTimesheetUseCaseMock = new Mock<IUpdateTimesheetUseCase>();
            _operativeHelpers = new MockOperativeHelpers();
            _problemDetailsFactoryMock = new MockProblemDetailsFactory();

            _classUnderTest = new OperativesController(
                _operativeHelpers.Object,
                _getOperativeUseCaseMock.Object,
                _getOperativesUseCaseMock.Object,
                _getOperativeSummaryUseCaseMock.Object,
                _getOperativeTimesheetUseCaseMock.Object,
                _updateTimesheetUseCaseMock.Object
            );

            // .NET 3.1 doesn't set ProblemDetailsFactory so we need to mock it
            _classUnderTest.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;
        }

        [Test]
        public async Task GetOperativesReturnsOk()
        {
            // Arrange
            var expectedOperatives = _fixture.CreateMany<Operative>();
            _getOperativesUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedOperatives);

            // Act
            var objectResult = await _classUnderTest.GetOperatives("12345678", 1, 25);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<OperativeResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedOperatives.Select(pe => pe.ToResponse()).ToList());
        }

        [Test]
        public async Task GetOperativesReturnsBadRequestIfQueryIsMissing()
        {
            // Arrange

            // Act
            var objectResult = await _classUnderTest.GetOperatives("", null, null);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetOperativeReturnsOk()
        {
            // Arrange
            var operative = _fixture.Create<Operative>();
            _operativeHelpers.ValidPrn(true);
            _getOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(operative);

            // Act
            var objectResult = await _classUnderTest.GetOperative("123456");
            var operativesResult = GetResultData<OperativeResponse>(objectResult);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            operativesResult.Should().BeEquivalentTo(operative.ToResponse());
        }

        [Test]
        public async Task GetOperativeReturnsNotFoundIfPayrollNumberIsNotFound()
        {
            // Arrange
            _operativeHelpers.ValidPrn(true);
            _getOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync((Operative) null);

            // Act
            var objectResult = await _classUnderTest.GetOperative("000000");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetOperativeReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(false);

            // Act
            var objectResult = await _classUnderTest.GetOperative("bad");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetSummaryReturnsOk()
        {
            // Arrange
            var expectedSummary = FixtureHelpers.CreateSummary();
            _getOperativeSummaryUseCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedSummary);
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(true);

            // Act
            var objectResult = await _classUnderTest.GetSummary(expectedSummary.OperativeId, expectedSummary.BonusPeriodId);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<SummaryResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedSummary.ToResponse());
        }

        [Test]
        public async Task GetSummaryReturnsNotFoundIfSummaryIsNotFound()
        {
            // Arrange
            var expectedSummary = _fixture.Create<Summary>();
            _getOperativeSummaryUseCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(null as Summary);
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(true);

            // Act
            var objectResult = await _classUnderTest.GetSummary(expectedSummary.OperativeId, expectedSummary.BonusPeriodId);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetSummaryReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(false);

            // Act
            var objectResult = await _classUnderTest.GetSummary("bad", "period");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetSummaryReturnsBadRequestIfBonusPeriodIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(false);

            // Act
            var objectResult = await _classUnderTest.GetSummary("123456", "period");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetTimesheetReturnsOk()
        {
            // Arrange
            var expectedTimesheet = _fixture.Create<Timesheet>();
            _getOperativeTimesheetUseCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedTimesheet);
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(true);

            // Act
            var objectResult = await _classUnderTest.GetTimesheet(expectedTimesheet.OperativeId, expectedTimesheet.WeekId);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<TimesheetResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedTimesheet.ToResponse());
        }

        [Test]
        public async Task GetTimesheetReturnsNotFoundIfTimesheetIsNotFound()
        {
            // Arrange
            var expectedTimesheet = _fixture.Create<Timesheet>();
            _getOperativeTimesheetUseCaseMock.Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(null as Timesheet);
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(true);

            // Act
            var objectResult = await _classUnderTest.GetTimesheet(expectedTimesheet.OperativeId, expectedTimesheet.WeekId);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task GetTimesheetReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(false);

            // Act
            var objectResult = await _classUnderTest.GetTimesheet("bad", "week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetTimesheetReturnsBadRequestIfWeekIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(false);

            // Act
            var objectResult = await _classUnderTest.GetTimesheet("123456", "week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateTimesheetReturnsOk()
        {
            // Arrange
            const string expectedOperativeId = "operative_id";
            const string expectedWeekId = "week_id";
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(true);
            var updateRequest = _fixture.Create<TimesheetUpdate>();

            // Act
            var objectResult = await _classUnderTest.UpdateTimesheet(updateRequest, expectedOperativeId, expectedWeekId);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            _updateTimesheetUseCaseMock.Verify(x => x.ExecuteAsync(updateRequest, expectedOperativeId, expectedWeekId));
        }

        [Test]
        public async Task UpdateTimesheetReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(false);

            // Act
            var objectResult = await _classUnderTest.UpdateTimesheet(new TimesheetUpdate(), "bad", "week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateTimesheetReturnsBadRequestIfWeekIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(true);
            _operativeHelpers.ValidDate(false);

            // Act
            var objectResult = await _classUnderTest.UpdateTimesheet(new TimesheetUpdate(), "123456", "week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }
    }
}
