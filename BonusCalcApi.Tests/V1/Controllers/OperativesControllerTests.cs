using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Controllers.Mocks;
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
        private MockProblemDetailsFactory _problemDetailsFactoryMock;

        private OperativesController _classUnderTest;
        private MockOperativeHelpers _operativeHelpers;
        private Mock<IGetOperativeTimesheetUseCase> _getOperativesTimesheetUseCaseMock;
        private Mock<IUpdateTimesheetUseCase> _updateTimesheetUseCaseMock;

        [SetUp]
        public void SetUp()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _getOperativeUseCaseMock = new Mock<IGetOperativeUseCase>();
            _getOperativesTimesheetUseCaseMock = new Mock<IGetOperativeTimesheetUseCase>();
            _updateTimesheetUseCaseMock = new Mock<IUpdateTimesheetUseCase>();
            _operativeHelpers = new MockOperativeHelpers();
            _problemDetailsFactoryMock = new MockProblemDetailsFactory();

            _classUnderTest = new OperativesController(
                _operativeHelpers.Object,
                _getOperativeUseCaseMock.Object,
                _getOperativesTimesheetUseCaseMock.Object,
                _updateTimesheetUseCaseMock.Object
            );

            // .NET 3.1 doesn't set ProblemDetailsFactory so we need to mock it
            _classUnderTest.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;
        }

        [Test]
        public async Task ReturnsOkIfPayrollNumberIsValid()
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
            operativesResult.Should().BeEquivalentTo(operative, options => options
                .Including(o => o.Id)
                .Including(o => o.Name)
                .Including(o => o.Trade)
                .Including(o => o.Section)
                .Including(o => o.Scheme)
                .Including(o => o.SalaryBand)
                .Including(o => o.FixedBand)
                .Including(o => o.IsArchived));
        }

        [Test]
        public async Task ReturnsNotFoundIfPayrollNumberIsNotFound()
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
        public async Task ReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange
            _operativeHelpers.ValidPrn(false);

            // Act
            var objectResult = await _classUnderTest.GetOperative("000000");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetTimesheetReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange

            // Act
            var objectResult = await _classUnderTest.GetTimesheet("bad", "week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task UpdateTimesheetReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange

            // Act
            var objectResult = await _classUnderTest.UpdateTimesheet(new TimesheetUpdateRequest(), "bad", "week");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task GetsTimesheet()
        {
            // Arrange
            var expectedTimesheet = _fixture.Create<Timesheet>();
            _getOperativesTimesheetUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(expectedTimesheet);
            _operativeHelpers.ValidPrn(true);

            // Act
            var objectResult = await _classUnderTest.GetTimesheet(expectedTimesheet.OperativeId, expectedTimesheet.WeekId);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<TimesheetResponse>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedTimesheet.ToResponse());
        }

        [Test]
        public async Task Returns404WhenNoTimesheetFound()
        {
            // Arrange
            var expectedTimesheet = _fixture.Create<Timesheet>();
            _getOperativesTimesheetUseCaseMock.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(null as Timesheet);
            _operativeHelpers.ValidPrn(true);

            // Act
            var objectResult = await _classUnderTest.GetTimesheet(expectedTimesheet.OperativeId, expectedTimesheet.WeekId);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.NotFound);
        }

        [Test]
        public async Task UpdatesTimesheet()
        {
            // Arrange
            const string expectedOperativeId = "operative_id";
            const string expectedWeekId = "week_id";
            _operativeHelpers.ValidPrn(true);
            var updateRequest = _fixture.Create<TimesheetUpdateRequest>();

            // Act
            var objectResult = await _classUnderTest.UpdateTimesheet(updateRequest, expectedOperativeId, expectedWeekId);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            _updateTimesheetUseCaseMock.Verify(x => x.Execute(updateRequest, expectedOperativeId, expectedWeekId));
        }
    }

}
