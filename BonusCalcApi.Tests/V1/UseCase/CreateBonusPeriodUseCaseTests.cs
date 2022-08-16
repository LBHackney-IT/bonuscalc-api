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
    public class CreateBonusPeriodUseCaseTests
    {
        private Mock<IBonusPeriodGateway> _mockBonusPeriodGateway;
        private Mock<IOperativeHelpers> _mockOperativeHelpers;
        private CreateBonusPeriodUseCase _classUnderTest;
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _mockBonusPeriodGateway = new Mock<IBonusPeriodGateway>();
            _mockOperativeHelpers = new Mock<IOperativeHelpers>();

            _classUnderTest = new CreateBonusPeriodUseCase(
                _mockBonusPeriodGateway.Object,
                _mockOperativeHelpers.Object
            );
        }

        [Test]
        public async Task CreateBonusPeriod()
        {
            // Arrange
            var request = new BonusPeriodRequest { Id = "2022-01-31" };

            var lastBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.Id, "2021-11-01")
                .With(bp => bp.StartAt, new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc))
                .With(bp => bp.Year, 2021)
                .With(bp => bp.Number, 4)
                .Without(bp => bp.ClosedAt)
                .Create();

            var expectedBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.StartAt, new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc))
                .With(bp => bp.Year, 2022)
                .With(bp => bp.Number, 1)
                .Without(bp => bp.ClosedAt)
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(request.Id))
                .ReturnsAsync(null as BonusPeriod);

            _mockBonusPeriodGateway
                .Setup(x => x.GetLastBonusPeriodAsync())
                .ReturnsAsync(lastBonusPeriod);

            _mockBonusPeriodGateway
                .Setup(x => x.CreateBonusPeriodAsync(request.Id))
                .ReturnsAsync(expectedBonusPeriod);

            // Act
            var result = await _classUnderTest.ExecuteAsync(request);

            // Assert
            result.Should().BeEquivalentTo(expectedBonusPeriod);
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateIsInvalid()
        {
            var request = new BonusPeriodRequest { Id = "20220131" };

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(false);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Date format is invalid - it should be YYYY-MM-DD");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateAlreadyExists()
        {
            var request = new BonusPeriodRequest { Id = "2022-01-31" };

            var existingBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.StartAt, new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc))
                .With(bp => bp.Year, 2022)
                .With(bp => bp.Number, 1)
                .Without(bp => bp.ClosedAt)
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(request.Id))
                .ReturnsAsync(existingBonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Bonus period '2022-01-31' already exists");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateCantBeParsed()
        {
            var request = new BonusPeriodRequest { Id = "2022-02-31" };

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(request.Id))
                .ReturnsAsync(null as BonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Date is invalid - could not parse '2022-02-31'");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateIsBeforeTheFirstPeriod()
        {
            var request = new BonusPeriodRequest { Id = "2021-02-01" };

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(request.Id))
                .ReturnsAsync(null as BonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Date is before the first bonus period");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateIsBeforeTheLastPeriod()
        {
            var request = new BonusPeriodRequest { Id = "2021-11-01" };

            var lastBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.StartAt, new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc))
                .With(bp => bp.Year, 2022)
                .With(bp => bp.Number, 1)
                .Without(bp => bp.ClosedAt)
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(request.Id))
                .ReturnsAsync(null as BonusPeriod);

            _mockBonusPeriodGateway
                .Setup(x => x.GetLastBonusPeriodAsync())
                .ReturnsAsync(lastBonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Date is before the last bonus period");
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodDateIsNotCorrectPeriod()
        {
            var request = new BonusPeriodRequest { Id = "2022-01-30" };

            var lastBonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.Id, "2021-11-01")
                .With(bp => bp.StartAt, new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc))
                .With(bp => bp.Year, 2021)
                .With(bp => bp.Number, 4)
                .Without(bp => bp.ClosedAt)
                .Create();

            _mockOperativeHelpers
                .Setup(x => x.IsValidDate(request.Id))
                .Returns(true);

            _mockBonusPeriodGateway
                .Setup(x => x.GetBonusPeriodAsync(request.Id))
                .ReturnsAsync(null as BonusPeriod);

            _mockBonusPeriodGateway
                .Setup(x => x.GetLastBonusPeriodAsync())
                .ReturnsAsync(lastBonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync(request);

            // Assert
            await act.Should()
                .ThrowAsync<BadRequestException>()
                .WithMessage("Date is not a valid 13 week period");
        }
    }
}
