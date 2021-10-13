using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class OperativesControllerTests : ControllerTests
    {
        private readonly Fixture _fixture = new Fixture();

        private Mock<IGetOperativeUseCase> _getOperativeUseCaseMock;
        private Mock<ProblemDetailsFactory> _problemDetailsFactoryMock;

        private OperativesController _classUnderTest;

        [SetUp]
        public void SetUp()
        {

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _getOperativeUseCaseMock = new Mock<IGetOperativeUseCase>();
            _problemDetailsFactoryMock = new Mock<ProblemDetailsFactory>();

            _classUnderTest = new OperativesController(
                _getOperativeUseCaseMock.Object
            );

            // .NET 3.1 doesn't set ProblemDetailsFactory so we need to mock it
            _classUnderTest.ProblemDetailsFactory = _problemDetailsFactoryMock.Object;
        }

        [TestCase("123456")]
        public async Task ReturnsOkIfPayrollNumberIsValid(string operativePayrollNumber)
        {
            // Arrange
            var operative = _fixture.Create<Operative>();
            _getOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync(operative);

            // Act
            var objectResult = await _classUnderTest.GetOperative(operativePayrollNumber);
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

        [TestCase("000000")]
        public async Task ReturnsNotFoundIfPayrollNumberIsNotFound(string operativePayrollNumber)
        {
            // Arrange
            _getOperativeUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>()))
                .ReturnsAsync((Operative) null);

            var problemDetails = new ProblemDetails()
            {
                Status = StatusCodes.Status404NotFound
            };

            _problemDetailsFactoryMock
                .Setup(m => m.CreateProblemDetails(
                    It.IsAny<HttpContext>(),
                    It.IsAny<int?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                )
                .Returns(problemDetails)
                .Verifiable();

            // Act
            var objectResult = await _classUnderTest.GetOperative(operativePayrollNumber);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.NotFound);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("123")]
        [TestCase("1234567")]
        [TestCase("ABCDEF")]
        public async Task ReturnsBadRequestIfPayrollNumberIsInvalid(string operativePayrollNumber)
        {
            // Arrange
            var problemDetails = new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest
            };

            _problemDetailsFactoryMock
                .Setup(m => m.CreateProblemDetails(
                    It.IsAny<HttpContext>(),
                    It.IsAny<int?>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())
                )
                .Returns(problemDetails)
                .Verifiable();

            // Act
            var objectResult = await _classUnderTest.GetOperative(operativePayrollNumber);
            var statusCode = GetStatusCode(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
        }
    }
}
