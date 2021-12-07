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
    public class WeekResponseComparer : IEqualityComparer<WeekResponse>
    {
        public bool Equals(WeekResponse w1, WeekResponse w2)
        {
            return w2.Id == w1.Id
                && w2.StartAt == w1.StartAt
                && w2.ClosedAt == w1.ClosedAt
                && w2.BonusPeriod.Id == w1.BonusPeriod.Id;
        }

        public int GetHashCode(WeekResponse w)
        {
            return w.Id.GetHashCode();
        }
    }

    public class OperativeSummaryResponseComparer : IEqualityComparer<OperativeSummaryResponse>
    {
        public bool Equals(OperativeSummaryResponse o1, OperativeSummaryResponse o2)
        {
            return o2.Id == o1.Id && o2.Name == o1.Name;
        }

        public int GetHashCode(OperativeSummaryResponse o)
        {
            return o.Id.GetHashCode();
        }
    }

    public class WeekTests : IntegrationTests<Startup>
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task CanGetWeek()
        {
            // Arrange
            var week = await SeedWeek();
            var operative = await SeedOperative();
            var comparer = new WeekResponseComparer();
            var operativeComparer = new OperativeSummaryResponseComparer();
            var operativeSummary = new OperativeSummaryResponse()
            {
                Id = operative.Id,
                Name = operative.Name
            };

            // Act
            var (code, response) = await Get<WeekResponse>($"/api/v1/weeks/2021-10-18");

            // Assert
            Assert.That(code, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response, Is.EqualTo(week.ToResponse()).Using(comparer));
            Assert.That(response.OperativeSummaries, Contains.Item(operativeSummary).Using(operativeComparer));
        }

        private async Task<Week> SeedWeek()
        {
            var week = new Week
            {
                Id = "2021-10-18",
                BonusPeriod = new BonusPeriod
                {
                    Id = "2021-08-02",
                    StartAt = new DateTime(2021, 8, 1, 23, 0, 0, DateTimeKind.Utc),
                    Year = 2020,
                    Number = 3,
                    ClosedAt = null
                },
                StartAt = new DateTime(2021, 10, 17, 23, 0, 0, DateTimeKind.Utc),
                Number = 12,
                ClosedAt = null
            };

            await Context.Weeks.AddAsync(week);
            await Context.SaveChangesAsync();

            return week;
        }

        private async Task<Operative> SeedOperative()
        {
            var operative = new Operative
            {
                Id = "123456",
                Name = "An Operative",
                EmailAddress = "an.operative@hackney.gov.uk",
                Trade = new Trade
                {
                    Id = "CP",
                    Description = "Carpenter"
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

            var timesheet = new Timesheet
            {
                Id = "123456/2021-10-18",
                OperativeId = "123456",
                WeekId = "2021-10-18",
                Utilisation = 1.0M
            };

            await Context.Operatives.AddAsync(operative);
            await Context.Timesheets.AddAsync(timesheet);
            await Context.SaveChangesAsync();

            return operative;
        }
    }
}
