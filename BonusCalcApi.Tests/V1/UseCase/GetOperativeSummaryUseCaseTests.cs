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
    public class GetOperativeSummaryUseCaseTests
    {
        private GetOperativeSummaryUseCase _classUnderTest;
        private Mock<ISummaryGateway> _mockSummaryGateway;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _mockSummaryGateway = new Mock<ISummaryGateway>();
            _classUnderTest = new GetOperativeSummaryUseCase(_mockSummaryGateway.Object);
        }
        [Test]
        public async Task GetOperativeSummary()
        {
            // Arrange
            var expectedSummary = _fixture.Create<Summary>();
            _mockSummaryGateway.Setup(x => x.GetOperativeSummaryAsync(expectedSummary.OperativeId, expectedSummary.BonusPeriodId))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _classUnderTest.ExecuteAsync(expectedSummary.OperativeId, expectedSummary.BonusPeriodId);

            // Assert
            result.Should().BeEquivalentTo(expectedSummary);
        }
    }
}
