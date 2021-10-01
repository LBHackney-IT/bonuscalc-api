using BonusCalcApi.V1.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using BonusCalcApi.V1.UseCase.Interfaces;
using BonusCalcApi.V1.Boundary.Response;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class OperativesControllerTests
    {
        private OperativesController _sut;
        private Mock<IOperativesGateway> _operativeGateway;
        private OperativeResponse _validOperative;

        public OperativesControllerTests()
        {
            _operativeGateway = new Mock<IOperativesGateway>();
            _sut = new OperativesController(_operativeGateway.Object);

        }

        [TestCase("")]
        [TestCase("  ")]
        [TestCase(null)]
        public void ReturnsBadRequestIfRequestIsBad(string testPayrollStr)
        {
            // Arrange
            _operativeGateway.Setup(og => og.Execute(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult(new OperativeResponse()));

            // Act
            var response = _sut.GetOperative(testPayrollStr);

            // Assert
            GetStatusCode(response).Should().Be(400);
        }

        public void ReturnsNotFoundIfNoMatches()
        {
            // Arrange
            _operativeGateway.Setup(og => og.Execute(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult((OperativeResponse) null));

            // Act
            var response = _sut.GetOperative("H-249387");

            // Assert
            GetStatusCode(response).Should().Be(404);
        }

        public void ReturnsCorrectDataFor()
        {
            // Arrange
            _validOperative = GenerateValidOperative();

            _operativeGateway.Setup(og => og.Execute(Moq.It.IsAny<string>()))
                .Returns(Task.FromResult(_validOperative));

            // Act
            var response = _sut.GetOperative("H-8345344");

            // Assert
            response.As<OperativeResponse>().Name.Should().Be("Paul Casey");
        }

        //TODO: generative values
        private static OperativeResponse GenerateValidOperative()
        {
            return new OperativeResponse
            {
                Id = 1000,
                Name = "Paul Casey",
                PayrollNumber = "H-8345344"
            };
        }

        protected static int? GetStatusCode(IActionResult result)
        {
            return (result as IStatusCodeActionResult).StatusCode;
        }
    }
}
