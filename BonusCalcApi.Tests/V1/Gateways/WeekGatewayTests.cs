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
    public class WeekGatewayTests : DatabaseTests
    {
        private WeekGateway _classUnderTest;

        [SetUp]
        public void Setup()
        {
            _classUnderTest = new WeekGateway(BonusCalcContext);
        }

        [Test]
        public async Task RetrievesWeekFromDb()
        {
            // Arrange
            var week = await AddWeek();

            // Act
            var result = await _classUnderTest.GetWeekAsync(week.Id);

            // Assert
            result.Should().BeEquivalentTo(week);
        }

        [Test]
        public async Task RetrievesNonExistentWeekFromDb()
        {
            // Act
            var result = await _classUnderTest.GetWeekAsync("2000-01-01");

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task RetrievesCountOfOpenWeeksFromDb()
        {
            // Arrange
            var bonusPeriod = new BonusPeriod
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
                        Number = 1,
                        ClosedAt = new DateTime(2022, 5, 11, 16, 0, 0, DateTimeKind.Utc)
                    },
                    new Week {
                        Id = "2022-05-09",
                        StartAt = new DateTime(2022, 5, 8, 23, 0, 0, DateTimeKind.Utc),
                        Number = 2
                    }
                }
            };

            await BonusCalcContext.AddAsync(bonusPeriod);
            await BonusCalcContext.SaveChangesAsync();

            // Act
            var result = await _classUnderTest.CountOpenWeeksAsync("2022-05-02");

            // Assert
            result.Should().Be(1);
        }

        private async Task<Week> AddWeek()
        {
            var bonusPeriod = new BonusPeriod
            {
                Id = "2021-08-02",
                StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                Year = 2021,
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
    }
}
