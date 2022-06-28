using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Controllers;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Controllers
{
    [TestFixture]
    public class PayElementTypesControllerTests : ControllerTests
    {
        private Fixture _fixture;
        private Mock<IGetPayElementTypeUseCase> _getPayElementTypeUseCaseMock;
        private PayElementTypesController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _getPayElementTypeUseCaseMock = new Mock<IGetPayElementTypeUseCase>();

            _classUnderTest = new PayElementTypesController(_getPayElementTypeUseCaseMock.Object);
        }

        [Test]
        public async Task GetsPayElementTypes()
        {
            // Arrange
            var expectedPayElementTypes = _fixture.CreateMany<PayElementType>();
            _getPayElementTypeUseCaseMock.Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedPayElementTypes);

            // Act
            var objectResult = await _classUnderTest.GetPayElementTypes();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<PayElementTypeResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedPayElementTypes.Select(pet => pet.ToResponse()).ToList());
        }
    }
}
