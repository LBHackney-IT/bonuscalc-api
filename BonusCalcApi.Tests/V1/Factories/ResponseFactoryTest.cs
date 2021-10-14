using System.Linq;
using AutoFixture;
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
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
        [Test]
        public void TimesheetResponseMapsCorrectly()
        {
            // Arrange
            var timesheet = _fixture.Create<Timesheet>();

            // Act
            var result = timesheet.ToResponse();

            // Assert
            result.TimesheetId.Should().Be(timesheet.Id);
            ValidateWeek(result.Week, timesheet.Week);
            foreach (var payElement in timesheet.PayElements)
            {
                var payElementResponse = result.PayElements.Single(pe => pe.PayElementId == payElement.Id);
                ValidatePayElement(payElementResponse, payElement);
            }
        }

        private static void ValidatePayElement(PayElementResponse payElementResponse, PayElement payElement)
        {
            payElementResponse.PayElementId.Should().Be(payElement.Id);
            payElementResponse.Address.Should().Be(payElement.Address);
            payElementResponse.Comment.Should().Be(payElement.Comment);
            payElementResponse.Duration.Should().Be(payElement.Duration);
            payElementResponse.Productive.Should().Be(payElement.Productive);
            payElementResponse.Value.Should().Be(payElement.Value);
            payElementResponse.WeekDay.Should().Be(payElement.WeekDay);
            payElementResponse.WorkOrder.Should().Be(payElement.WorkOrder);
            payElementResponse.PayElementTypeId.Should().Be(payElement.PayElementTypeId);
        }

        private static void ValidateWeek(WeekResponse weekResponse, Week week)
        {
            weekResponse.WeekId.Should().Be(week.WeekId);
            weekResponse.Number.Should().Be(week.Number);
            weekResponse.ClosedAt.Should().Be(week.ClosedAt);
            weekResponse.StartAt.Should().Be(week.StartAt);
            ValidateBonusPeriod(weekResponse.BonusPeriod, week.BonusPeriod);
        }

        private static void ValidateBonusPeriod(BonusPeriodResponse bonusPeriodResponse, BonusPeriod bonusPeriod)
        {
            bonusPeriodResponse.Period.Should().Be(bonusPeriod.Period);
            bonusPeriodResponse.Year.Should().Be(bonusPeriod.Year);
            bonusPeriodResponse.ClosedAt.Should().Be(bonusPeriod.ClosedAt);
            bonusPeriodResponse.StartAt.Should().Be(bonusPeriod.StartAt);
        }
    }
}
