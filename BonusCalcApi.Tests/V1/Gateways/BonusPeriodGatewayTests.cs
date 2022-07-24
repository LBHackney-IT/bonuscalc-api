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
