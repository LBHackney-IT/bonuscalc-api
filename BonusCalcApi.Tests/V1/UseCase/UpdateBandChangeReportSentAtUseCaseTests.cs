using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
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
    public class UpdateBandChangeReportSentAtUseCaseTests
    {
        private Fixture _fixture;
        private Mock<IOperativeHelpers> _operativeHelpersMock;
        private Mock<IBonusPeriodGateway> _bonusPeriodGatewayMock;
        private Mock<IBandChangeGateway> _bandChangeGatewayMock;
        private UpdateBandChangeReportSentAtUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _operativeHelpersMock = new Mock<IOperativeHelpers>();
            _bonusPeriodGatewayMock = new Mock<IBonusPeriodGateway>();
            _bandChangeGatewayMock = new Mock<IBandChangeGateway>();

            _classUnderTest = new UpdateBandChangeReportSentAtUseCase(
                _operativeHelpersMock.Object,
                _bonusPeriodGatewayMock.Object,
                _bandChangeGatewayMock.Object,
                InMemoryDb.DbSaver
            );
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task ReportSentAtIsUpdatedWhenNull()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var operative = _fixture.Build<Operative>()
                .With(o => o.Id, "123456")
                .Create();

            var weeks = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.manager@hackney.gov.uk")
                .CreateMany();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .Without(bp => bp.ClosedBy)
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.Weeks, weeks.ToList)
                .Create();

            var bandChange = _fixture.Build<BandChange>()
                .Without(bc => bc.ReportSentAt)
                .With(bc => bc.Operative, operative)
                .With(bc => bc.BonusPeriod, bonusPeriod)
                .With(bc => bc.FinalBand, 6)
                .Create();

            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync("2022-01-31", "123456"))
                .ReturnsAsync(bandChange);

            // Act
            await _classUnderTest.ExecuteAsync(operative.Id);

            // Assert
            bandChange.ReportSentAt.Should().BeOnOrAfter(now);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ReportSentAtIsNotUpdatedWhenNotNull()
        {
            // Arrange
            var reportSentAt = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc);

            var operative = _fixture.Build<Operative>()
                .With(o => o.Id, "123456")
                .Create();

            var weeks = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.manager@hackney.gov.uk")
                .CreateMany();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .Without(bp => bp.ClosedBy)
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.Weeks, weeks.ToList)
                .Create();

            var bandChange = _fixture.Build<BandChange>()
                .With(bc => bc.ReportSentAt, reportSentAt)
                .With(bc => bc.Operative, operative)
                .With(bc => bc.BonusPeriod, bonusPeriod)
                .Create();

            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync("2022-01-31", "123456"))
                .ReturnsAsync(bandChange);

            // Act
            await _classUnderTest.ExecuteAsync(operative.Id);

            // Assert
            bandChange.ReportSentAt.Should().Be(reportSentAt);
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsBadRequestWhenOperativePrnIsInvalid()
        {
            // Arrange
            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("1234"))
                .Returns(false);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("1234");

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Operative payroll number is invalid");

            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotFoundWhenBonusPeriodDoesNotExist()
        {
            // Arrange
            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(null as BonusPeriod);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("There is no open bonus period");

            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotProcessableWhenBonusPeriodIsClosed()
        {
            // Arrange
            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.ClosedAt, DateTime.UtcNow)
                .With(bp => bp.ClosedBy, "a.manager@hackney.gov.uk")
                .With(bp => bp.Id, "2022-01-31")
                .Create();

            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>()
                .WithMessage("Bonus period has already been closed");

            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotProcessableWhenBonusPeriodHasOpenWeeks()
        {
            // Arrange
            var weeks = _fixture.Build<Week>()
                .Without(w => w.ClosedAt)
                .Without(w => w.ClosedBy)
                .CreateMany();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .Without(bp => bp.ClosedBy)
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.Weeks, weeks.ToList)
                .Create();

            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>()
                .WithMessage("Bonus period still has open weeks");

            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotFoundWhenBandChangeDoesNotExist()
        {
            // Arrange
            var weeks = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.manager@hackney.gov.uk")
                .CreateMany();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .Without(bp => bp.ClosedBy)
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.Weeks, weeks.ToList)
                .Create();

            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync("2022-01-31", "123456"))
                .ReturnsAsync(null as BandChange);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>()
                .WithMessage("Band change not found");

            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotProcessableWhenBandChangeIsNotFinal()
        {
            // Arrange
            var weeks = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.manager@hackney.gov.uk")
                .CreateMany();

            var operative = _fixture.Build<Operative>()
                .With(o => o.Id, "123456")
                .Create();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .Without(bp => bp.ClosedBy)
                .With(bp => bp.Id, "2022-01-31")
                .With(bp => bp.Weeks, weeks.ToList)
                .Create();

            var bandChange = _fixture.Build<BandChange>()
                .Without(bc => bc.FinalBand)
                .Without(bc => bc.ReportSentAt)
                .With(bc => bc.Operative, operative)
                .With(bc => bc.BonusPeriod, bonusPeriod)
                .Create();

            _operativeHelpersMock
                .Setup(x => x.IsValidPrn("123456"))
                .Returns(true);

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync("2022-01-31", "123456"))
                .ReturnsAsync(bandChange);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("123456");

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>()
                .WithMessage("Band change has not been finalised");

            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }
    }
}
