using System;
using System.Collections.Generic;
using System.Linq;
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
    public class SupervisorBandDecisionUseCaseTests
    {
        private Fixture _fixture;
        private Mock<IBonusPeriodGateway> _bonusPeriodGatewayMock;
        private Mock<IBandChangeGateway> _bandChangeGatewayMock;
        private SupervisorBandDecisionUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _bonusPeriodGatewayMock = new Mock<IBonusPeriodGateway>();
            _bandChangeGatewayMock = new Mock<IBandChangeGateway>();

            _classUnderTest = new SupervisorBandDecisionUseCase(
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
        public async Task SavesSupervisorApprovalDecision()
        {
            // Arrange
            var request = new BandChangeRequest
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Approved,
                Reason = null,
                SalaryBand = 6
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
                .Without(bc => bc.FinalBand)
                .With(bc => bc.ProjectedBand, 6)
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
            var response = await _classUnderTest.ExecuteAsync("123456", request);
            var supervisor = response.Supervisor;

            // Assert
            supervisor.Name.Should().Be("A Supervisor");
            supervisor.EmailAddress.Should().Be("a.supervisor@hackney.gov.uk");
            supervisor.Decision.Should().Be(BandChangeDecision.Approved);
            supervisor.Reason.Should().BeNull();
            supervisor.SalaryBand.Should().Be(6);
            response.FinalBand.Should().Be(6);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task SavesSupervisorRejectionDownwardsDecision()
        {
            // Arrange
            var request = new BandChangeRequest
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Rejected,
                Reason = "Some reasons",
                SalaryBand = 5
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
                .Without(bc => bc.FinalBand)
                .With(bc => bc.ProjectedBand, 6)
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
            var response = await _classUnderTest.ExecuteAsync("123456", request);
            var supervisor = response.Supervisor;

            // Assert
            supervisor.Name.Should().Be("A Supervisor");
            supervisor.EmailAddress.Should().Be("a.supervisor@hackney.gov.uk");
            supervisor.Decision.Should().Be(BandChangeDecision.Rejected);
            supervisor.Reason.Should().Be("Some reasons");
            supervisor.SalaryBand.Should().Be(5);
            response.FinalBand.Should().Be(5);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task SavesSupervisorRejectionUpwardsDecision()
        {
            // Arrange
            var request = new BandChangeRequest
            {
                Name = "A Supervisor",
                EmailAddress = "a.supervisor@hackney.gov.uk",
                Decision = BandChangeDecision.Rejected,
                Reason = "Some reasons",
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
                .With(bc => bc.FinalBand, 6)
                .With(bc => bc.ProjectedBand, 6)
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
            var response = await _classUnderTest.ExecuteAsync("123456", request);
            var supervisor = response.Supervisor;

            // Assert
            supervisor.Name.Should().Be("A Supervisor");
            supervisor.EmailAddress.Should().Be("a.supervisor@hackney.gov.uk");
            supervisor.Decision.Should().Be(BandChangeDecision.Rejected);
            supervisor.Reason.Should().Be("Some reasons");
            supervisor.SalaryBand.Should().Be(7);
            response.FinalBand.Should().BeNull();
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
        public async Task ThrowsWhenBandChangeIsAlreadyProcessedByManager()
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

            var managerDecision = new BandChangeApprover
            {
                Name = "A Mananger",
                EmailAddress = "a.manager@hackney.gov.uk",
                Decision = BandChangeDecision.Approved,
                Reason = null,
                SalaryBand = 6
            };

            var bandChange = _fixture.Build<BandChange>()
                .With(bc => bc.Manager, managerDecision)
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
