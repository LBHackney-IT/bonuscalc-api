using System;
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
        public async Task RetrievesWeekFromDB()
        {
            // Arrange
            var week = await AddWeek();

            // Act
            var result = await _classUnderTest.GetWeekAsync(week.Id);

            // Assert
            result.Should().BeEquivalentTo(week);
        }

        [Test]
        public async Task RetrievesNonExistentWeekFromDB()
        {
            // Act
            var result = await _classUnderTest.GetWeekAsync("2000-01-01");

            // Assert
            result.Should().BeNull();
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
