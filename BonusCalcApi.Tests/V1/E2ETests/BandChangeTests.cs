using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.E2ETests
{
    public class BandChangeTests : IntegrationTests<Startup>
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CanGetBonusPeriod()
        {
            // Arrange
            await SeedBonusPeriods();

            // Act
            var (code, response) = await Get<BonusPeriodResponse>($"/api/v1/band-changes/period");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Id, Is.EqualTo("2021-11-01"));
        }

        [Test]
        public async Task CanGetProjectedChanges()
        {
            // Arrange
            await SeedProjections();

            // Act
            var (code, response) = await Get<List<OperativeProjectionResponse>>($"/api/v1/band-changes/projected");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Count, Is.EqualTo(1));
            Assert.That(response[0].Id, Is.EqualTo("123456/2021-11-01"));
            Assert.That(response[0].OperativeId, Is.EqualTo("123456"));
            Assert.That(response[0].OperativeName, Is.EqualTo("An Operative"));
            Assert.That(response[0].Trade, Is.EqualTo("Carpenter (CP)"));
            Assert.That(response[0].Scheme, Is.EqualTo("Reactive"));
            Assert.That(response[0].BandValue, Is.EqualTo(28080.0M));
            Assert.That(response[0].MaxValue, Is.EqualTo(62868.0M));
            Assert.That(response[0].SickDuration, Is.EqualTo(36.0M));
            Assert.That(response[0].TotalValue, Is.EqualTo(13594.5m));
            Assert.That(response[0].Utilisation, Is.EqualTo(1.0M));
            Assert.That(response[0].FixedBand, Is.EqualTo(false));
            Assert.That(response[0].SalaryBand, Is.EqualTo(5));
            Assert.That(response[0].ProjectedBand, Is.EqualTo(1));
        }

        private async Task SeedBonusPeriods()
        {
            var bonusPeriods = new List<BonusPeriod>()
            {
                new BonusPeriod
                {
                    Id = "2021-08-02",
                    StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 3,
                    ClosedAt = new DateTime(2021, 11, 5, 17, 0, 0, DateTimeKind.Utc)
                },
                new BonusPeriod
                {
                    Id = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 4,
                    ClosedAt = null
                },
                new BonusPeriod
                {
                    Id = "2022-01-31",
                    StartAt = new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2022,
                    Number = 1,
                    ClosedAt = null
                }
            };

            await Context.BonusPeriods.AddRangeAsync(bonusPeriods);
            await Context.SaveChangesAsync();
        }

        private async Task SeedProjections()
        {
            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                MinValue = 28080,
                MaxValue = 62868,
                PayBands = new List<PayBand>()
                {
                    new PayBand { Id = 11, Band = 1, Value = 2160 },
                    new PayBand { Id = 12, Band = 2, Value = 2772 },
                    new PayBand { Id = 13, Band = 3, Value = 3132 },
                    new PayBand { Id = 14, Band = 4, Value = 3366 },
                    new PayBand { Id = 15, Band = 5, Value = 3618 },
                    new PayBand { Id = 16, Band = 6, Value = 3888 },
                    new PayBand { Id = 17, Band = 7, Value = 4182 },
                    new PayBand { Id = 18, Band = 8, Value = 4494 },
                    new PayBand { Id = 19, Band = 9, Value = 4836 }
                }
            };

            var trade = new Trade
            {
                Id = "CP",
                Description = "Carpenter"
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                TradeId = "CP",
                SchemeId = 1,
                Section = "H3007",
                SalaryBand = 5,
                Utilisation = 1.0M,
                FixedBand = false,
                IsArchived = false
            };

            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-11-01",
                StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Year = 2021,
                Number = 4,
                ClosedAt = null
            };

            var weeks = new List<Week>()
            {
                new Week
                {
                    Id = "2021-11-01",
                    BonusPeriodId = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Number = 1,
                    ClosedAt = new DateTime(2021, 11, 17, 14, 0, 0, DateTimeKind.Utc),
                    ClosedBy = "bonus.manager@hackney.gov.uk",
                    ReportsSentAt = new DateTime(2021, 11, 17, 14, 0, 0, DateTimeKind.Utc)
                },
                new Week
                {
                    Id = "2021-11-08",
                    BonusPeriodId = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 8, 0, 0, 0, DateTimeKind.Utc),
                    Number = 2,
                    ClosedAt = new DateTime(2021, 11, 24, 14, 0, 0, DateTimeKind.Utc),
                    ClosedBy = "bonus.manager@hackney.gov.uk",
                    ReportsSentAt = new DateTime(2021, 11, 24, 14, 0, 0, DateTimeKind.Utc)
                },
                new Week
                {
                    Id = "2021-11-15",
                    BonusPeriodId = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 15, 0, 0, 0, DateTimeKind.Utc),
                    Number = 3,
                    ClosedAt = new DateTime(2021, 12, 1, 14, 0, 0, DateTimeKind.Utc),
                    ClosedBy = "bonus.manager@hackney.gov.uk",
                    ReportsSentAt = new DateTime(2021, 12, 1, 14, 0, 0, DateTimeKind.Utc)
                },
                new Week
                {
                    Id = "2021-11-22",
                    BonusPeriodId = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 22, 0, 0, 0, DateTimeKind.Utc),
                    Number = 4,
                    ClosedAt = new DateTime(2021, 12, 8, 14, 0, 0, DateTimeKind.Utc),
                    ClosedBy = "bonus.manager@hackney.gov.uk",
                    ReportsSentAt = new DateTime(2021, 12, 8, 14, 0, 0, DateTimeKind.Utc)
                }
            };

            var timesheets = new List<Timesheet>()
            {
                new Timesheet
                {
                    Id = "123456/2021-11-01",
                    OperativeId = "123456",
                    WeekId = "2021-11-01",
                    Utilisation = 1.0M,
                    ReportSentAt = new DateTime(2021, 11, 17, 14, 0, 0, DateTimeKind.Utc)
                },
                new Timesheet
                {
                    Id = "123456/2021-11-08",
                    OperativeId = "123456",
                    WeekId = "2021-11-08",
                    Utilisation = 1.0M,
                    ReportSentAt = new DateTime(2021, 11, 24, 14, 0, 0, DateTimeKind.Utc)
                },
                new Timesheet
                {
                    Id = "123456/2021-11-15",
                    OperativeId = "123456",
                    WeekId = "2021-11-15",
                    Utilisation = 1.0M,
                    ReportSentAt = new DateTime(2021, 12, 1, 14, 0, 0, DateTimeKind.Utc)
                },
                new Timesheet
                {
                    Id = "123456/2021-11-22",
                    OperativeId = "123456",
                    WeekId = "2021-11-22",
                    Utilisation = 1.0M,
                    ReportSentAt = new DateTime(2021, 12, 8, 14, 0, 0, DateTimeKind.Utc)
                },
            };

            var payElementTypes = new List<PayElementType>
            {
                new PayElementType
                {
                    Id = 101,
                    Description = "Dayworks",
                    PayAtBand = false,
                    Paid = true,
                    Adjustment = false,
                    Productive = false,
                    NonProductive = true,
                    OutOfHours = false,
                    Overtime = false,
                    SickLeave = false,
                    Selectable = true,
                    SmvPerHour = null,
                    CostCode = null
                },
                new PayElementType
                {
                    Id = 102,
                    Description = "Annual Leave",
                    PayAtBand = true,
                    Paid = true,
                    Adjustment = false,
                    Productive = false,
                    NonProductive = true,
                    OutOfHours = false,
                    Overtime = false,
                    SickLeave = false,
                    Selectable = true,
                    SmvPerHour = null,
                    CostCode = null
                },
                new PayElementType
                {
                    Id = 104,
                    Description = "Sick",
                    PayAtBand = true,
                    Paid = true,
                    Adjustment = false,
                    Productive = false,
                    NonProductive = true,
                    OutOfHours = false,
                    Overtime = false,
                    SickLeave = true,
                    Selectable = true,
                    SmvPerHour = null,
                    CostCode = null
                },
                new PayElementType
                {
                    Id = 301,
                    Description = "Reactive Repairs",
                    PayAtBand = false,
                    Paid = false,
                    Adjustment = false,
                    Productive = true,
                    NonProductive = false,
                    OutOfHours = false,
                    Overtime = false,
                    SickLeave = false,
                    Selectable = false,
                    SmvPerHour = null,
                    CostCode = null
                }
            };

            var payElements = new List<PayElement>()
            {
                new PayElement
                {
                    TimesheetId = "123456/2021-11-01",
                    PayElementTypeId = 301,
                    WorkOrder = "10001001",
                    Address = "1 Somewhere Road",
                    ClosedAt = new DateTime(2021, 11, 3, 11, 0, 0, DateTimeKind.Utc),
                    Wednesday = 7.25M,
                    Duration = 7.25M,
                    Value = 435.0M,
                    ReadOnly = true
                },
                new PayElement
                {
                    TimesheetId = "123456/2021-11-01",
                    PayElementTypeId = 301,
                    WorkOrder = "10001002",
                    Address = "2 Knowhere Road",
                    ClosedAt = new DateTime(2021, 11, 4, 11, 0, 0, DateTimeKind.Utc),
                    Thursday = 7.25M,
                    Duration = 7.25M,
                    Value = 435.0M,
                    ReadOnly = true
                },
                new PayElement
                {
                    TimesheetId = "123456/2021-11-01",
                    PayElementTypeId = 101,
                    Monday = 7.25M,
                    Tuesday = 7.25M,
                    Friday = 7.0M,
                    Duration = 21.5M,
                    Value = 1870.5M,
                    ReadOnly = false
                },
                new PayElement
                {
                    TimesheetId = "123456/2021-11-08",
                    PayElementTypeId = 102,
                    Monday = 7.25M,
                    Tuesday = 7.25M,
                    Wednesday = 7.25M,
                    Thursday = 7.25M,
                    Friday = 7.0M,
                    Duration = 36.0M,
                    Value = 3618.0M,
                    ReadOnly = false
                },
                new PayElement
                {
                    TimesheetId = "123456/2021-11-15",
                    PayElementTypeId = 102,
                    Monday = 7.25M,
                    Tuesday = 7.25M,
                    Wednesday = 7.25M,
                    Thursday = 7.25M,
                    Friday = 7.0M,
                    Duration = 36.0M,
                    Value = 3618.0M,
                    ReadOnly = false
                },
                new PayElement
                {
                    TimesheetId = "123456/2021-11-22",
                    PayElementTypeId = 104,
                    Monday = 7.25M,
                    Tuesday = 7.25M,
                    Wednesday = 7.25M,
                    Thursday = 7.25M,
                    Friday = 7.0M,
                    Duration = 36.0M,
                    Value = 3618.0M,
                    ReadOnly = false
                }
            };

            await Context.Schemes.AddAsync(scheme);
            await Context.Trades.AddAsync(trade);
            await Context.Operatives.AddAsync(operative);
            await Context.BonusPeriods.AddAsync(bonusPeriod);
            await Context.Weeks.AddRangeAsync(weeks);
            await Context.Timesheets.AddRangeAsync(timesheets);
            await Context.PayElementTypes.AddRangeAsync(payElementTypes);
            await Context.PayElements.AddRangeAsync(payElements);

            await Context.SaveChangesAsync();
        }
    }
}
