using System;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Gateways.Interfaces;
using BonusCalcApi.V1.Infrastructure;
using BonusCalcApi.V1.UseCase;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Extensions;
using Moq;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.UseCase
{
    public class UpdateWeekUseCaseTests
    {
        private UpdateWeekUseCase _classUnderTest;
        private Fixture _fixture;
        private Mock<IWeekGateway> _weekGatewayMock;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;

            _weekGatewayMock = new Mock<IWeekGateway>();

            _classUnderTest = new UpdateWeekUseCase(_weekGatewayMock.Object, InMemoryDb.DbSaver);
        }

        [TearDown]
        public void Teardown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task UpdatesWeek()
        {
            // Arrange
            var closedAt = new DateTime(2021, 10, 29, 16, 0, 0, DateTimeKind.Utc);
            var closedBy = "a.manager@hackney.gov.uk";
            var week = CreateWeek(null, null);
            var request = CreateRequest(closedAt, closedBy);

            // Act
            var updatedWeek = await _classUnderTest.ExecuteAsync(week.Id, request);

            // Assert
            updatedWeek.ClosedAt.Should().Be(29.October(2021).At(16, 0));
            updatedWeek.ClosedBy.Should().Be("a.manager@hackney.gov.uk");
            InMemoryDb.DbSaver.VerifySaveCalled();
        }

        [Test]
        public async Task DoesNotUpdateWeekIfAlreadyClosed()
        {
            // Arrange
            var closedAt = new DateTime(2021, 10, 29, 16, 0, 0, DateTimeKind.Utc);
            var closedBy = "a.manager@hackney.gov.uk";
            var newClosedAt = new DateTime(2021, 10, 29, 17, 0, 0, DateTimeKind.Utc);
            var newClosedBy = "other.manager@hackney.gov.uk";
            var week = CreateWeek(closedAt, closedBy);
            var request = CreateRequest(newClosedAt, newClosedBy);

            // Act
            var updatedWeek = await _classUnderTest.ExecuteAsync(week.Id, request);

            // Assert
            updatedWeek.ClosedAt.Should().Be(29.October(2021).At(16, 0));
            updatedWeek.ClosedBy.Should().Be("a.manager@hackney.gov.uk");
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task DoesNotReopenWeekIfStillOpen()
        {
            // Arrange
            var week = CreateWeek(null, null);
            var request = CreateRequest(null, null);

            // Act
            var updatedWeek = await _classUnderTest.ExecuteAsync(week.Id, request);

            // Assert
            updatedWeek.ClosedAt.Should().BeNull();
            updatedWeek.ClosedBy.Should().BeNull();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        [Test]
        public async Task ReturnsNullIfWeekNotFound()
        {
            // Arrange
            var closedAt = new DateTime(2021, 10, 29, 16, 0, 0, DateTimeKind.Utc);
            var closedBy = "a.manager@hackney.gov.uk";
            var week = CreateWeek(null, null);
            var request = CreateRequest(closedAt, closedBy);

            // Act
            var updatedWeek = await _classUnderTest.ExecuteAsync("2021-10-19", request);

            // Assert
            updatedWeek.Should().BeNull();
            InMemoryDb.DbSaver.VerifySaveNotCalled();
        }

        private Week CreateWeek(DateTime? closedAt, string closedBy)
        {
            var week = _fixture.Build<Week>()
                .With(w => w.Id, "2021-10-18")
                .With(w => w.ClosedAt, closedAt)
                .With(w => w.ClosedBy, closedBy)
                .Create();

            _weekGatewayMock.Setup(x => x.GetWeekAsync(week.Id))
                .ReturnsAsync(week);

            return week;
        }

        private WeekUpdate CreateRequest(DateTime? closedAt, string closedBy)
        {
            var request = _fixture.Build<WeekUpdate>()
                .With(w => w.ClosedAt, closedAt)
                .With(w => w.ClosedBy, closedBy)
                .Create();

            return request;
        }
    }
}
