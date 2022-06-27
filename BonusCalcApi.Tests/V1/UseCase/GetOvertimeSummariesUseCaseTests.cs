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
    public class GetOvertimeSummariesUseCaseTests
    {
        private Mock<IOvertimeSummaryGateway> _mockOvertimeSummaryGateway;
        private GetOvertimeSummariesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockOvertimeSummaryGateway = new Mock<IOvertimeSummaryGateway>();
            _classUnderTest = new GetOvertimeSummariesUseCase(_mockOvertimeSummaryGateway.Object);
        }
        [Test]
        public async Task GetOvertimeSummaries()
        {
            // Arrange
            var expectedOvertimeSummaries = _fixture.CreateMany<OvertimeSummary>();
            _mockOvertimeSummaryGateway
                .Setup(x => x.GetOvertimeSummariesAsync("2021-10-18"))
                .ReturnsAsync(expectedOvertimeSummaries);

            // Act
            var result = await _classUnderTest.ExecuteAsync("2021-10-18");

            // Assert
            result.Should().BeEquivalentTo(expectedOvertimeSummaries);
        }
    }
}
