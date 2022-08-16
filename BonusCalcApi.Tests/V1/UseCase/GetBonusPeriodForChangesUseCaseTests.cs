using System;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetBonusPeriodForChangesUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private GetBonusPeriodForChangesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();

            _classUnderTest = new GetBonusPeriodForChangesUseCase(
                _mockBonusPeriodGateway.Object
            );
        }

        [Test]
        public async Task GetBonusPeriodForChanges()
        {
            // Arrange
            var expectedBonusPeriod = _fixture.Create<BonusPeriod>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(expectedBonusPeriod);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedBonusPeriod);
        }

        [Test]
        public async Task ThrowsWhenNoOpenBonusPeriod()
        {
            // Arrange
            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(null as BonusPeriod);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync();

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
