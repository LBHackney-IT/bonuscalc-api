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
    public class OperativeProjectionComparer : IEqualityComparer<OperativeProjection>
    {
        public bool Equals(OperativeProjection op1, OperativeProjection op2)
        {
            return op1.Id == op2.Id;
        }

        public int GetHashCode(OperativeProjection op)
        {
            return op.Id.GetHashCode();
        }
    }

    [TestFixture]
    public class OperativeProjectionGatewayTests : DatabaseTests
    {
        private OperativeProjectionGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new OperativeProjectionGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesOperativeProjectionsFromDb()
        {
            // Arrange
            await AddPayElements();

            var operativeProjection = new OperativeProjection()
            {
                Id = "123456/2021-08-02"
            };

            var otherOperativeProjection = new OperativeProjection()
            {
                Id = "123456/2021-11-01"
            };

            var comparer = new OperativeProjectionComparer();

            // Act
            var result = await _classUnderTest.GetAllByBonusPeriodIdAsync("2021-08-02");

            // Assert
            Assert.That(result, Contains.Item(operativeProjection).Using(comparer));
            Assert.That(result, Does.Not.Contain(otherOperativeProjection).Using(comparer));
        }

        private async Task AddPayElements()
        {
            var payElementType = new PayElementType
            {
                Id = 301,
                Description = "Reactive Repairs",
                PayAtBand = false,
                Paid = true,
                Adjustment = false,
                Productive = true,
                NonProductive = false,
                OutOfHours = false,
                Overtime = false,
                Selectable = true,
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
                new Timesheet
                {
                    Id = "123456/2021-08-02",
                    Week = new Week
                    {
                        Id = "2021-08-02",
                        BonusPeriod = new BonusPeriod
                        {
                            Id = "2021-08-02",
                            StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                            Year = 2020,
                            Number = 3,
                            ClosedAt = null
                        },
                        StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                        Number = 1,
                        ClosedAt = null
                    },
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            WorkOrder = "10010001",
                            ClosedAt = new DateTime(2021, 8, 2, 11, 0, 0, DateTimeKind.Utc),
                            Monday = 1.0M,
                            Duration = 1.0M,
                            Value = 60.0M,
                            ReadOnly = true
                        }
                    }
                },
                new Timesheet
                {
                    Id = "123456/2021-11-01",
                    Week = new Week
                    {
                        Id = "2021-11-01",
                        BonusPeriod = new BonusPeriod
                        {
                            Id = "2021-11-01",
                            StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                            Year = 2020,
                            Number = 4,
                            ClosedAt = null
                        },
                        StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                        Number = 1,
                        ClosedAt = null
                    },
                    Operative = operative,
                    PayElements = new List<PayElement>()
                    {
                        new PayElement
                        {
                            PayElementType = payElementType,
                            WorkOrder = "10010002",
                            ClosedAt = new DateTime(2021, 11, 1, 12, 0, 0, DateTimeKind.Utc),
                            Monday = 1.0M,
                            Duration = 1.0M,
                            Value = 60.0M,
                            ReadOnly = true
                        }
                    }
                }
            };

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
