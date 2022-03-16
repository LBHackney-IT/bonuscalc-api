using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Gateways
{
    public class OvertimeSummaryComparer : IEqualityComparer<OvertimeSummary>
    {
        public bool Equals(OvertimeSummary os1, OvertimeSummary os2)
        {
            return os1.Id == os2.Id && os1.WeekId == os2.WeekId;
        }

        public int GetHashCode(OvertimeSummary os)
        {
            return $"{os.Id}/{os.WeekId}".GetHashCode();
        }
    }

    [TestFixture]
    public class OvertimeSummaryGatewayTests : DatabaseTests
    {
        private OvertimeSummaryGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new OvertimeSummaryGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesOperativeSummariesFromDB()
        {
            // Arrange
            await AddPayElements();

            var overtimeSummary = new OvertimeSummary()
            {
                Id = "123456",
                WeekId = "2021-10-18"
            };

            var otherOvertimeSummary = new OvertimeSummary()
            {
                Id = "123456",
                WeekId = "2021-10-25"
            };

            var comparer = new OvertimeSummaryComparer();

            // Act
            var result = await _classUnderTest.GetOvertimeSummariesAsync("2021-10-18");

            // Assert
            Assert.That(result, Contains.Item(overtimeSummary).Using(comparer));
            Assert.That(result, Does.Not.Contain(otherOvertimeSummary).Using(comparer));
        }

        [Test]
        public async Task RetrievesEmptyResultsFromDB()
        {
            // Act
            var result = await _classUnderTest.GetOvertimeSummariesAsync("2021-10-18");

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
                Id = 401,
                Description = "Overtime Job",
                PayAtBand = false,
                Paid = false,
                Adjustment = false,
                Productive = false,
                NonProductive = false,
                OutOfHours = false,
                Overtime = true,
                Selectable = false,
                SmvPerHour = null,
                CostCode = null
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                Trade = new Trade
                {
                    Id = "EL",
                    Description = "Electrician"
                },
                Scheme = new Scheme
                {
                    Id = 1,
                    Type = "SMV",
                    Description = "Reactive",
                    ConversionFactor = 1.0M
                },
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
                            Value = 21.6M,
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
                            Value = 21.6M,
                            ReadOnly = true
                        }
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.PayElementTypes.AddAsync(payElementType);
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.Timesheets.AddRangeAsync(timesheets);
            await BonusCalcContext.SaveChangesAsync();
        }
    }
}
