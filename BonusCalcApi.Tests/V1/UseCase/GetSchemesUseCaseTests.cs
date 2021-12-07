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
    public class GetSchemesUseCaseTests
    {
        private Mock<ISchemeGateway> _mockSchemeGateway;
        private GetSchemesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockSchemeGateway = new Mock<ISchemeGateway>();
            _classUnderTest = new GetSchemesUseCase(_mockSchemeGateway.Object);
        }
        [Test]
        public async Task GetSchemes()
        {
            // Arrange
            var expectedSchemes = _fixture.CreateMany<Scheme>();
            _mockSchemeGateway
                .Setup(x => x.GetSchemesAsync())
                .ReturnsAsync(expectedSchemes);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedSchemes);
        }
    }
}
