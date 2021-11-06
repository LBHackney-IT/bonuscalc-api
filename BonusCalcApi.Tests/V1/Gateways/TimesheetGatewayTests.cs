using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class TimesheetGatewayTests
    {
        private TimesheetGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TimesheetGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task RetrievesTimesheetFromDB()
        {
            // Arrange
            var operative = await AddOperative();
            var week = await AddWeek();

            var expectedTimesheet = new Timesheet
            {
                WeekId = week.Id,
                OperativeId = operative.Id
            };
            await AddTimesheets(expectedTimesheet, week);

            // Act
            var result = await _classUnderTest.GetOperativesTimesheetAsync(operative.Id, week.Id);

            // Assert
            result.Should().BeEquivalentTo(expectedTimesheet);
        }

        [Test]
        public async Task RetrievesNonExistentTimesheetFromDB()
        {
            // Act
            var result = await _classUnderTest.GetOperativesTimesheetAsync("1234", "4569");

            // Assert
            result.Should().BeNull();
        }

        private static async Task AddTimesheets(Timesheet expectedTimesheet, Week week)
        {

            var timeSheets = new List<Timesheet>
            {
                expectedTimesheet,
                new Timesheet
                {
                    WeekId = week.Id, OperativeId = "4321"
                }
            };
            await InMemoryDb.Instance.Timesheets.AddRangeAsync(timeSheets);
            await InMemoryDb.Instance.SaveChangesAsync();
        }

        private static async Task<Week> AddWeek()
        {

            var week = new Week
            {
                Id = "2000-01-31"
            };
            await InMemoryDb.Instance.Weeks.AddAsync(week);
            await InMemoryDb.Instance.SaveChangesAsync();
            return week;
        }

        private static async Task<Operative> AddOperative()
        {

            var operative = new Operative
            {
                Id = "1234"
            };
            await InMemoryDb.Instance.Operatives.AddAsync(operative);
            await InMemoryDb.Instance.SaveChangesAsync();
            return operative;
        }
    }
}
