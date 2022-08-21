using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class OutOfHoursSummaryComparer : IEqualityComparer<OutOfHoursSummary>
    {
        public bool Equals(OutOfHoursSummary os1, OutOfHoursSummary os2)
        {
            return os1.Id == os2.Id && os1.WeekId == os2.WeekId;
        }

        public int GetHashCode(OutOfHoursSummary os)
        {
            return $"{os.Id}/{os.WeekId}".GetHashCode();
        }
    }

    [TestFixture]
    public class OutOfHoursSummaryGatewayTests : DatabaseTests
    {
        private OutOfHoursSummaryGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new OutOfHoursSummaryGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesOperativeSummariesFromDb()
        {
            // Arrange
            await AddPayElements();

            var overtimeSummary = new OutOfHoursSummary()
            {
                Id = "123456",
                WeekId = "2021-10-18"
            };

            var otherOutOfHoursSummary = new OutOfHoursSummary()
            {
                Id = "123456",
                WeekId = "2021-10-25"
            };

            var comparer = new OutOfHoursSummaryComparer();

            // Act
            var result = await _classUnderTest.GetOutOfHoursSummariesAsync("2021-10-18");

            // Assert
            Assert.That(result, Contains.Item(overtimeSummary).Using(comparer));
            Assert.That(result, Does.Not.Contain(otherOutOfHoursSummary).Using(comparer));
        }

        [Test]
        public async Task RetrievesEmptyResultsFromDb()
        {
            // Act
            var result = await _classUnderTest.GetOutOfHoursSummariesAsync("2021-10-18");

            // Assert
            Assert.That(result, Is.Empty);
        }

        private async Task AddPayElements()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2020,
                Number = 3,
                ClosedAt = null
            };

            var payElementType = new PayElementType
            {
                Id = 501,
                Description = "OOH Job",
                PayAtBand = false,
                Paid = false,
                Adjustment = false,
                Productive = false,
                NonProductive = false,
                OutOfHours = true,
                Overtime = false,
                Selectable = false,
                SmvPerHour = null,
                CostCode = null
            };

            var scheme = new Scheme
            {
                Id = 1,
                Type = "SMV",
                Description = "Reactive",
                ConversionFactor = 1.0M,
                MaxValue = 62868.0M,
                PayBands = new List<PayBand>
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
                Id = "EL",
                Description = "Electrician"
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                TradeId = "EL",
                SchemeId = 1,
                Section = "H3007",
                SalaryBand = 5,
                Utilisation = 1.0M,
                FixedBand = false,
                IsArchived = false
            };

            var timesheets = new List<Timesheet>()
            {
                new Timesheet()
                {
                    Id = "123456/2021-10-18",
                    Week = new Week
                    {
                        Id = "2021-10-18",
                        BonusPeriod = bonusPeriod,
                        StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                        Number = 12,
                        ClosedAt = null
                    },
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement()
                        {
                            PayElementType = payElementType,
                            Value = 21.98M,
                            ReadOnly = true
                        }
                    }
                },
                new Timesheet()
                {
                    Id = "123456/2021-10-25",
                    Week = new Week
                    {
                        Id = "2021-10-25",
                        BonusPeriod = bonusPeriod,
                        StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                        Number = 13,
                        ClosedAt = null
                    },
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement()
                        {
                            PayElementType = payElementType,
                            Value = 21.98M,
                            ReadOnly = true
                        }
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.PayElementTypes.AddAsync(payElementType);
            await BonusCalcContext.Trades.AddAsync(trade);
            await BonusCalcContext.Schemes.AddAsync(scheme);
            await BonusCalcContext.SaveChangesAsync();

            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();

            await BonusCalcContext.Timesheets.AddRangeAsync(timesheets);
            await BonusCalcContext.SaveChangesAsync();

            BonusCalcContext.ChangeTracker.Clear();
        }
    }
}
