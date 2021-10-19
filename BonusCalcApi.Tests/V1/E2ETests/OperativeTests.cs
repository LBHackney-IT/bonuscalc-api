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
            var timesheetUpdate = _fixture.Build<TimesheetUpdateRequest>()
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
        private static void ValidatePayElement(PayElementResponse payElement, PayElementUpdate expectedPayElement)
        {
            payElement.Should().BeEquivalentTo(expectedPayElement.ToDb(), options => options
                .Excluding(pe => pe.Id)
                .Excluding(pe => pe.Timesheet)
                .Excluding(pe => pe.PayElementType)
                .Excluding(pe => pe.TimesheetId)
            );
        }
        private PayElementUpdate CreatePayElementUpdate(IEnumerable<PayElementType> payElementsTypes)
        {

            var newPayElement = _fixture.Build<PayElementUpdate>()
                .Without(pe => pe.Id)
                .Without(pe => pe.WorkOrder)
                .With(pe => pe.PayElementTypeId, payElementsTypes.GetRandom().Id)
                .Create();
            return newPayElement;
        }

        private async Task<Timesheet> SeedTimesheet(Operative operative, IEnumerable<PayElementType> payElementsTypes = null)
        {
            var week = _fixture.Build<Week>()
                .Without(w => w.Timesheets)
                .Create();
            await BonusCalcContext.Weeks.AddAsync(week);

            if (!(payElementsTypes is null))
            {
                _fixture.Customize<PayElement>(composer => composer
                    .Without(pe => pe.PayElementType)
                    .Without(pe => pe.Timesheet)
                    .With(pe => pe.PayElementTypeId, () => payElementsTypes.GetRandom().Id)
                );
            }
            ;

            var timesheet = _fixture.Build<Timesheet>()
                .With(t => t.OperativeId, operative.Id)
                .Without(t => t.Operative)
                .With(t => t.WeekId, week.Id)
                .Without(t => t.Week)
                .Create();
            await BonusCalcContext.Timesheets.AddAsync(timesheet);
            await BonusCalcContext.SaveChangesAsync();
            return timesheet;
        }

        private async Task<Operative> SeedOperative()
        {
            var operative = FixtureHelpers.CreateOperative();
            await BonusCalcContext.Operatives.AddAsync(operative);
            await BonusCalcContext.SaveChangesAsync();
            return operative;
        }

        private async Task<IEnumerable<PayElementType>> SeedPayElementTypes()
        {
            var types = _fixture.Build<PayElementType>()
                .Without(pet => pet.PayElements)
                .CreateMany();
            await BonusCalcContext.PayElementTypes.AddRangeAsync(types);
            await BonusCalcContext.SaveChangesAsync();
            return types;
        }
    }
}
