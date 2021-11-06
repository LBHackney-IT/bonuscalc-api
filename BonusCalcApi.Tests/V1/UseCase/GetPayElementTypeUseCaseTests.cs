using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetPayElementTypeUseCaseTests
    {
        private Mock<IPayElementTypesGateway> _mockPayElementTypeGateway;
        private GetPayElementTypeUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockPayElementTypeGateway = new Mock<IPayElementTypesGateway>();
            _classUnderTest = new GetPayElementTypeUseCase(_mockPayElementTypeGateway.Object);
        }
        [Test]
        public async Task GetPayElementType()
        {
            // Arrange
            var expectedPayElementType = _fixture.CreateMany<PayElementType>();
            _mockPayElementTypeGateway
                .Setup(x => x.GetPayElementTypesAsync())
                .ReturnsAsync(expectedPayElementType);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedPayElementType);
        }
    }
}
