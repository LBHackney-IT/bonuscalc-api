using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class WorkElementsControllerTests : ControllerTests
    {
        private readonly Fixture _fixture = new Fixture();
        private Mock<IGetWorkElementsUseCase> _getWorkElementsUseCaseMock;
        private WorkElementsController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _getWorkElementsUseCaseMock = new Mock<IGetWorkElementsUseCase>();

            _classUnderTest = new WorkElementsController(
                _getWorkElementsUseCaseMock.Object
            );
        }

        [Test]
        public async Task GetWorkElementsReturnsOk()
        {
            // Arrange
            var expectedWorkElements = _fixture.CreateMany<WorkElement>();
            _getWorkElementsUseCaseMock
                .Setup(m => m.ExecuteAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedWorkElements);

            // Act
            var objectResult = await _classUnderTest.GetWorkElements("12345678", 1, 25);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<WorkElementResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedWorkElements.Select(pe => pe.ToResponse()).ToList());
        }

        [Test]
        public async Task GetWorkElementsReturnsBadRequestIfQueryIsMissing()
        {
            // Arrange

            // Act
            var objectResult = await _classUnderTest.GetWorkElements("", null, null);
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<ProblemDetails>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.BadRequest);
            result.Status.Should().Be((int) HttpStatusCode.BadRequest);
        }
    }
}
