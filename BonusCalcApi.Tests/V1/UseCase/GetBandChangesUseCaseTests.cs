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
    public class GetBandChangesUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private Mock<IBandChangeGateway> _mockBandChangeGateway;
        private GetBandChangesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockBandChangeGateway = new Mock<IBandChangeGateway>();

            _classUnderTest = new GetBandChangesUseCase(
                _mockBonusPeriodGateway.Object,
                _mockBandChangeGateway.Object
            );
        }

        [Test]
        public async Task GetBandChanges()
        {
            // Arrange
            var bonusPeriod = _fixture.Create<BonusPeriod>();
            var expectedBandChanges = _fixture.CreateMany<BandChange>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _mockBandChangeGateway
                .Setup(x => x.GetBandChangesAsync(bonusPeriod.Id))
                .ReturnsAsync(expectedBandChanges);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedBandChanges);
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
