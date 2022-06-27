using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetOperativesUseCaseTests
    {
        private Mock<IOperativeGateway> _mockOperativeGateway;
        private GetOperativesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockOperativeGateway = new Mock<IOperativeGateway>();
            _classUnderTest = new GetOperativesUseCase(_mockOperativeGateway.Object);
        }
        [Test]
        public async Task GetOperatives()
        {
            // Arrange
            var expectedOperatives = _fixture.CreateMany<Operative>();
            _mockOperativeGateway
                .Setup(x => x.GetOperativesAsync("123456", 1, 25))
                .ReturnsAsync(expectedOperatives);

            // Act
            var result = await _classUnderTest.ExecuteAsync("123456", 1, 25);

            // Assert
            result.Should().BeEquivalentTo(expectedOperatives);
        }
    }
}
