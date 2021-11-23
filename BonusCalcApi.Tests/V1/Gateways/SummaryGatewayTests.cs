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
    public class SummaryGatewayTests : DatabaseTests
    {
        private SummaryGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new SummaryGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesSummaryFromDB()
        {
            // Arrange
            var operative = await AddOperative();
            var bonusPeriod = await AddBonusPeriod();
            await AddTimesheets(operative);

            var summary = new Summary()
            {
                Id = "123456/2021-08-02",
                OperativeId = "123456",
                BonusPeriodId = "2021-08-02",
                BonusPeriod = bonusPeriod,
                WeeklySummaries = new List<WeeklySummary>()
                {
                    new WeeklySummary
                    {
                        Id = "123456/2021-08-02/2021-08-02",
                        SummaryId = "123456/2021-08-02",
                        StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                        Number = 1,
                        ClosedAt = null,
                        ProductiveValue = 0.0M,
                        NonProductiveDuration = 5.0M,
                        NonProductiveValue = 500.0M,
                        TotalValue = 500.0M,
                        ProjectedValue = 500.0M
                    },
                    new WeeklySummary
                    {
                        Id = "123456/2021-08-02/2021-08-09",
                        SummaryId = "123456/2021-08-02",
                        StartAt = new DateTime(2021, 8, 8, 23, 0, 0, DateTimeKind.Utc),
                        Number = 2,
                        ClosedAt = null,
                        ProductiveValue = 0.0M,
                        NonProductiveDuration = 3.0M,
                        NonProductiveValue = 300.0M,
                        TotalValue = 300.0M,
                        ProjectedValue = 400.0M
                    },
                    new WeeklySummary
                    {
                        Id = "123456/2021-08-02/2021-08-16",
                        SummaryId = "123456/2021-08-02",
                        StartAt = new DateTime(2021, 8, 15, 23, 0, 0, DateTimeKind.Utc),
                        Number = 3,
                        ClosedAt = null,
                        ProductiveValue = 100.0M,
                        NonProductiveDuration = 0.0M,
                        NonProductiveValue = 0.0M,
                        TotalValue = 100.0M,
                        ProjectedValue = 300.0M
                    }
                }
            };

            // Act
            var result = await _classUnderTest.GetOperativeSummaryAsync(operative.Id, bonusPeriod.Id);

            // Assert
            result.Should().BeEquivalentTo(summary);
        }

        [Test]
        public async Task RetrievesNonExistentSummaryFromDB()
        {
            // Act
            var result = await _classUnderTest.GetOperativeSummaryAsync("000000", "2000-01-01");

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
                EmailAddress = "an.operative@hackney.gov.uk",
                Trade = trade,
                Scheme = scheme,
                Section = "H3007",
                SalaryBand = 5,
                Utilisation = 1.0M,
                FixedBand = false,
                IsArchived = false
            };

            await BonusCalcContext.Trades.AddAsync(trade);
            await BonusCalcContext.Schemes.AddAsync(scheme);
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();

            return operative;
        }

        private async Task<BonusPeriod> AddBonusPeriod()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2020,
                Number = 3,
                ClosedAt = null,
                Weeks = new List<Week>()
                {
                    new Week
                    {
                        Id = "2021-08-02",
                        StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                        Number = 1,
                        ClosedAt = null
                    },
                    new Week
                    {
                        Id = "2021-08-09",
                        StartAt = new DateTime(2021, 8, 8, 23, 0, 0, DateTimeKind.Utc),
                        Number = 2,
                        ClosedAt = null
                    },
                    new Week
                    {
                        Id = "2021-08-16",
                        StartAt = new DateTime(2021, 8, 15, 23, 0, 0, DateTimeKind.Utc),
                        Number = 3,
                        ClosedAt = null
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.SaveChangesAsync();

            return bonusPeriod;
        }

        private async Task AddTimesheets(Operative operative)
        {
            var nonProductiveType = new PayElementType
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
                Selectable = true
            };

            var productiveType = new PayElementType
            {
                Id = 301,
                Description = "Reactive Repairs",
                PayAtBand = true,
                Paid = true,
                Adjustment = false,
                Productive = true,
                NonProductive = false,
                OutOfHours = false,
                Overtime = false,
                Selectable = false
            };

            var timesheets = new List<Timesheet>()
            {
                new Timesheet
                {
                    Id = "123456/2021-08-02",
                    WeekId = "2021-08-02",
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = nonProductiveType,
                            Monday = 1.0M,
                            Tuesday = 1.0M,
                            Wednesday = 1.0M,
                            Thursday = 1.0M,
                            Friday = 1.0M,
                            Duration = 5.0M,
                            Value = 500.0M,
                            ReadOnly = false
                        }
                    }
                },
                new Timesheet
                {
                    Id = "123456/2021-08-09",
                    WeekId = "2021-08-09",
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = nonProductiveType,
                            Monday = 1.0M,
                            Tuesday = 1.0M,
                            Wednesday = 1.0M,
                            Thursday = 0.0M,
                            Friday = 0.0M,
                            Duration = 3.0M,
                            Value = 300.0M,
                            ReadOnly = false
                        }
                    }
                },
                new Timesheet
                {
                    Id = "123456/2021-08-16",
                    WeekId = "2021-08-16",
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = productiveType,
                            WorkOrder = "1000001",
                            Address = "1 Knowhere Road",
                            Comment = "Fix broken light switch",
                            Tuesday = 100.0M,
                            Value = 100.0M,
                            ReadOnly = true,
                            ClosedAt = new DateTime(2021, 8, 17, 14, 0, 0, DateTimeKind.Utc)
                        }
                    }
                }
            };

            await BonusCalcContext.PayElementTypes.AddAsync(nonProductiveType);
            await BonusCalcContext.PayElementTypes.AddAsync(productiveType);
            await BonusCalcContext.Timesheets.AddRangeAsync(timesheets);
            await BonusCalcContext.SaveChangesAsync();
        }
    }
}
