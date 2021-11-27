using System;
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
    public class GetCurrentBonusPeriodsUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private GetCurrentBonusPeriodsUseCase _classUnderTest;
        private Fixture _fixture;
        private DateTime _currentDate;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _classUnderTest = new GetCurrentBonusPeriodsUseCase(_mockBonusPeriodGateway.Object);
            _currentDate = new DateTime(2021, 12, 5, 16, 0, 0, DateTimeKind.Utc);
        }

        [Test]
        public async Task GetCurrentBonusPeriods()
        {
            // Arrange
            var expectedBonusPeriods = _fixture.CreateMany<BonusPeriod>();
            _mockBonusPeriodGateway
                .Setup(x => x.GetCurrentBonusPeriodsAsync(_currentDate))
                .ReturnsAsync(expectedBonusPeriods);

            // Act
            var result = await _classUnderTest.ExecuteAsync(_currentDate);

            // Assert
            result.Should().BeEquivalentTo(expectedBonusPeriods);
        }
    }
}
