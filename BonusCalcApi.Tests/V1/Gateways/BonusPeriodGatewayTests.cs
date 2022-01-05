using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
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
        public async Task RetrievesCurrentBonusPeriodsFromDB()
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
