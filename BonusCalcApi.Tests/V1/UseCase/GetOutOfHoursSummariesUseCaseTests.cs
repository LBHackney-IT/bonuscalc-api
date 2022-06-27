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
    public class GetOutOfHoursSummariesUseCaseTests
    {
        private Mock<IOutOfHoursSummaryGateway> _mockOutOfHoursSummaryGateway;
        private GetOutOfHoursSummariesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockOutOfHoursSummaryGateway = new Mock<IOutOfHoursSummaryGateway>();
            _classUnderTest = new GetOutOfHoursSummariesUseCase(_mockOutOfHoursSummaryGateway.Object);
        }
        [Test]
        public async Task GetOutOfHoursSummaries()
        {
            // Arrange
            var expectedOutOfHoursSummaries = _fixture.CreateMany<OutOfHoursSummary>();
            _mockOutOfHoursSummaryGateway
                .Setup(x => x.GetOutOfHoursSummariesAsync("2021-10-18"))
                .ReturnsAsync(expectedOutOfHoursSummaries);

            // Act
            var result = await _classUnderTest.ExecuteAsync("2021-10-18");

            // Assert
            result.Should().BeEquivalentTo(expectedOutOfHoursSummaries);
        }
    }
}
