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
    public class UpdateWeekReportsSentAtUseCaseTests
    {
        private UpdateWeekReportsSentAtUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IWeekGateway> _weekGatewayMock;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;

            _weekGatewayMock = new Mock<IWeekGateway>();

            _classUnderTest = new UpdateWeekReportsSentAtUseCase(_weekGatewayMock.Object, InMemoryDb.DbSaver);
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task ReportsSentAtIsUpdateWhenNull()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var week = CreateExistingWeek(null);

            // Act
            await _classUnderTest.ExecuteAsync(week.Id);

            // Assert
            week.ReportsSentAt.Should().BeOnOrAfter(now);
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task ReportsSentAtIsNotUpdatedWhenNotNull()
        {
            // Arrange
            var reportsSentAt = new DateTime(2021, 10, 22, 16, 0, 0, DateTimeKind.Utc);
            var week = CreateExistingWeek(reportsSentAt);

            // Act
            await _classUnderTest.ExecuteAsync(week.Id);

            // Assert
            week.ReportsSentAt.Should().Be(reportsSentAt);
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ThrowsResourceNotFoundWhenWeekDoesntExist()
        {
            // Arrange
            _weekGatewayMock.Setup(x => x.GetWeekAsync(It.IsAny<string>()))
                .ReturnsAsync(null as Week);

            // Act
            Func<Task> act = async () => await _classUnderTest.ExecuteAsync("1");

            // Assert
            await act.Should().ThrowAsync<ResourceNotFoundException>();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        private Week CreateExistingWeek(DateTime? reportsSentAt)
        {
            var week = _fixture.Build<Week>()
                .With(t => t.ReportsSentAt, reportsSentAt)
                .Create();
            _weekGatewayMock.Setup(x => x.GetWeekAsync(week.Id))
                .ReturnsAsync(week);
            return week;
        }
    }
}
