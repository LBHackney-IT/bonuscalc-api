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
    public class GetBonusPeriodUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private GetBonusPeriodUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _classUnderTest = new GetBonusPeriodUseCase(_mockBonusPeriodGateway.Object);
        }

        [Test]
        public async Task GetBonusPeriod()
        {
            // Arrange
            var expectedBonusPeriod = _fixture.Create<BonusPeriod>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodIncludingWeeksAsync("2021-11-01"))
                .ReturnsAsync(expectedBonusPeriod);

            // Act
            var result = await _classUnderTest.ExecuteAsync("2021-11-01");

            // Assert
            result.Should().BeEquivalentTo(expectedBonusPeriod);
        }
    }
}
