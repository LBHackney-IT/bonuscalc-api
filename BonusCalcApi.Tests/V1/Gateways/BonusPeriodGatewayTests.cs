using BonusCalcApi.V1.Gateways;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BonusCalcApi.Tests.V1.Gateways
{
    [TestFixture]
    public class BonusPeriodGatewayTests : DatabaseTests
    {
        private BonusPeriodGateway _classUnderTest;
        private DateTime _currentDate;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new BonusPeriodGateway(BonusCalcContext);
            _currentDate = new DateTime(2021, 12, 5, 16, 0, 0, DateTimeKind.Utc);
        }

        [Test]
        public async Task ClosesBonusPeriodInDb()
        {
            // Arrange
            var closedAt = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc);
            var closedBy = "a.manager@hackney.gov.uk";

            var bonusPeriods = new List<BonusPeriod>
            {
                new BonusPeriod
                {
                    Id = "2022-01-31",
                    StartAt = new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2022,
                    Number = 1,
                    ClosedAt = null,
                    Weeks = new List<Week>
                    {
                        new Week {
                            Id = "2022-01-31",
                            StartAt = new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                            ClosedAt = new DateTime(2022, 2, 9, 16, 0, 0, DateTimeKind.Utc),
                            Number = 1
                        }
                    }
                },
                new BonusPeriod
                {
                    Id = "2022-05-02",
                    StartAt = new DateTime(2022, 5, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2022,
                    Number = 2,
                    ClosedAt = null,
                    Weeks = new List<Week>
                    {
                        new Week {
                            Id = "2022-05-02",
                            StartAt = new DateTime(2022, 5, 1, 23, 0, 0, DateTimeKind.Utc),
                            Number = 1
                        }
                    }
                }
            };

            var payElementTypes = new List<PayElementType>
            {
                new PayElementType
                {
                    Id = 102,
                    Description = "Annual Leave",
                    PayAtBand = true,
                    Paid = true,
                    NonProductive = true
                },
                new PayElementType
                {
                    Id = 202,
                    Description = "Balance Brought Forward",
                    Adjustment = true,
                    Productive = true
                }
            };

            var operative = new Operative
            {
                Id = "123456",
                Name = "Alex Cable",
                Section = "H3009",
                SalaryBand = 6,
                Trade = new Trade
                {
                    Id = "EL",
                    Description = "Electrician"
                },
                Scheme = new Scheme
                {
                    Id = 1,
                    Type = "SMV",
                    Description = "Planned",
                    PayBands = new List<PayBand>
                    {
                        new PayBand { Id = 11, Band = 1, Value = 2160 },
                        new PayBand { Id = 12, Band = 2, Value = 2700 },
                        new PayBand { Id = 13, Band = 3, Value = 3132 },
                        new PayBand { Id = 14, Band = 4, Value = 3348 },
                        new PayBand { Id = 15, Band = 5, Value = 3564 },
                        new PayBand { Id = 16, Band = 6, Value = 3780 },
                        new PayBand { Id = 17, Band = 7, Value = 3996 },
                        new PayBand { Id = 18, Band = 8, Value = 4320 },
                        new PayBand { Id = 19, Band = 9, Value = 4644 }
                    }
                }
            };

            var timesheet = new Timesheet
            {
                Id = "123456/2022-05-02",
                WeekId = "2022-05-02",
                OperativeId = "123456",
                Utilisation = 1.0m
            };

            var payElement = new PayElement
            {
                TimesheetId = "123456/2022-05-02",
                PayElementTypeId = 102,
                Duration = 36.0m,
                Value = 3780.0m,
                Monday = 7.25m,
                Tuesday = 7.25m,
                Wednesday = 7.25m,
                Thursday = 7.25m,
                Friday = 7.0m,
                Saturday = 0.0m,
                Sunday = 0.0m
            };

            var bandChange = new BandChange
            {
                Id = "123456/2022-01-31",
                OperativeId = "123456",
                BonusPeriodId = "2022-01-31",
                Trade = "Electrician (EL)",
                Scheme = "Planned",
                BandValue = 51948.0m,
                MaxValue = 60372.0m,
                SickDuration = 0.0m,
                TotalValue = 52948.0m,
                Utilisation = 1.0m,
                FixedBand = false,
                SalaryBand = 6,
                ProjectedBand = 7,
                Supervisor = new BandChangeApprover
                {
                    Name = "A Supervisor",
                    EmailAddress = "a.supervisor@hackney.gov.uk",
                    Decision = BandChangeDecision.Approved,
                    Reason = null,
                    SalaryBand = 7
                },
                FinalBand = 7
            };

            await BonusCalcContext.AddRangeAsync(bonusPeriods);
            await BonusCalcContext.AddRangeAsync(payElementTypes);

            await BonusCalcContext.AddAsync(operative);
            await BonusCalcContext.AddAsync(timesheet);
            await BonusCalcContext.AddAsync(payElement);
            await BonusCalcContext.AddAsync(bandChange);

            await BonusCalcContext.SaveChangesAsync();

            BonusCalcContext.ChangeTracker.Clear();

            // Act
            var result = await _classUnderTest.CloseBonusPeriodAsync("2022-01-31", 202, closedAt, closedBy);

            // Assert bonus period has been closed
            result.ClosedAt.Should().Be(closedAt);
            result.ClosedBy.Should().Be(closedBy);

            // Assert that it recalculates non-productive elements
            BonusCalcContext.PayElements.Should().ContainSingle(pe =>
                pe.PayElementTypeId == 102 &&
                pe.TimesheetId == "123456/2022-05-02" &&
                pe.Value == 3996.0m);

            // Assert that it adds any carried-over SMVs
            BonusCalcContext.PayElements.Should().ContainSingle(pe =>
                pe.PayElementTypeId == 202 &&
                pe.TimesheetId == "123456/2022-05-02" &&
                pe.Value == 1000.0m);

            // Assert that it updates the operative's salary band
            BonusCalcContext.Operatives.Should().ContainSingle(o =>
                o.Id == "123456" &&
                o.SalaryBand == 7);
        }

        [Test]
        public async Task CreatesBonusPeriodInDb()
        {
            // Arrange
            var bonusPeriods = new List<BonusPeriod>()
            {
                new BonusPeriod
                {
                    Id = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 4,
                    ClosedAt = new DateTime(2022, 02, 11, 17, 0, 0, DateTimeKind.Utc)
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

            var bonusPeriod = new BonusPeriod()
            {
                Id = "2022-05-02",
                StartAt = new DateTime(2022, 5, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2022,
                Number = 2,
                ClosedAt = null
            };

            await BonusCalcContext.BonusPeriods.AddRangeAsync(bonusPeriods);
            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.CreateBonusPeriodAsync("2022-05-02");

            // Assert
            result.Should().BeEquivalentTo(bonusPeriod);
        }

        [Test]
        public async Task RetrievesBonusPeriodFromDb()
        {
            // Arrange
            var bonusPeriod = new BonusPeriod()
            {
                Id = "2021-11-01",
                StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Year = 2021,
                Number = 4,
                ClosedAt = new DateTime(2022, 02, 11, 17, 0, 0, DateTimeKind.Utc)
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetBonusPeriodAsync("2021-11-01");

            // Assert
            result.Should().BeEquivalentTo(bonusPeriod);
        }

        [Test]
        public async Task RetrievesBonusPeriodsFromDb()
        {
            // Arrange
            var bonusPeriods = new List<BonusPeriod>()
            {
                new BonusPeriod
                {
                    Id = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 4,
                    ClosedAt = new DateTime(2022, 02, 11, 17, 0, 0, DateTimeKind.Utc)
                },
                new BonusPeriod
                {
                    Id = "2022-01-31",
                    StartAt = new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2022,
                    Number = 1,
                    ClosedAt = null
                },
                new BonusPeriod
                {
                    Id = "2022-05-02",
                    StartAt = new DateTime(2022, 5, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2022,
                    Number = 2,
                    ClosedAt = null
                }
            };

            await BonusCalcContext.BonusPeriods.AddRangeAsync(bonusPeriods);
            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.GetBonusPeriodsAsync();

            // Assert
            result.Should().BeEquivalentTo(bonusPeriods);
        }

        [Test]
        public async Task RetrievesCurrentBonusPeriodsFromDb()
        {
            // Arrange
            var bonusPeriods = await AddBonusPeriods();

            // Act
            var result = await _classUnderTest.GetCurrentBonusPeriodsAsync(_currentDate);

            // Assert
            result.Should().BeEquivalentTo(bonusPeriods);
        }

        [Test]
        public async Task IgnoresClosedBonusPeriods()
        {
            // Arrange
            await AddClosedBonusPeriod();

            // Act
            var result = await _classUnderTest.GetCurrentBonusPeriodsAsync(_currentDate);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task IgnoresFutureBonusPeriods()
        {
            // Arrange
            await AddFutureBonusPeriod();

            // Act
            var result = await _classUnderTest.GetCurrentBonusPeriodsAsync(_currentDate);

            // Assert
            result.Should().BeEmpty();
        }

        [Test]
        public async Task RetrievesEarliestOpenBonusPeriod()
        {
            // Arrange
            await AddClosedBonusPeriod();
            await AddFutureBonusPeriod();

            var bonusPeriod = await AddBonusPeriod();

            // Act
            var result = await _classUnderTest.GetEarliestOpenBonusPeriodAsync();

            // Assert
            result.Should().BeEquivalentTo(bonusPeriod);
        }

        private async Task<IEnumerable<BonusPeriod>> AddBonusPeriods()
        {
            var bonusPeriods = new List<BonusPeriod>()
            {
                new BonusPeriod
                {
                    Id = "2021-08-02",
                    StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 3,
                    ClosedAt = null,
                    Weeks = new List<Week>()
                    {
                        new Week
                        {
                            Id = "2021-10-18",
                            StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                            Number = 12,
                            ClosedAt = null
                        }
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddRangeAsync(bonusPeriods);
            await BonusCalcContext.SaveChangesAsync();

            return bonusPeriods;
        }

        private async Task<BonusPeriod> AddBonusPeriod()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-11-01",
                StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Year = 2021,
                Number = 4,
                ClosedAt = null,
                Weeks = new List<Week>()
                {
                    new Week
                    {
                        Id = "2021-11-01",
                        StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                        Number = 1,
                        ClosedAt = null
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.SaveChangesAsync();

            return bonusPeriod;
        }

        private async Task AddClosedBonusPeriod()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2021,
                Number = 3,
                ClosedAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Weeks = new List<Week>()
                {
                    new Week
                    {
                        Id = "2021-10-18",
                        StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                        Number = 12,
                        ClosedAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.SaveChangesAsync();
        }

        private async Task AddFutureBonusPeriod()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2121-08-02",
                StartAt = new DateTime(2121, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2121,
                Number = 3,
                ClosedAt = new DateTime(2121, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Weeks = new List<Week>()
                {
                    new Week
                    {
                        Id = "2121-10-18",
                        StartAt = new DateTime(2121, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                        Number = 12,
                        ClosedAt = new DateTime(2121, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    }
                }
            };

            await BonusCalcContext.BonusPeriods.AddAsync(bonusPeriod);
            await BonusCalcContext.SaveChangesAsync();
        }
    }
}
