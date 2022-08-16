using System;
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
    public class GetBonusPeriodsUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private GetBonusPeriodsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _classUnderTest = new GetBonusPeriodsUseCase(_mockBonusPeriodGateway.Object);
        }

        [Test]
        public async Task GetBonusPeriods()
        {
            // Arrange
            var expectedBonusPeriods = _fixture.CreateMany<BonusPeriod>();
            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodsAsync())
                .ReturnsAsync(expectedBonusPeriods);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedBonusPeriods);
        }
    }
}
