using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Infrastructure;
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
        public async Task CanCreateBonusPeriod()
        {
            // Arrange
            await SeedBonusPeriods();
            var request = new BonusPeriodRequest { Id = "2022-05-02" };

            // Act
            var (code, response) = await Post<BonusPeriodResponse>($"/api/v1/periods", request);

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Id, Is.EqualTo("2022-05-02"));
            Assert.That(response.StartAt, Is.EqualTo(new DateTime(2022, 5, 1, 23, 0, 0, 0, DateTimeKind.Utc)));
            Assert.That(response.Year, Is.EqualTo(2022));
            Assert.That(response.Number, Is.EqualTo(2));
            Assert.That(response.ClosedAt, Is.Null);
        }

        [Test]
        public async Task CanGetBonusPeriods()
        {
            // Arrange
            await SeedBonusPeriods();

            // Act
            var (code, response) = await Get<List<BonusPeriodResponse>>($"/api/v1/periods");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response[0].Id, Is.EqualTo("2021-08-02"));
            Assert.That(response[1].Id, Is.EqualTo("2021-11-01"));
            Assert.That(response[2].Id, Is.EqualTo("2022-01-31"));
        }

        [Test]
        public async Task CanGetBonusPeriod()
        {
            // Arrange
            await SeedBonusPeriods();

            var week = new WeekResponse()
            {
                Id = "2021-11-01",
                BonusPeriod = new BonusPeriodResponse()
                {
                    Id = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Year = 2021,
                    Number = 4
                },
                StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                Number = 1,
                ClosedAt = new DateTime(2021, 11, 13, 17, 0, 0, DateTimeKind.Utc)
            };

            var comparer = new WeekResponseComparer();

            // Act
            var (code, response) = await Get<BonusPeriodResponse>($"/api/v1/periods/2021-11-01");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Id, Is.EqualTo("2021-11-01"));
            Assert.That(response.Weeks, Contains.Item(week).Using(comparer));
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

        [Test]
        public async Task CanCloseBonusPeriod()
        {
            // Arrange
            await SeedBonusPeriods();

            var closedAt = new DateTime(2022, 5, 9, 16, 0, 0, DateTimeKind.Utc);
            var closedBy = "a.manager@hackney.gov.uk";

            var request = new BonusPeriodUpdate
            {
                ClosedAt = closedAt,
                ClosedBy = closedBy
            };

            // Act
            var (code, response) = await Post<BonusPeriodResponse>($"/api/v1/periods/2021-11-01", request);

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Id, Is.EqualTo("2021-11-01"));
            Assert.That(response.ClosedAt, Is.EqualTo(closedAt));
            Assert.That(response.ClosedBy, Is.EqualTo(closedBy));
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

            var weeks = new List<Week>()
            {
                new Week()
                {
                    Id = "2021-08-02",
                    BonusPeriodId = "2021-08-02",
                    StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                    Number = 1,
                    ClosedAt = new DateTime(2021, 8, 13, 17, 0, 0, DateTimeKind.Utc)
                },
                new Week()
                {
                    Id = "2021-11-01",
                    BonusPeriodId = "2021-11-01",
                    StartAt = new DateTime(2021, 11, 1, 0, 0, 0, DateTimeKind.Utc),
                    Number = 1,
                    ClosedAt = new DateTime(2021, 11, 13, 17, 0, 0, DateTimeKind.Utc)
                },
                new Week()
                {
                    Id = "2022-01-31",
                    BonusPeriodId = "2022-01-31",
                    StartAt = new DateTime(2022, 1, 31, 0, 0, 0, DateTimeKind.Utc),
                    Number = 1,
                    ClosedAt = null
                }
            };

            await Context.BonusPeriods.AddRangeAsync(bonusPeriods);
            await Context.Weeks.AddRangeAsync(weeks);
            await Context.SaveChangesAsync();

            Context.ChangeTracker.Clear();
        }
    }
}
