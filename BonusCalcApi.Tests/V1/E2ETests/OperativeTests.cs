using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.E2ETests
{
    public class OperativeTests : IntegrationTests<Startup>
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
        }

        [Test]
        public async Task CanGetOperative()
        {
            // Arrange
            var operative = await SeedOperative();

            // Act
            var (code, response) = await Get<OperativeResponse>($"/api/v1/operatives/{operative.Id}");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(operative.ToResponse());
        }

        [Test]
        public async Task CanGetTimesheet()
        {
            // Arrange
            var operative = await SeedOperative();
            var timesheet = await SeedTimesheet(operative);

            // Act
            var (code, response) = await Get<TimesheetResponse>($"/api/v1/operatives/{operative.Id}/timesheet?week={timesheet.WeekId}");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(timesheet.ToResponse(), options =>
                options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation)).WhenTypeIs<DateTime>()
                );
        }

        private async Task<Timesheet> SeedTimesheet(Operative operative)
        {
            var week = _fixture.Create<Week>();
            await BonusCalcContext.Weeks.AddAsync(week);
            var timesheet = _fixture.Build<Timesheet>()
                .With(t => t.OperativeId, operative.Id)
                .Without(t => t.Operative)
                .With(t => t.WeekId, week.Id)
                .Without(t => t.Week)
                .Create();
            await BonusCalcContext.Timesheets.AddAsync(timesheet);
            await BonusCalcContext.SaveChangesAsync();
            return timesheet;
        }

        private async Task<Operative> SeedOperative()
        {
            var operative = FixtureHelpers.CreateOperative();
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();
            return operative;
        }
    }
}
