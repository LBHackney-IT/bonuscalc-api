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
    public class GetBandChangeAuthorisationsUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private Mock<IBandChangeGateway> _mockBandChangeGateway;
        private GetBandChangeAuthorisationsUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockBandChangeGateway = new Mock<IBandChangeGateway>();

            _classUnderTest = new GetBandChangeAuthorisationsUseCase(
                _mockBonusPeriodGateway.Object,
                _mockBandChangeGateway.Object
            );
        }

        [Test]
        public async Task GetBandChangeAuthorisations()
        {
            // Arrange
            var bonusPeriod = _fixture.Create<BonusPeriod>();
            var expectedAuthorisations = _fixture.CreateMany<BandChange>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _mockBandChangeGateway
                .Setup(x => x.GetBandChangeAuthorisationsAsync(bonusPeriod.Id))
                .ReturnsAsync(expectedAuthorisations);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedAuthorisations);
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
