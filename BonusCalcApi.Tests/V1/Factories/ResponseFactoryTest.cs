using System.Linq;
using AutoFixture;
using BonusCalcApi.Tests.V1.Helpers;
using BonusCalcApi.V1.Boundary.Response;
using BonusCalcApi.V1.Factories;
using BonusCalcApi.V1.Infrastructure;
using FluentAssertions;
using NUnit.Framework;

namespace BonusCalcApi.Tests.V1.Factories
{
    public class ResponseFactoryTest
    {
        private readonly Fixture _fixture;
        public ResponseFactoryTest()
        {
            _fixture = FixtureHelpers.Fixture;
        }

        [Test]
        public void TimesheetResponseMapsCorrectly()
        {
            // Arrange
            var timesheet = _fixture.Create<Timesheet>();

            // Act
            var result = timesheet.ToResponse();

            // Assert
            result.Id.Should().Be(timesheet.Id);
            ValidateWeek(result.Week, timesheet.Week);

            foreach (var payElement in timesheet.PayElements)
            {
                var payElementResponse = result.PayElements.Single(pe => pe.Id == payElement.Id);
                ValidatePayElement(payElementResponse, payElement);
            }
        }

        [Test]
        public void SummaryResponseMapsCorrectly()
        {
            // Arrange
            var summary = FixtureHelpers.CreateSummary();

            // Act
            var result = summary.ToResponse();

            // Assert
            result.Id.Should().Be(summary.Id);
            ValidateBonusPeriod(result.BonusPeriod, summary.BonusPeriod);

            foreach (var weeklySummary in summary.WeeklySummaries)
            {
                var weeklySummaryResponse = result.WeeklySummaries.Single(ws => ws.Number == weeklySummary.Number);
                ValidateWeeklySummary(weeklySummaryResponse, weeklySummary);
            }
        }

        private static void ValidatePayElement(PayElementResponse payElementResponse, PayElement payElement)
        {
            payElementResponse.Id.Should().Be(payElement.Id);
            payElementResponse.Address.Should().Be(payElement.Address);
            payElementResponse.Comment.Should().Be(payElement.Comment);
            payElementResponse.Monday.Should().Be(payElement.Monday);
            payElementResponse.Tuesday.Should().Be(payElement.Tuesday);
            payElementResponse.Wednesday.Should().Be(payElement.Wednesday);
            payElementResponse.Thursday.Should().Be(payElement.Thursday);
            payElementResponse.Friday.Should().Be(payElement.Friday);
            payElementResponse.Saturday.Should().Be(payElement.Saturday);
            payElementResponse.Sunday.Should().Be(payElement.Sunday);
            payElementResponse.Duration.Should().Be(payElement.Duration);
            payElementResponse.Value.Should().Be(payElement.Value);
            payElementResponse.WorkOrder.Should().Be(payElement.WorkOrder);
            payElementResponse.ClosedAt.Should().Be(payElement.ClosedAt);
            ValidatePayElementType(payElementResponse.PayElementType, payElement.PayElementType);
        }

        private static void ValidatePayElementType(PayElementTypeResponse payElementTypeResponse, PayElementType payElementType)
        {
            payElementTypeResponse.Id.Should().Be(payElementType.Id);
            payElementTypeResponse.Description.Should().Be(payElementType.Description);
            payElementTypeResponse.PayAtBand.Should().Be(payElementType.PayAtBand);
            payElementTypeResponse.Paid.Should().Be(payElementType.Paid);
            payElementTypeResponse.Productive.Should().Be(payElementType.Productive);
            payElementTypeResponse.Adjustment.Should().Be(payElementType.Adjustment);
            payElementTypeResponse.OutOfHours.Should().Be(payElementType.OutOfHours);
            payElementTypeResponse.Overtime.Should().Be(payElementType.Overtime);
            payElementTypeResponse.Selectable.Should().Be(payElementType.Selectable);
            payElementTypeResponse.SmvPerHour.Should().Be(payElementType.SmvPerHour);
        }

        private static void ValidateWeek(WeekResponse weekResponse, Week week)
        {
            weekResponse.Id.Should().Be(week.Id);
            weekResponse.Number.Should().Be(week.Number);
            weekResponse.ClosedAt.Should().Be(week.ClosedAt);
            weekResponse.ClosedBy.Should().Be(week.ClosedBy);
            weekResponse.StartAt.Should().Be(week.StartAt);
            ValidateBonusPeriod(weekResponse.BonusPeriod, week.BonusPeriod);
        }

        private static void ValidateBonusPeriod(BonusPeriodResponse bonusPeriodResponse, BonusPeriod bonusPeriod)
        {
            bonusPeriodResponse.Number.Should().Be(bonusPeriod.Number);
            bonusPeriodResponse.Year.Should().Be(bonusPeriod.Year);
            bonusPeriodResponse.ClosedAt.Should().Be(bonusPeriod.ClosedAt);
            bonusPeriodResponse.StartAt.Should().Be(bonusPeriod.StartAt);
        }

        private static void ValidateWeeklySummary(WeeklySummaryResponse weeklySummaryResponse, WeeklySummary weeklySummary)
        {
            weeklySummaryResponse.Number.Should().Be(weeklySummary.Number);
            weeklySummaryResponse.StartAt.Should().Be(weeklySummary.StartAt);
            weeklySummaryResponse.ClosedAt.Should().Be(weeklySummary.ClosedAt);
            weeklySummaryResponse.ProductiveValue.Should().Be(weeklySummary.ProductiveValue);
            weeklySummaryResponse.NonProductiveDuration.Should().Be(weeklySummary.NonProductiveDuration);
            weeklySummaryResponse.NonProductiveValue.Should().Be(weeklySummary.NonProductiveValue);
            weeklySummaryResponse.TotalValue.Should().Be(weeklySummary.TotalValue);
            weeklySummaryResponse.ProjectedValue.Should().Be(weeklySummary.ProjectedValue);
        }
    }
}
