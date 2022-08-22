using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Request;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.E2ETests
{
    public class OperativeTests : IntegrationTests<Startup>
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = FixtureHelpers.Fixture;
        }

        [Test]
        public async Task CanGetOperatives()
        {
            // Arrange
            var operatives = await SeedOperatives();
            var operativeId = operatives.First().Id;

            Context.ChangeTracker.Clear();

            // Act
            var (code, response) = await Get<List<OperativeResponse>>($"/api/v1/operatives?query={operativeId}");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(operatives.Select(o => o.ToResponse()).ToList());
        }

        [Test]
        public async Task CanGetOperative()
        {
            // Arrange
            var operative = await SeedOperative();

            // Act
            var (code, response) = await Get<OperativeResponse>($"/api/v1/operatives/{operative.Id}");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(operative.ToResponse());
        }

        [Test]
        public async Task CanGetSummary()
        {
            // Arrange
            var operative = await SeedOperative();
            var summary = await SeedSummary(operative);

            // Act
            var (code, response) = await Get<SummaryResponse>($"/api/v1/operatives/{operative.Id}/summary?bonusPeriod={summary.BonusPeriodId}");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(summary.ToResponse(), options =>
                options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation)).WhenTypeIs<DateTime>()
            );
        }

        [Test]
        public async Task CanGetTimesheet()
        {
            // Arrange
            var operative = await SeedOperative();
            var timesheet = await SeedTimesheet(operative);

            // Act
            var (code, response) = await Get<TimesheetResponse>($"/api/v1/operatives/{operative.Id}/timesheet?week={timesheet.WeekId}");

            // Assert
            code.Should().Be(HttpStatusCode.OK);
            response.Should().BeEquivalentTo(timesheet.ToResponse(), options =>
                options.Using<DateTime>(x => x.Subject.Should().BeCloseTo(x.Expectation)).WhenTypeIs<DateTime>()
            );
        }

        [Test]
        public async Task CanUpdateTimesheet()
        {
            // Arrange
            var payElementsTypes = await SeedPayElementTypes();
            var operative = await SeedOperative();
            var timesheet = await SeedTimesheet(operative, payElementsTypes);
            var newPayElement = CreatePayElementUpdate(payElementsTypes);
            var updatedPayElements = timesheet.PayElements.Select(pe => new PayElementUpdate
            {
                Id = pe.Id,
                PayElementTypeId = pe.PayElementTypeId,
                Address = "updated"
            }).ToList();
            var timesheetUpdate = _fixture.Build<TimesheetUpdate>()
                .With(r => r.Id, timesheet.Id)
                .With(r => r.PayElements, updatedPayElements.Concat(new[] { newPayElement }).ToList)
                .Create();

            // Act
            var (postCode, _) = await Post<TimesheetResponse>($"/api/v1/operatives/{operative.Id}/timesheet?week={timesheet.WeekId}", timesheetUpdate);
            var (getCode, response) = await Get<TimesheetResponse>($"/api/v1/operatives/{operative.Id}/timesheet?week={timesheet.WeekId}");

            // Assert
            postCode.Should().Be(HttpStatusCode.OK);
            getCode.Should().Be(HttpStatusCode.OK);

            var result = response.PayElements.Single(pe => pe.Address == newPayElement.Address);
            ValidatePayElement(result, newPayElement);

            foreach (var updatedPayElement in updatedPayElements)
            {
                var updatedResult = response.PayElements.Single(pe => pe.Id == updatedPayElement.Id);
                ValidatePayElement(updatedResult, updatedPayElement);
            }
        }

        [Test]
        public async Task CanUpdateReportSentAt()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var operative = await SeedOperative();
            var timesheet = await SeedTimesheet(operative);

            // Act
            var (postCode, _) = await Post<TimesheetResponse>($"/api/v1/operatives/{operative.Id}/timesheet/report/?week={timesheet.WeekId}", null);
            var (getCode, response) = await Get<TimesheetResponse>($"/api/v1/operatives/{operative.Id}/timesheet?week={timesheet.WeekId}");

            // Assert
            postCode.Should().Be(HttpStatusCode.OK);
            getCode.Should().Be(HttpStatusCode.OK);
            response.ReportSentAt.Should().BeOnOrAfter(now);
        }

        private static void ValidatePayElement(PayElementResponse payElement, PayElementUpdate expectedPayElement)
        {
            payElement.Address.Should().Be(expectedPayElement.Address);
            payElement.Comment.Should().Be(expectedPayElement.Comment);
            payElement.Monday.Should().Be(expectedPayElement.Monday);
            payElement.Tuesday.Should().Be(expectedPayElement.Tuesday);
            payElement.Wednesday.Should().Be(expectedPayElement.Wednesday);
            payElement.Thursday.Should().Be(expectedPayElement.Thursday);
            payElement.Friday.Should().Be(expectedPayElement.Friday);
            payElement.Saturday.Should().Be(expectedPayElement.Saturday);
            payElement.Sunday.Should().Be(expectedPayElement.Sunday);
            payElement.Duration.Should().Be(expectedPayElement.Duration);
            payElement.Value.Should().Be(expectedPayElement.Value);
            payElement.WorkOrder.Should().Be(expectedPayElement.WorkOrder);
            payElement.CostCode.Should().Be(expectedPayElement.CostCode);
            payElement.ClosedAt.Should().Be(expectedPayElement.ClosedAt);
        }

        private PayElementUpdate CreatePayElementUpdate(IEnumerable<PayElementType> payElementsTypes)
        {

            var newPayElement = _fixture.Build<PayElementUpdate>()
                .Without(pe => pe.Id)
                .Without(pe => pe.WorkOrder)
                .Without(pe => pe.CostCode)
                .With(pe => pe.PayElementTypeId, payElementsTypes.GetRandom().Id)
                .Create();
            return newPayElement;
        }

        private async Task<Summary> SeedSummary(Operative operative)
        {
            var bonusPeriod = FixtureHelpers.CreateBonusPeriod();
            await Context.BonusPeriods.AddAsync(bonusPeriod);

            var week = FixtureHelpers.CreateWeek(bonusPeriod);
            await Context.Weeks.AddAsync(week);

            var timesheet = _fixture.Build<Timesheet>()
                .With(t => t.Utilisation, 1.0M)
                .With(t => t.OperativeId, operative.Id)
                .Without(t => t.Operative)
                .With(t => t.WeekId, week.Id)
                .Without(t => t.Week)
                .Create();
            await Context.Timesheets.AddAsync(timesheet);
            await Context.SaveChangesAsync();

            var summary = await Context.Summaries
                .Include(s => s.BonusPeriod)
                .Include(s => s.WeeklySummaries)
                .Where(s => s.OperativeId == operative.Id && s.BonusPeriodId == bonusPeriod.Id)
                .SingleOrDefaultAsync();

            return summary;
        }

        private async Task<Timesheet> SeedTimesheet(Operative operative, IEnumerable<PayElementType> payElementsTypes = null)
        {
            var week = FixtureHelpers.CreateWeek();
            await Context.Weeks.AddAsync(week);

            if (!(payElementsTypes is null))
            {
                _fixture.Customize<PayElement>(composer => composer
                    .Without(pe => pe.PayElementType)
                    .Without(pe => pe.Timesheet)
                    .With(pe => pe.PayElementTypeId, () => payElementsTypes.GetRandom().Id)
                );
            }

            var timesheet = _fixture.Build<Timesheet>()
                .With(t => t.Utilisation, 1.0M)
                .Without(t => t.ReportSentAt)
                .With(t => t.OperativeId, operative.Id)
                .Without(t => t.Operative)
                .With(t => t.WeekId, week.Id)
                .Without(t => t.Week)
                .Create();
            await Context.Timesheets.AddAsync(timesheet);
            await Context.SaveChangesAsync();
            return timesheet;
        }

        private async Task<Operative> SeedOperative()
        {
            var operative = FixtureHelpers.CreateOperative();
            await Context.Operatives.AddAsync(operative);
            await Context.SaveChangesAsync();
            return operative;
        }

        private async Task<IEnumerable<Operative>> SeedOperatives()
        {
            return new List<Operative>() { await SeedOperative() };
        }

        private async Task<IEnumerable<PayElementType>> SeedPayElementTypes()
        {
            var types = _fixture.Build<PayElementType>()
                .Without(pet => pet.PayElements)
                .CreateMany();
            await Context.PayElementTypes.AddRangeAsync(types);
            await Context.SaveChangesAsync();
            return types;
        }
    }
}
