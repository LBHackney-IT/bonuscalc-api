using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Controllers.Mocks;
using BonusCalcApi.Tests.V1.Helpers.Mocks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Controllers.Helpers;
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

        [SetUp]
        public void SetUp()
        {
            _getOperativeUseCaseMock = new Mock<IGetOperativeUseCase>();
            _operativeHelpers = new MockOperativeHelpers();
            _problemDetailsFactoryMock = new MockProblemDetailsFactory();

            _classUnderTest = new OperativesController(
                _operativeHelpers.Object,
                _getOperativeUseCaseMock.Object
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
            operativesResult.Should().BeEquivalentTo(operative);
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
        public async Task GetUnproductiveTimeReturnsBadRequestIfPayrollNumberIsInvalid()
        {
            // Arrange

            // Act
            var objectResult = await _classUnderTest.GetNonProductiveTime("bad");
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            _problemDetailsFactoryMock.VerifyStatusCode(HttpStatusCode.BadRequest);
        }
    }

}
