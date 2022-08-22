using System;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.Tests.V1.Helpers.Mocks;
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
        private MockOperativeHelpers _mockOperativeHelpers;
        private GetBandChangesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockBandChangeGateway = new Mock<IBandChangeGateway>();
            _mockOperativeHelpers = new MockOperativeHelpers();

            _classUnderTest = new GetBandChangesUseCase(
                _mockBonusPeriodGateway.Object,
                _mockBandChangeGateway.Object,
                _mockOperativeHelpers.Object
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
            var result = await _classUnderTest.ExecuteAsync(null);

            // Assert
            result.Should().BeEquivalentTo(expectedBandChanges);
        }

        [Test]
        public async Task GetBandChangesForPeriod()
        {
            // Arrange
            var bonusPeriod = _fixture.Create<BonusPeriod>();
            var expectedBandChanges = _fixture.CreateMany<BandChange>();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriod.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(bonusPeriod.Id))
                .ReturnsAsync(bonusPeriod);

            _mockBandChangeGateway
                .Setup(x => x.GetBandChangesAsync(bonusPeriod.Id))
                .ReturnsAsync(expectedBandChanges);

            // Act
            var result = await _classUnderTest.ExecuteAsync(bonusPeriod.Id);

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
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync(null);

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodNotFound()
        {
            // Arrange
            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync("2021-08-02"))
                .ReturnsAsync(null as BonusPeriod);

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate("2021-08-02"))
                .Returns(true);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("2021-08-02");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodIdInvalid()
        {
            // Arrange
            _mockOperativeHelpers
                .Setup(x => x.IsValidDate("20210802"))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("20210802");

            // Assert
            await act.Should().ThrowAsync<BadRequestException>();
        }
    }
}
