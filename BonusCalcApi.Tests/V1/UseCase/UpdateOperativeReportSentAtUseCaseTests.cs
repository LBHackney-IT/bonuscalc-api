using System;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Exceptions;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class UpdateOperativeReportSentAtUseCaseTests
    {
        private UpdateOperativeReportSentAtUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<ITimesheetGateway> _timesheetGatewayMock;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;

            _timesheetGatewayMock = new Mock<ITimesheetGateway>();

            _classUnderTest = new UpdateOperativeReportSentAtUseCase(_timesheetGatewayMock.Object, InMemoryDb.DbSaver);
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task ReportSentAtIsUpdateWhenNull()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var timesheet = CreateExistingTimesheet(null);

            // Act
            await _classUnderTest.ExecuteAsync(timesheet.OperativeId, timesheet.WeekId);

            // Assert
            timesheet.ReportSentAt.Should().BeOnOrAfter(now);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ReportSentAtIsNotUpdatedWhenNotNull()
        {
            // Arrange
            var reportSentAt = new DateTime(2021, 10, 22, 16, 0, 0, DateTimeKind.Utc);
            var timesheet = CreateExistingTimesheet(reportSentAt);

            // Act
            await _classUnderTest.ExecuteAsync(timesheet.OperativeId, timesheet.WeekId);

            // Assert
            timesheet.ReportSentAt.Should().Be(reportSentAt);
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotFoundWhenTimesheetDoesntExist()
        {
            // Arrange
            _timesheetGatewayMock.Setup(x => x.GetOperativeTimesheetAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(null as Timesheet);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("1", "1");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        private Timesheet CreateExistingTimesheet(DateTime? reportSentAt)
        {
            var timesheet = _fixture.Build<Timesheet>()
                .With(t => t.Utilisation, 1.0M)
                .With(t => t.ReportSentAt, reportSentAt)
                .Without(t => t.PayElements)
                .Create();
            _timesheetGatewayMock.Setup(x => x.GetOperativeTimesheetAsync(timesheet.OperativeId, timesheet.WeekId))
                .ReturnsAsync(timesheet);
            return timesheet;
        }
    }
}
