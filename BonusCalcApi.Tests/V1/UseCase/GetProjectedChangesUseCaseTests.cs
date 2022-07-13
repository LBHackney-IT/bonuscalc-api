using System;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class GetProjectedChangesUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private Mock<IOperativeProjectionGateway> _mockOperativeProjectionGateway;
        private GetProjectedChangesUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockOperativeProjectionGateway = new Mock<IOperativeProjectionGateway>();

            _classUnderTest = new GetProjectedChangesUseCase(
                _mockBonusPeriodGateway.Object,
                _mockOperativeProjectionGateway.Object
            );
        }

        [Test]
        public async Task GetProjectedChanges()
        {
            // Arrange
            var bonusPeriod = _fixture.Create<BonusPeriod>();
            var expectedProjections = _fixture.CreateMany<OperativeProjection>();

            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriod())
                .ReturnsAsync(bonusPeriod);

            _mockOperativeProjectionGateway
                .Setup(x => x.GetAllByBonusPeriodIdAsync(bonusPeriod.Id))
                .ReturnsAsync(expectedProjections);

            // Act
            var result = await _classUnderTest.ExecuteAsync();

            // Assert
            result.Should().BeEquivalentTo(expectedProjections);
        }

        [Test]
        public async Task ThrowsWhenNoOpenBonusPeriod()
        {
            // Arrange
            _mockBonusPeriodGateway
                .Setup(x => x.GetEarliestOpenBonusPeriod())
                .ReturnsAsync(null as BonusPeriod);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync();

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
        }
    }
}
