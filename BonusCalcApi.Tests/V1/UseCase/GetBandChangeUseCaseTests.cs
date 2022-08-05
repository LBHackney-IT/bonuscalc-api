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
    public class GetBandChangeUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private Mock<IBandChangeGateway> _mockBandChangeGateway;
        private GetBandChangeUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockBandChangeGateway = new Mock<IBandChangeGateway>();

            _classUnderTest = new GetBandChangeUseCase(
                _mockBonusPeriodGateway.Object,
                _mockBandChangeGateway.Object
            );
        }

        [Test]
        public async Task GetBandChange()
        {
            // Arrange
            var bonusPeriod = _fixture.Create<BonusPeriod>();
            var bandChange = _fixture.Create<BandChange>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _mockBandChangeGateway
                .Setup(x => x.GetBandChangeAsync(bonusPeriod.Id, bandChange.OperativeId))
                .ReturnsAsync(bandChange);

            // Act
            var result = await _classUnderTest.ExecuteAsync(bandChange.OperativeId);

            // Assert
            result.Should().BeEquivalentTo(bandChange);
        }

        [Test]
        public async Task ThrowsWhenNoOpenBonusPeriod()
        {
            // Arrange
            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(null as BonusPeriod);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Open bonus period not found");
        }

        [Test]
        public async Task ThrowsWhenNoBandChangeFound()
        {
            // Arrange
            var bonusPeriod = _fixture.Create<BonusPeriod>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _mockBandChangeGateway
                .Setup(x => x.GetBandChangeAsync(bonusPeriod.Id, "123456"))
                .ReturnsAsync(null as BandChange);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Band change not found");
        }
    }
}
