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
    public class PayBandsControllerTests : ControllerTests
    {
        private Fixture _fixture;
        private Mock<IGetSchemesUseCase> _getSchemesUseCaseMock;

        private PayBandsController _classUnderTest;

        [SetUp]
        public void SetUp()
        {
            _fixture = FixtureHelpers.Fixture;
            _getSchemesUseCaseMock = new Mock<IGetSchemesUseCase>();

            _classUnderTest = new PayBandsController(_getSchemesUseCaseMock.Object);
        }

        [Test]
        public async Task GetsPayBands()
        {
            // Arrange
            var expectedSchemes = _fixture.CreateMany<Scheme>();
            _getSchemesUseCaseMock.Setup(x => x.ExecuteAsync())
                .ReturnsAsync(expectedSchemes);

            // Act
            var objectResult = await _classUnderTest.GetPayBands();
            var statusCode = GetStatusCode(objectResult);
            var result = GetResultData<List<SchemeResponse>>(objectResult);

            // Assert
            statusCode.Should().Be((int) HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedSchemes.Select(s => s.ToResponse()).ToList());
        }
    }
}
