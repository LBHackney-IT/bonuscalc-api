using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class TimesheetGatewayTests : DatabaseTests
    {
        private TimesheetGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new TimesheetGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesTimesheetFromDB()
        {
            // Arrange
            var operative = await AddOperative();
            var week = await AddWeek();
            var timesheet = await AddTimesheet(operative, week);

            // Act
            var result = await _classUnderTest.GetOperativeTimesheetAsync(operative.Id, week.Id);

            // Assert
            result.Should().BeEquivalentTo(timesheet);
        }

        [Test]
        public async Task RetrievesNonExistentTimesheetFromDB()
        {
            // Act
            var result = await _classUnderTest.GetOperativeTimesheetAsync("000000", "2000-01-01");

            // Assert
            result.Should().BeNull();
        }

        private async Task<Operative> AddOperative()
        {
            var trade = new Trade
            {
                Id = "EL",
                Description = "Electrician"
            };

            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                ConversionFactor = 1.0M
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                Trade = trade,
                Scheme = scheme,
                Section = "H3007",
                SalaryBand = 5,
                FixedBand = false,
                IsArchived = false
            };

            await BonusCalcContext.Trades.AddAsync(trade);
            await BonusCalcContext.Schemes.AddAsync(scheme);
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();

            return operative;
        }

        private async Task<Week> AddWeek()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2020,
                Number = 3,
                ClosedAt = null
            };

            var week = new Week
            {
                Id = "2021-10-18",
                BonusPeriod = bonusPeriod,
                StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                Number = 12,
                ClosedAt = null
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.Weeks.AddAsync(week);
            await BonusCalcContext.SaveChangesAsync();

            return week;
        }

        private async Task<Timesheet> AddTimesheet(Operative operative, Week week)
        {
            var payElementType = new PayElementType
            {
                Id = 101,
                Description = "Dayworks",
                PayAtBand = false,
                Paid = true,
                Adjustment = false,
                Productive = false,
                NonProductive = true,
                OutOfHours = false,
                Overtime = false
            };

            var timesheet = new Timesheet
            {
                Week = week,
                Operative = operative,
                PayElements = new List<PayElement>()
                {
                    new PayElement
                    {
                        PayElementType = payElementType,
                        Monday = 1.0M,
                        Tuesday = 1.0M,
                        Wednesday = 1.0M,
                        Thursday = 1.0M,
                        Friday = 0.0M,
                        Duration = 4.0M,
                        Value = 348.0M,
                        ReadOnly = false
                    }
                }
            };

            await BonusCalcContext.PayElementTypes.AddAsync(payElementType);
            await BonusCalcContext.Timesheets.AddAsync(timesheet);
            await BonusCalcContext.SaveChangesAsync();

            return timesheet;
        }
    }
}
