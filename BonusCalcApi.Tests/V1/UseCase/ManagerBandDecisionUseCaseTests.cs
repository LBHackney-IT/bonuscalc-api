using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class ManagerBandDecisionUseCaseTests
    {
        private Fixture _fixture;
        private Mock<IBonusPeriodGateway> _bonusPeriodGatewayMock;
        private Mock<IBandChangeGateway> _bandChangeGatewayMock;
        private ManagerBandDecisionUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _bonusPeriodGatewayMock = new Mock<IBonusPeriodGateway>();
            _bandChangeGatewayMock = new Mock<IBandChangeGateway>();

            _classUnderTest = new ManagerBandDecisionUseCase(
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
        public async Task SavesManagerBandDecision()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var request = new BandChangeRequest
            {
                Name = "A Manager",
                EmailAddress = "a.manager@hackney.gov.uk",
                Decision = BandChangeDecision.Rejected,
                Reason = "Too many sick days",
                SalaryBand = 5
            };

            var supervisor = new BandChangeApprover
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Approved,
                Reason = null,
                SalaryBand = 7
            };

            var closedWeek = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.week.manager@hackney.gov.uk")
                .Create();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .With(bp => bp.Weeks, new List<Week>() { closedWeek })
                .Create();

            var bandChange = _fixture.Build<BandChange>()
                .With(bc => bc.Supervisor, supervisor)
                .With(bc => bc.Manager, new BandChangeApprover())
                .With(bc => bc.FinalBand, 7)
                .Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync(bonusPeriod.Id, "123456"))
                .ReturnsAsync(bandChange);

            // Act
            var response = await _classUnderTest.ExecuteAsync("123456", request);
            var manager = response.Manager;

            // Assert
            manager.Name.Should().Be("A Manager");
            manager.EmailAddress.Should().Be("a.manager@hackney.gov.uk");
            manager.Decision.Should().Be(BandChangeDecision.Rejected);
            manager.Reason.Should().Be("Too many sick days");
            manager.SalaryBand.Should().Be(5);
            manager.UpdatedAt.Should().BeOnOrAfter(now);
            response.FinalBand.Should().Be(5);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ThrowsWhenNoBonusPeriodOpen()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(null as BonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync("123456", request);

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodHasOpenWeeks()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            var closedWeek = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.week.manager@hackney.gov.uk")
                .Create();

            var openWeek = _fixture.Build<Week>()
                .Without(w => w.ClosedAt)
                .Without(w => w.ClosedBy)
                .Create();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .With(bp => bp.Weeks, new List<Week>() { closedWeek, openWeek })
                .Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync("123456", request);

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodIsClosed()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.ClosedAt, DateTime.UtcNow)
                .Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync("123456", request);

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsWhenBandChangeIsNotFound()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            var closedWeek = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.week.manager@hackney.gov.uk")
                .Create();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .With(bp => bp.Weeks, new List<Week>() { closedWeek })
                .Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync(bonusPeriod.Id, "123456"))
                .ReturnsAsync(null as BandChange);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync("123456", request);

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsWhenBandChangeHasNotBeenProcessedBySupervisor()
        {
            // Arrange
            var request = _fixture.Create<BandChangeRequest>();

            var closedWeek = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.week.manager@hackney.gov.uk")
                .Create();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .With(bp => bp.Weeks, new List<Week>() { closedWeek })
                .Create();

            var bandChange = _fixture.Build<BandChange>()
                .With(bc => bc.Supervisor, new BandChangeApprover())
                .With(bc => bc.Manager, new BandChangeApprover())
                .Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _bandChangeGatewayMock
                .Setup(x => x.GetBandChangeAsync(bonusPeriod.Id, "123456"))
                .ReturnsAsync(bandChange);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync("123456", request);

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }
    }
}
