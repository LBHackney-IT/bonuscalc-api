using System;
using System.Collections.Generic;
using System.Linq;
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
    public class StartBandChangeProcessUseCaseTests
    {
        private Fixture _fixture;
        private Mock<IBonusPeriodGateway> _bonusPeriodGatewayMock;
        private Mock<IOperativeProjectionGateway> _operativeProjectionGatewayMock;
        private StartBandChangeProcessUseCase _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
            _bonusPeriodGatewayMock = new Mock<IBonusPeriodGateway>();
            _operativeProjectionGatewayMock = new Mock<IOperativeProjectionGateway>();

            _classUnderTest = new StartBandChangeProcessUseCase(
                _bonusPeriodGatewayMock.Object,
                _operativeProjectionGatewayMock.Object,
                InMemoryDb.DbSaver
            );
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CopiesProjectionsToBandChanges()
        {
            // Arrange
            var closedWeek = _fixture.Build<Week>()
                .With(w => w.ClosedAt, DateTime.UtcNow)
                .With(w => w.ClosedBy, "a.week.manager@hackney.gov.uk")
                .Create();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .Without(bp => bp.ClosedAt)
                .With(bp => bp.BandChanges, new List<BandChange>())
                .With(bp => bp.Weeks, new List<Week>() { closedWeek })
                .Create();

            var operativeProjection = _fixture.Build<OperativeProjection>().Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            _operativeProjectionGatewayMock
                .Setup(x => x.GetAllByBonusPeriodIdAsync(bonusPeriod.Id))
                .ReturnsAsync(new List<OperativeProjection>() { operativeProjection });

            // Act
            await _classUnderTest.ExecuteAsync();

            // Assert
            var bandChange = bonusPeriod.BandChanges.Single();
            bandChange.Id.Should().Be(operativeProjection.Id);
            bandChange.BonusPeriodId.Should().Be(operativeProjection.BonusPeriodId);
            bandChange.OperativeId.Should().Be(operativeProjection.OperativeId);
            bandChange.Trade.Should().Be(operativeProjection.Trade);
            bandChange.Scheme.Should().Be(operativeProjection.Scheme);
            bandChange.BandValue.Should().Be(operativeProjection.BandValue);
            bandChange.MaxValue.Should().Be(operativeProjection.MaxValue);
            bandChange.SickDuration.Should().Be(operativeProjection.SickDuration);
            bandChange.TotalValue.Should().Be(operativeProjection.TotalValue);
            bandChange.Utilisation.Should().Be(operativeProjection.Utilisation);
            bandChange.FixedBand.Should().Be(operativeProjection.FixedBand);
            bandChange.SalaryBand.Should().Be(operativeProjection.SalaryBand);
            bandChange.ProjectedBand.Should().Be(operativeProjection.ProjectedBand);
            bandChange.Supervisor.Name.Should().Be(operativeProjection.SupervisorName);
            bandChange.Supervisor.EmailAddress.Should().Be(operativeProjection.SupervisorEmailAddress);
            bandChange.Manager.Name.Should().Be(operativeProjection.ManagerName);
            bandChange.Manager.EmailAddress.Should().Be(operativeProjection.ManagerEmailAddress);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ThrowsWhenNoBonusPeriodOpen()
        {
            // Arrange
            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(null as BonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync();

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodHasOpenWeeks()
        {
            // Arrange
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
            var act = async () => await _classUnderTest.ExecuteAsync();

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsWhenBonusPeriodIsClosed()
        {
            // Arrange
            var bandChanges = _fixture.Build<BandChange>().CreateMany();

            var bonusPeriod = _fixture.Build<BonusPeriod>()
                .With(bp => bp.BandChanges, bandChanges.ToList)
                .With(bp => bp.ClosedAt, DateTime.UtcNow)
                .Create();

            _bonusPeriodGatewayMock
                .Setup(x => x.GetEarliestOpenBonusPeriodAsync())
                .ReturnsAsync(bonusPeriod);

            // Act
            var act = async () => await _classUnderTest.ExecuteAsync();

            // Assert
            await act.Should().ThrowAsync<ResourceNotProcessableException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }
    }
}
