using System;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Controllers.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class CloseBonusPeriodUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private Mock<IBandChangeGateway> _mockBandChangeGateway;
        private Mock<IWeekGateway> _mockWeekGateway;
        private Mock<IOperativeHelpers> _mockOperativeHelpers;
        private CloseBonusPeriodUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockBandChangeGateway = new Mock<IBandChangeGateway>();
            _mockWeekGateway = new Mock<IWeekGateway>();
            _mockOperativeHelpers = new Mock<IOperativeHelpers>();

            _classUnderTest = new CloseBonusPeriodUseCase(
                _mockBonusPeriodGateway.Object,
                _mockBandChangeGateway.Object,
                _mockWeekGateway.Object,
                _mockOperativeHelpers.Object
            );
        }

        [Test]
        public async Task ClosesBonusPeriod()
        {
            // Arrange
            var bonusPeriodId = "2022-01-31";

            var request = _fixture.Build<BonusPeriodUpdate>()
                .With(x => x.ClosedAt, new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc))
                .With(x => x.ClosedBy, "a.manager@hackney.gov.uk")
                .Create();

            var openBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(x => x.Id, "2022-01-31")
                .With(x => x.StartAt, new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc))
                .With(x => x.Year, 2022)
                .With(x => x.Number, 1)
                .Without(x => x.ClosedAt)
                .Without(x => x.ClosedBy)
                .Create();

            var closedBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(x => x.Id, "2022-01-31")
                .With(x => x.StartAt, new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc))
                .With(x => x.Year, 2022)
                .With(x => x.Number, 1)
                .With(x => x.ClosedAt, new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc))
                .With(x => x.ClosedBy, "a.manager@hackney.gov.uk")
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriodId))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(bonusPeriodId))
                .ReturnsAsync(openBonusPeriod);

            _mockWeekGateway
                .Setup(x => x.CountOpenWeeksAsync(openBonusPeriod.Id))
                .ReturnsAsync(0);

            _mockBandChangeGateway
                .Setup(x => x.CountRemainingBandChangesAsync(openBonusPeriod.Id))
                .ReturnsAsync(0);

            _mockBonusPeriodGateway
                .Setup(x => x.CloseBonusPeriodAsync(openBonusPeriod.Id, 202, request.ClosedAt, request.ClosedBy))
                .ReturnsAsync(closedBonusPeriod);

            // Act
            var result = await _classUnderTest.ExecuteAsync(bonusPeriodId, request);

            // Assert
            result.Should().BeEquivalentTo(closedBonusPeriod);
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateIsInvalid()
        {
            // Arrange
            var bonusPeriodId = "20220131";
            var request = _fixture.Create<BonusPeriodUpdate>();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriodId))
                .Returns(false);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(bonusPeriodId, request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Bonus period is invalid - it should be YYYY-MM-DD");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodIsNotFound()
        {
            // Arrange
            var bonusPeriodId = "2022-01-31";
            var request = _fixture.Create<BonusPeriodUpdate>();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriodId))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(bonusPeriodId))
                .ReturnsAsync(null as BonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(bonusPeriodId, request);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Bonus period not found");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodIsClosed()
        {
            // Arrange
            var bonusPeriodId = "2022-01-31";
            var request = _fixture.Create<BonusPeriodUpdate>();
            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .With(x => x.ClosedAt, new DateTime(2022, 5, 8, 16, 0, 0, DateTimeKind.Utc))
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriodId))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(bonusPeriodId))
                .ReturnsAsync(bonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(bonusPeriodId, request);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotProcessableException>()
                .WithMessage("Bonus period is already closed");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodHasOpenWeeks()
        {
            // Arrange
            var bonusPeriodId = "2022-01-31";
            var request = _fixture.Create<BonusPeriodUpdate>();
            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(x => x.ClosedAt)
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriodId))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(bonusPeriodId))
                .ReturnsAsync(bonusPeriod);

            _mockWeekGateway
                .Setup(x => x.CountOpenWeeksAsync(bonusPeriod.Id))
                .ReturnsAsync(3);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(bonusPeriodId, request);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotProcessableException>()
                .WithMessage("Bonus period still has open weeks");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodHasRemainingBandChanges()
        {
            // Arrange
            var bonusPeriodId = "2022-01-31";
            var request = _fixture.Create<BonusPeriodUpdate>();
            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(x => x.ClosedAt)
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(bonusPeriodId))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(bonusPeriodId))
                .ReturnsAsync(bonusPeriod);

            _mockWeekGateway
                .Setup(x => x.CountOpenWeeksAsync(bonusPeriod.Id))
                .ReturnsAsync(0);

            _mockBandChangeGateway
                .Setup(x => x.CountRemainingBandChangesAsync(bonusPeriod.Id))
                .ReturnsAsync(3);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(bonusPeriodId, request);

            // Assert
            await act.Should()
                .ThrowAsync<ResourceNotProcessableException>()
                .WithMessage("Bonus period still has band changes that have not been processed");
        }
    }
}
