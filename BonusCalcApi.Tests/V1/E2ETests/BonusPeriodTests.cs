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
    public class BonusPeriodResponseComparer : IEqualityComparer<BonusPeriodResponse>
    {
        public bool Equals(BonusPeriodResponse bp1, BonusPeriodResponse bp2)
        {
            return bp2.Id == bp1.Id;
        }

        public int GetHashCode(BonusPeriodResponse bp)
        {
            return bp.Id.GetHashCode();
        }
    }

    public class BonusPeriodTests : IntegrationTests<Startup>
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CanGetCurrentBonusPeriods()
        {
            // Arrange
            await SeedBonusPeriods();

            var closedBonusPeriod = new BonusPeriodResponse()
            {
                Id = "2021-08-02"
            };

            var currentBonusPeriod = new BonusPeriodResponse()
            {
                Id = "2021-11-01"
            };

            var futureBonusPeriod = new BonusPeriodResponse()
            {
                Id = "2022-01-31"
            };

            var comparer = new BonusPeriodResponseComparer();

            // Act
            var (code, response) = await Get<List<BonusPeriodResponse>>($"/api/v1/periods/current?date=2021-12-05T16:00:00Z");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Contains.Item(currentBonusPeriod).Using(comparer));
            Assert.That(response, Does.Not.Contain(closedBonusPeriod).Using(comparer));
            Assert.That(response, Does.Not.Contain(futureBonusPeriod).Using(comparer));
        }

        private async Task SeedBonusPeriods()
        {
            var bonusPeriods = new List<BonusPeriod>()
            {
                new BonusPeriod()
                {
                    Id = "2021-08-02",
                    StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 3,
                    ClosedAt = new DateTime(2021, 11, 5, 17, 0, 0, DateTimeKind.Utc)
                },
                new BonusPeriod()
                {
                    Id = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 4,
                    ClosedAt = null
                },
                new BonusPeriod()
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
    }
}
